using System;
using System.ComponentModel.DataAnnotations;

namespace Dreamer.Models.Base
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public DateTimeOffset CreationDate { get; set; }
        public bool IsValid { get; set; } = true;
    }
}
