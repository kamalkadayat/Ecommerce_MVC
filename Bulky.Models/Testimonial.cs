using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models
{
    public class Testimonial
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; }
        public string ClientName { get; set; }
        public string? ClientImage { get; set; }
        [NotMapped]
        public IFormFile? ClientImageFile { get; set; }
    }
}
