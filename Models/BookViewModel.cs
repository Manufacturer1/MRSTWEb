using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MRSTWEb.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string PathImage { get; set; }
        public decimal Price { get; set; }
    }
}