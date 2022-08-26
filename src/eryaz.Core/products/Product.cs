using System;
using Abp.Domain.Entities;

namespace eryaz.Products
{
    public class Product : Entity<int>, ISoftDelete
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }

        public bool IsDeleted { get; set ; }
    }
}

