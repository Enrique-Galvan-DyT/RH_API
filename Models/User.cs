namespace RH_API.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        public User()
        {
            creation_date = DateTime.Now;
            Status = false;
            IdRole = 6;
        }
        [Key]
        public int IdUser { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public DateTime creation_date { get; set; }

        public bool Status { get; set; }

        public int IdRole { get; set; }

        public virtual Role Role { get; set; }
    }
}
