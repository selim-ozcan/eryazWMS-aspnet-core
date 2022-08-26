using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using eryaz.Customers;
using eryaz.Products;
using eryaz.Products.Dto;
using eryaz.Warehouses;

namespace eryaz.Movements.Dto
{
    [AutoMap(typeof(Movement))]
    public class CreateMovementDto
    {
        public DateTime MovementDate { get; set; }
        public MovementType MovementType { get; set; }

        public ProductDto ProductDto { get; set; }
        public int ProductId { get; set; }
        public int Stock { get; set; }

        public int WarehouseId { get; set; }
    }
}

