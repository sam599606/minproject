using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minproject.ViewModels
{
    public class CartItemViewModel
    {
        public int BookID { get; set; }
        public int LessonID { get; set; }
        public string LessonName { get; set; }
        public decimal Price { get; set; }
    }
}