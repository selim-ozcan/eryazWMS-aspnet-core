using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using eryaz.Customers;
using eryaz.Customers.Dto;
using eryaz.Documents;
using eryaz.Documents.Dto;
using eryaz.Warehouses;

namespace eryaz.Documents.Dto
{
    [AutoMapTo(typeof(Document))]
    public class CreateDocumentDto : EntityDto<int>
    {
        public string DocumentNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string Status { get; set; }

        public int CustomerId { get; set; }
        public IEnumerable<int> ProductIds {get; set;}
        public List<int> Stocks { get; set; }

        public CreateDocumentDto()
        {
        }
    }
}

