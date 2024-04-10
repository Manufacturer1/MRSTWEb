using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MRSTWEb.Models
{
    public class EditModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]

        public string Email { get; set; }
    }
}