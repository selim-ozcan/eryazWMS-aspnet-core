using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using eryaz.Authorization.Users;
using eryaz.Documents;

namespace eryaz.Customers
{
    public class Customer : Entity<int>, ISoftDelete
    {
        public string CustomerCode { get; set; }
        public string Title { get; set; }
        public string TaxNo { get; set; }
        public string TaxOffice { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }

        ICollection<DocumentHeader> Documents { get; set; }

        public bool IsDeleted { get; set; }
    }
}

