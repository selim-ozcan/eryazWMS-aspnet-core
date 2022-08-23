using System;
using Abp.Application.Services.Dto;

namespace eryaz.Documents.Dto
{
    public class PagedDocumentResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }

        public PagedDocumentResultRequestDto()
        {
        }
    }
}

