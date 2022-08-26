using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using eryaz.Customers;

namespace eryaz.Documents
{
    public class DocumentHeader : Entity<int>, ISoftDelete
    {
        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        
        public Customer Customer { get; set; }
        public int CustomerId { get; set; }

        public bool IsCompleted { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public ICollection<DocumentDetail> DocumentDetails { get; set; }
    }
}

