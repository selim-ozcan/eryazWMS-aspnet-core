using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using eryaz.Customers;
using eryaz.Movements;
using eryaz.Warehouses;

namespace eryaz.Documents
{
    public class Document : Entity<int>, ISoftDelete
    {
        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; }

        public Customer Customer { get; set; }
        public IEnumerable<Movement> Movements { get; set; }

        public bool IsDeleted { get; set; }
    }
}

