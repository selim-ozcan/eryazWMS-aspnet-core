using System;
using Abp.Authorization.Roles;
using eryaz.Authorization.Roles;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Abp.Application.Services.Dto;

namespace eryaz.Customers.Dto
{
    [AutoMap(typeof(Customer))]
    public class CreateCustomerDto
    {
        public string CustomerCode { get; set; }
        public string Title { get; set; }
        public string TaxNo { get; set; }
        public string TaxOffice { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }

        public CreateCustomerDto()
        {
        }
    }
}

