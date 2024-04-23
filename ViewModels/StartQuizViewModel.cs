using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace minproject.ViewModels
{
    public class StartQuizViewModel
    {
        [DisplayName("帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(30, ErrorMessage = "帳號不可超過30字")]
        public string Account { get; set; }

        [DisplayName("題目年分")]
        public int Year { get; set; }

        [DisplayName("題目科目")]
        public int Type { get; set; }
    }
}