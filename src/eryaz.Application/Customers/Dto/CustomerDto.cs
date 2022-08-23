using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace eryaz.Customers.Dto
{
    [AutoMapTo(typeof(Customer))]
    [AutoMapFrom(typeof(Customer))]
    public class CustomerDto : EntityDto<int>
    {
        public string CustomerCode { get; set; }
        public string Title { get; set; }
        public string TaxNo { get; set; }
        public string TaxOffice { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
    }
}

