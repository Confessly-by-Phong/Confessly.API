using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Confessly.Domain.Core
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public bool IsDeleted { get; set; } = false;
        public Guid CreatedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }
    }
}
