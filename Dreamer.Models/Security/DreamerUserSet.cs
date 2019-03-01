using Dreamer.Models.Base;
using Dreamer.Models.Enums;
using Dreamer.Models.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dreamer.Models.Security
{
    [Table("DreamerUsers", Schema = "Security")]
    public class DreamerUserSet : BaseEntity
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        public DreamerUserRoles Role { get; set; }
        /// <summary>
        /// Will be updated on each autherized request
        /// </summary>
        public DateTimeOffset LastActivityDate { get; set; }

        public IEnumerable<CartItemSet> CartItems { get; set; }
    }
}
