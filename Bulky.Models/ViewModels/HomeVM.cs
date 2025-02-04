using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Models.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Testimonial> Testimonials { get; set; }

        public IEnumerable<Blog> Blogs { get; set; }
    }
}
