using System;
using eryaz.Customers;
using System.Collections.Generic;
using Abp.AutoMapper;
using eryaz.Customers.Dto;
using Abp.Application.Services.Dto;

namespace eryaz.Documents.Dto
{
    [AutoMap(typeof(DocumentHeader))]
    public class DocumentHeaderDto: EntityDto<int>
    {
        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime RegistrationDate { get; set; }

        public CustomerDto CustomerDto { get; set; }
        public int CustomerId { get; set; }

        public bool IsCompleted { get; set; } = false;
    }
}

