using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Linkverse.Application.Cache;
using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.StudyPDFResponse;
using Linkverse.Application.DTO.StudyPDFDTO;
using Linkverse.Application.Interfaces.IRepositories;
using Linkverse.Application.Interfaces.IServices;
using Linkverse.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Linkverse.Persistence.Services
{
    public class StudyPDFService : IStudyPDFService
    {
        private readonly IStudyPDFRepository _repo;
        private readonly ICacheService _cache;
        private readonly ILogger<StudyPDFService> _logger;
        private readonly Cloudinary _cloudinary;

        private const string CachePrefix = "studypdf:";

        private const long MaxFileSizeBytes = 20 * 1024 * 1024;
        private const int ChunkSizeBytes = 6 * 1024 * 1024;

        public StudyPDFService(
            IStudyPDFRepository repo,
            ICacheService cache,
            ILogger<StudyPDFService> logger,
            IConfiguration configuration)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;

            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);

            _cloudinary = new Cloudinary(account) { Api = { Secure = true } };
        }

        public async Task<BaseResponse<StudyPDFResponseDto>> UploadAsync(
            Guid providerId, IFormFile file, UploadStudyPDFDto dto, CancellationToken cancellationToken)
        {
            try
            {
                if (file is null || file.Length == 0)
                    return BaseResponse<StudyPDFResponseDto>.Failure("No file provided.", statusCode: 400);

                if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                    return BaseResponse<StudyPDFResponseDto>.Failure("Only PDF files are accepted.", statusCode: 400);

                if (file.Length > MaxFileSizeBytes)
                    return BaseResponse<StudyPDFResponseDto>.Failure(
                        $"File size exceeds the {MaxFileSizeBytes / 1024 / 1024} MB limit.", statusCode: 400);

                string cloudinaryUrl;
                await using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new RawUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = "linkverse/notes",
                        PublicId = $"{providerId}_{dto.CourseCode.Replace(" ", "_")}_{Guid.NewGuid():N}",
                        UseFilename = false,
                        UniqueFilename = true,
                        Overwrite = false,
                    };

                    RawUploadResult result;

                    if (file.Length > ChunkSizeBytes)
                    {
                        result = await _cloudinary.UploadLargeRawAsync(uploadParams,
                            bufferSize: ChunkSizeBytes,
                            cancellationToken: cancellationToken);
                    }
                    else
                    {
                        result = await _cloudinary.UploadAsync(uploadParams);
                    }

                    if (result.Error is not null)
                    {
                        _logger.LogError("Cloudinary upload failed: {Error}", result.Error.Message);
                        return BaseResponse<StudyPDFResponseDto>.Failure(
                            "File upload failed. Please try again.", statusCode: 502);
                    }

                    cloudinaryUrl = result.SecureUrl.ToString();
                }

                var pdf = new StudyPDF
                {
                    ProviderId = providerId,
                    CourseCode = dto.CourseCode.ToUpper().Trim(),
                    Department = dto.Department,
                    Price = dto.Price,
                    Pages = dto.Pages,
                    FileUrl = cloudinaryUrl,
                    DownloadCount = 0
                };

                await _repo.AddAsync(pdf, cancellationToken);
                await _repo.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation(
                    "PDF uploaded — Id: {PdfId}, CourseCode: {CourseCode}, Provider: {ProviderId}",
                    pdf.Id, pdf.CourseCode, providerId);

                return BaseResponse<StudyPDFResponseDto>.Succes(
                    MapToResponse(pdf, false), statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading PDF for provider: {ProviderId}", providerId);
                return BaseResponse<StudyPDFResponseDto>.Failure(
                    "An error occurred during upload.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<StudyPDFResponseDto>> UpdateAsync(
            Guid providerId, Guid pdfId, UpdateStudyPDFDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var pdf = await _repo.GetByIdAsync(pdfId, cancellationToken);
                if (pdf is null)
                {
                    _logger.LogWarning("UpdatePDF — not found: {PdfId}", pdfId);
                    return BaseResponse<StudyPDFResponseDto>.Failure("PDF not found.", statusCode: 404);
                }

                if (pdf.ProviderId != providerId)
                {
                    _logger.LogWarning("UpdatePDF — unauthorized. ProviderId: {ProviderId}", providerId);
                    return BaseResponse<StudyPDFResponseDto>.Failure(
                        "You are not authorised to update this resource.", statusCode: 403);
                }

                if (dto.CourseCode is not null) pdf.CourseCode = dto.CourseCode.ToUpper().Trim();
                if (dto.Department is not null) pdf.Department = dto.Department;
                if (dto.Price.HasValue) pdf.Price = dto.Price.Value;
                if (dto.Pages.HasValue) pdf.Pages = dto.Pages.Value;

                pdf.ModifiedOn = DateTime.UtcNow;

                await _repo.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation("PDF updated — Id: {PdfId}", pdfId);

                var hasPurchased = await _repo.HasUserPurchasedAsync(pdfId, providerId, cancellationToken);
                return BaseResponse<StudyPDFResponseDto>.Succes(MapToResponse(pdf, hasPurchased));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating PDF: {PdfId}", pdfId);
                return BaseResponse<StudyPDFResponseDto>.Failure(
                    "An error occurred while updating.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<bool>> DeleteAsync(
            Guid providerId, Guid pdfId, CancellationToken cancellationToken)
        {
            try
            {
                var pdf = await _repo.GetByIdAsync(pdfId, cancellationToken);
                if (pdf is null)
                    return BaseResponse<bool>.Failure("PDF not found.", statusCode: 404);

                if (pdf.ProviderId != providerId)
                    return BaseResponse<bool>.Failure(
                        "You are not authorised to delete this resource.", statusCode: 403);

                pdf.IsDeleted = true;
                pdf.DeletedOn = DateTime.UtcNow;
                pdf.ModifiedOn = DateTime.UtcNow;

                await _repo.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation("PDF soft-deleted — Id: {PdfId}", pdfId);

                return BaseResponse<bool>.Succes(true, "PDF deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PDF: {PdfId}", pdfId);
                return BaseResponse<bool>.Failure("An error occurred.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<StudyPDFResponseDto>> GetByIdAsync(
            Guid pdfId, Guid userId, CancellationToken cancellationToken)
        {
            try
            {
                var pdf = await _repo.GetByIdAsync(pdfId, cancellationToken);
                if (pdf is null)
                {
                    _logger.LogWarning("GetPDF — not found: {PdfId}", pdfId);
                    return BaseResponse<StudyPDFResponseDto>.Failure("PDF not found.", statusCode: 404);
                }

                var hasPurchased = await _repo.HasUserPurchasedAsync(pdfId, userId, cancellationToken);
                return BaseResponse<StudyPDFResponseDto>.Succes(MapToResponse(pdf, hasPurchased));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PDF: {PdfId}", pdfId);
                return BaseResponse<StudyPDFResponseDto>.Failure("An error occurred.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<PagedStudyPDFResponseDto>> SearchAsync(
            StudyPDFFilterDto filter, CancellationToken cancellationToken)
        {
            try
            {
                var cacheKey = CacheKeys.StudyPDFs(filter.ToHashKey());

                var result = await _cache.GetOrSetAsync(
                    cacheKey,
                    async () =>
                    {
                        var (items, total) = await _repo.GetPagedAsync(filter, cancellationToken);
                        return new PagedStudyPDFResponseDto
                        {
                            Items = items.Select(p => new StudyPDFSummaryDto
                            {
                                Id = p.Id,
                                CourseCode = p.CourseCode,
                                Department = p.Department,
                                Price = p.Price,
                                Pages = p.Pages,
                                DownloadCount = p.DownloadCount,
                                ProviderName = p.Provider?.BusinessName ?? string.Empty
                            }).ToList(),
                            TotalCount = total,
                            Page = filter.Page,
                            PageSize = filter.PageSize
                        };
                    },
                    CacheTTL.StudyPDFListings,
                    cancellationToken);

                return BaseResponse<PagedStudyPDFResponseDto>.Succes(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching PDFs");
                return BaseResponse<PagedStudyPDFResponseDto>.Failure(
                    "An error occurred while searching.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<PDFPurchaseResponseDto>> PurchaseAsync(
            Guid userId, PurchasePDFDto dto, CancellationToken cancellationToken)
        {
            try
            {
                var pdf = await _repo.GetByIdAsync(dto.NoteId, cancellationToken);
                if (pdf is null)
                    return BaseResponse<PDFPurchaseResponseDto>.Failure("PDF not found.", statusCode: 404);

                var alreadyPurchased = await _repo.HasUserPurchasedAsync(dto.NoteId, userId, cancellationToken);
                if (alreadyPurchased)
                    return BaseResponse<PDFPurchaseResponseDto>.Failure(
                        "You have already purchased this PDF.", statusCode: 409);

                var purchase = new PDFPurchase
                {
                    NoteId = dto.NoteId,
                    UserId = userId,
                    PurchasedAt = DateTime.UtcNow
                };

                await _repo.AddPurchaseAsync(purchase, cancellationToken);

                pdf.DownloadCount++;
                pdf.ModifiedOn = DateTime.UtcNow;

                await _repo.SaveChangesAsync(cancellationToken);

                _cache.RemoveByPrefix(CachePrefix);

                _logger.LogInformation(
                    "PDF purchased — NoteId: {NoteId}, UserId: {UserId}", dto.NoteId, userId);

                return BaseResponse<PDFPurchaseResponseDto>.Succes(new PDFPurchaseResponseDto
                {
                    Id = purchase.Id,
                    NoteId = purchase.NoteId,
                    CourseCode = pdf.CourseCode,
                    FileUrl = pdf.FileUrl,
                    PurchasedAt = purchase.PurchasedAt
                }, statusCode: 201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing purchase — NoteId: {NoteId}", dto.NoteId);
                return BaseResponse<PDFPurchaseResponseDto>.Failure(
                    "An error occurred during purchase.", statusCode: 500);
            }
        }

        public async Task<BaseResponse<string>> GetDownloadUrlAsync(
            Guid userId, Guid pdfId, CancellationToken cancellationToken)
        {
            try
            {
                var hasPurchased = await _repo.HasUserPurchasedAsync(pdfId, userId, cancellationToken);
                if (!hasPurchased)
                    return BaseResponse<string>.Failure(
                        "You must purchase this PDF before downloading.", statusCode: 403);

                var pdf = await _repo.GetByIdAsync(pdfId, cancellationToken);
                if (pdf is null)
                    return BaseResponse<string>.Failure("PDF not found.", statusCode: 404);

                _logger.LogInformation(
                    "Download URL served — PdfId: {PdfId}, UserId: {UserId}", pdfId, userId);

                return BaseResponse<string>.Succes(pdf.FileUrl, "Download URL retrieved.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error serving download URL — PdfId: {PdfId}", pdfId);
                return BaseResponse<string>.Failure("An error occurred.", statusCode: 500);
            }
        }

        private static StudyPDFResponseDto MapToResponse(StudyPDF pdf, bool alreadyPurchased) => new()
        {
            Id = pdf.Id,
            ProviderId = pdf.ProviderId,
            ProviderName = pdf.Provider?.BusinessName ?? string.Empty,
            CourseCode = pdf.CourseCode,
            Department = pdf.Department,
            Price = pdf.Price,
            Pages = pdf.Pages,
            FileUrl = alreadyPurchased ? pdf.FileUrl : string.Empty,
            DownloadCount = pdf.DownloadCount,
            CreatedAt = pdf.CreatedAt,
            AlreadyPurchased = alreadyPurchased
        };
    }
}
