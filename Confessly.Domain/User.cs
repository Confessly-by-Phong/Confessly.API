using Confessly.Domain.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Confessly.Domain
{
    public class User : BaseEntity
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required] public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
