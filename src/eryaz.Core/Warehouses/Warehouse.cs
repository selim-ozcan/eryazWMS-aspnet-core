using System;
using Abp.Domain.Entities;

namespace eryaz.Warehouses
{
    public class Warehouse : Entity<int>, ISoftDelete
    {
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public WarehouseType WarehouseType { get; set; }

        public bool IsDeleted { get; set; }
    }
}

