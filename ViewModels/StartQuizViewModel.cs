using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using minproject.Models.question;

namespace minproject.ViewModels
{
    public class StartQuizViewModel
    {
        [DisplayName("帳號")]
        public string Account { get; set; }

        [DisplayName("題目年分")]
        public int Year { get; set; }

        [DisplayName("題目科目")]
        public int Type { get; set; }
        public question question { get; set; }
    }
}