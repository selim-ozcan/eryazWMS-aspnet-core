using System;
using Abp.Domain.Entities;

namespace eryaz.Movements
{
    public class DocumentMovementStatus : Entity<int>, ISoftDelete
    {
        public bool IsDeleted { get; set; }
        public Movement Movement { get; set; }
        public bool IsCompleted { get; set; }

        public DocumentMovementStatus()
        {
        }
    }
}

