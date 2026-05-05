using Linkverse.Application.Common.Responses;
using Linkverse.Application.Common.Responses.StudyPDFResponse;
using Linkverse.Application.DTO.StudyPDFDTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IServices
{
    public interface IStudyPDFService
    {
        Task<BaseResponse<StudyPDFResponseDto>> UploadAsync(Guid providerId, IFormFile file, UploadStudyPDFDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<StudyPDFResponseDto>> UpdateAsync(Guid providerId, Guid pdfId, UpdateStudyPDFDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<bool>> DeleteAsync(Guid providerId, Guid pdfId, CancellationToken cancellationToken);
        Task<BaseResponse<StudyPDFResponseDto>> GetByIdAsync(Guid pdfId, Guid userId, CancellationToken cancellationToken);
        Task<BaseResponse<PagedStudyPDFResponseDto>> SearchAsync(StudyPDFFilterDto filter, CancellationToken cancellationToken);
        Task<BaseResponse<PDFPurchaseResponseDto>> PurchaseAsync(Guid userId, PurchasePDFDto dto, CancellationToken cancellationToken);
        Task<BaseResponse<string>> GetDownloadUrlAsync(Guid userId, Guid pdfId, CancellationToken cancellationToken);
    }
}
