using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using eryaz.Customers;
using eryaz.Products;
using eryaz.Warehouses;

namespace eryaz.Movements.Dto
{
    [AutoMap(typeof(Movement))]
    public class CreateMovementDto : EntityDto<int>
    {
        public DateTime MovementDate { get; set; }
        public Product Product { get; set; }
        public MovementType MovementType { get; set; }
        public Warehouse Warehouse { get; set; }
        public int Stock { get; set; }
    }
}

