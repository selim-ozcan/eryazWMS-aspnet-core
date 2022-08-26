using System;
using Abp.Application.Services.Dto;

namespace eryaz.Movements.Dto
{
    public class PagedMovementResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IncludeDeleted { get; set; }

        public PagedMovementResultRequestDto()
        {
        }
    }
}

