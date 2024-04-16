using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace minproject.Models
{
    public class booksave
    {
        [DisplayName("訂閱ID")] //主鍵兼外來鍵，關聯至Book的主鍵
        public int BookID { get; set; }

        [DisplayName("帳號")] //主鍵兼外來鍵，關聯至Member的主鍵
        [Required]
        [MaxLength(30)]
        public string Account { get; set; } = string.Empty;
    }
}