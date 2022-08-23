using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using eryaz.Customers;
using eryaz.Customers.Dto;
using eryaz.Documents;
using eryaz.Movements;
using eryaz.Warehouses;

namespace eryaz.Documents.Dto
{
    [AutoMap(typeof(Document))]
    public class DocumentDto : EntityDto<int>
    {
        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; }

        public Customer Customer { get; set; }
        public IEnumerable<Movement> Movements { get; set; }
        public IEnumerable<DocumentMovementStatus>? MovementStatuses { get; set; }

        public DocumentDto()
        {
        }
    }
}

