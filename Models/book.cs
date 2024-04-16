using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace minproject.Models
{
    public class book
    {
        [DisplayName("訂閱ID")] //主鍵，自動增加
        public int BookID { get; set; }

        [DisplayName("課程ID")] //主鍵兼外來鍵，關聯至Class的主鍵
        [Required]
        public int ClassID { get; set; }

        [DisplayName("開始訂閱時間")]
        [Required]
        public DateTime StartTime { get; set; }

        [DisplayName("訂閱結束時間")]
        [Required]
        public DateTime EndTime { get; set; }
    }
}