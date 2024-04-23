using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace minproject.Models
{
    public class Book
    {
        [DisplayName("訂閱ID")] //主鍵，自動增加
        public int BookID { get; set; }

        [DisplayName("課程ID")] //主鍵兼外來鍵，關聯至Lesson的主鍵
        public int LessonID { get; set; }
        public string? Account { get; set; }

        [DisplayName("開始訂閱時間")]
        public DateTime? StartTime { get; set; }

        [DisplayName("訂閱結束時間")]
        public DateTime? EndTime { get; set; }
        public bool IsOpen { get; set; }
    }
}