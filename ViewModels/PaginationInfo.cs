using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minproject.ViewModels
{
    public class PaginationInfo
    {
        public int NowPage { get; set; }
        public int TotalItems { get; set; }
        public int ItemNum { get; set; }
        public int TotalPages { get; set; }
    }
}