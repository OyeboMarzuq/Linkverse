using Linkverse.Application.DTO.StudyPDFDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linkverse.Application.Common.Responses.StudyPDFResponse
{
    public class PagedStudyPDFResponseDto
    {
        public List<StudyPDFSummaryDto> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
