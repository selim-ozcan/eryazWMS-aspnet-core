using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;

namespace eryaz.Warehouses.Dto
{
    [AutoMap(typeof(Warehouse))]
    public class CreateWarehouseDto
    {
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public WarehouseType WarehouseType { get; set; }

        public CreateWarehouseDto()
        {
        }
    }
}

