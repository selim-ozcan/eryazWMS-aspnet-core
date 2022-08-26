using System;
using Abp.Application.Services.Dto;

namespace eryaz.Warehouses.Dto
{
    public class PagedWarehouseResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public bool? IncludeDeleted { get; set; }

        public PagedWarehouseResultRequestDto()
        {
        }
    }
}

