using System;
using Abp.Application.Services.Dto;

namespace eryaz.Customers.Dto
{
    public class PagedCustomerResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
        public PagedCustomerResultRequestDto()
        {
        }
    }
}

