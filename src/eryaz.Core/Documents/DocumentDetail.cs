using System;
using Abp.Domain.Entities;
using eryaz.Movements;
using eryaz.Products;
using eryaz.Warehouses;

namespace eryaz.Documents
{
    public class DocumentDetail : Entity<int>, ISoftDelete
    {
        public DateTime DetailDate { get; set; }

        public Product Product { get; set; }
        public int ProductId { get; set; }
        public int Stock { get; set; }

        public bool IsCompleted { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

        public DocumentHeader DocumentHeader { get; set; }
        public int DocumentHeaderId { get; set; }
    }
}

