using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace minproject.Models
{
    public class ExamHistory
    {
        [DisplayName("考試ID")] //主鍵，自動增加
        public int ExamHistoryID { get; set; }

        public string Account { get; set; }

        [DisplayName("考試時間")]
        public DateTime ExamDate { get; set; }

        [DisplayName("分數")]
        public int Score { get; set; }
    }
}