using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using eryaz.Authorization.Users;

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

        public bool IsDeleted { get; set; }
    }
}

