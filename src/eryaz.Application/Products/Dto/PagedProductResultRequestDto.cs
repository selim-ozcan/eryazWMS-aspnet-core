using System;
using Abp.Application.Services.Dto;

namespace eryaz.Products.Dto
{
    public class PagedProductResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }

        public PagedProductResultRequestDto()
        {
        }
    }
}

