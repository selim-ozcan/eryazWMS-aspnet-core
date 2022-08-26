using System;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using eryaz.Products;
using eryaz.Products.Dto;

namespace eryaz.Documents.Dto
{
    [AutoMap(typeof(DocumentDetail))]
    public class DocumentDetailDto: EntityDto<int>
    {
        public DateTime DetailDate { get; set; }

        public ProductDto ProductDto { get; set; }
        public int ProductId { get; set; }
        public int Stock { get; set; }

        public DocumentHeaderDto DocumentHeaderDto{get; set; }

        public bool IsCompleted { get; set; } = false;
    }
}

