using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace minproject.Models
{
    public class Bought
    {
        [DisplayName("課程科目")]
        public int? Type { get; set; }
        [DisplayName("課程介紹")]
        public string? Content { get; set; }
        [DisplayName("課程影片")]
        public string? Video { get; set; }
        [DisplayName("課程年分")]
        public int? Year { get; set; }
        [DisplayName("開始訂閱時間")]
        public DateTime? StartTime { get; set; }
        [DisplayName("訂閱結束時間")]
        public DateTime? EndTime { get; set; }
    }
}