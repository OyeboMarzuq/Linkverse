using Linkverse.Application.DTO.StudyPDFDTO;
using Linkverse.Application.Interfaces.IRepositories;
using Linkverse.Domain.Entities;
using Linkverse.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Persistence.Repositories
{
    public class StudyPDFRepository : IStudyPDFRepository
    {
        private readonly ApplicationDbContext _context;

        public StudyPDFRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<StudyPDF?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.StudyPDFs
                .AsNoTracking()
                .Include(s => s.Provider)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        public async Task<(List<StudyPDF> Items, int TotalCount)> GetPagedAsync(
            StudyPDFFilterDto filter, CancellationToken cancellationToken)
        {
            var query = _context.StudyPDFs
                .AsNoTracking()
                .Include(s => s.Provider)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.CourseCode))
            {
                var code = filter.CourseCode.Trim().ToUpper();
                query = query.Where(s => s.CourseCode.ToUpper().Contains(code));
            }

            if (!string.IsNullOrWhiteSpace(filter.Department))
            {
                var dept = filter.Department.Trim().ToLower();
                query = query.Where(s => s.Department.ToLower().Contains(dept));
            }

            if (filter.MaxPrice.HasValue)
                query = query.Where(s => s.Price <= filter.MaxPrice.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(s =>
                    s.CourseCode.ToLower().Contains(term) ||
                    s.Department.ToLower().Contains(term));
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(s => s.DownloadCount)
                .ThenByDescending(s => s.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }

        public async Task<List<StudyPDF>> GetByProviderAsync(Guid providerId, CancellationToken cancellationToken)
            => await _context.StudyPDFs
                .AsNoTracking()
                .Where(s => s.ProviderId == providerId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);

        public async Task<bool> HasUserPurchasedAsync(Guid noteId, Guid userId, CancellationToken cancellationToken)
            => await _context.PDFPurchases
                .AnyAsync(p => p.NoteId == noteId && p.UserId == userId, cancellationToken);

        public async Task AddAsync(StudyPDF pdf, CancellationToken cancellationToken)
            => await _context.StudyPDFs.AddAsync(pdf, cancellationToken);

        public async Task AddPurchaseAsync(PDFPurchase purchase, CancellationToken cancellationToken)
            => await _context.PDFPurchases.AddAsync(purchase, cancellationToken);

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);
    }
}
