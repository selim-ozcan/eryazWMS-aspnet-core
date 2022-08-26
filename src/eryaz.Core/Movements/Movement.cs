using System;
using Abp.Domain.Entities;
using eryaz.Customers;
using eryaz.Products;
using eryaz.Warehouses;

namespace eryaz.Movements
{
    public class Movement : Entity<int>, ISoftDelete
    {
        public DateTime MovementDate { get; set; }
        public MovementType MovementType { get; set; }

        public Product Product { get; set; }
        public int ProductId { get; set; }
        public int Stock { get; set; }

        public Warehouse Warehouse { get; set; }
        public int WarehouseId { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}

