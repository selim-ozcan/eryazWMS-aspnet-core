using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using eryaz.Products;

namespace eryaz.Products.Dto
{
    [AutoMap(typeof(Product))]
    public class ProductDto : EntityDto<int>
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }

        public ProductDto()
        {
        }
    }
}

