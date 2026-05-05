using Linkverse.Application.DTO.StudyPDFDTO;
using Linkverse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Interfaces.IRepositories
{
    public interface IStudyPDFRepository
    {
        Task<StudyPDF?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<(List<StudyPDF> Items, int TotalCount)> GetPagedAsync(StudyPDFFilterDto filter, CancellationToken cancellationToken);
        Task<List<StudyPDF>> GetByProviderAsync(Guid providerId, CancellationToken cancellationToken);
        Task<bool> HasUserPurchasedAsync(Guid noteId, Guid userId, CancellationToken cancellationToken);
        Task AddAsync(StudyPDF pdf, CancellationToken cancellationToken);
        Task AddPurchaseAsync(PDFPurchase purchase, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
