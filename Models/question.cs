using System.ComponentModel;
using System.Runtime.Serialization.Formatters;
using minproject.Models.member;

namespace minproject.Models.question
{
    public class question
    {
        [DisplayName("題目編號")]
        public int QuestionID { get; set; }
        [DisplayName("老師帳號")]
        public string? Account { get; set; }
        [DisplayName("題目科目")]
        public int Type { get; set; }
        [DisplayName("題目題號")]
        public int? QuestionNum { get; set; }
        [DisplayName("題目內容")]
        public string? Content { get; set; }
        [DisplayName("題目圖片")]
        public string? Image { get; set; }
        [DisplayName("題目正解")]
        public string? Answer { get; set; }
        [DisplayName("題目解析")]
        public string Solution { get; set; }
        [DisplayName("題目年分")]
        public int Year { get; set; }
        [DisplayName("題目新增時間")]
        public DateTime? CreateTime { get; set; }
        [DisplayName("題目修改時間")]
        public DateTime? EditTime { get; set; }
        [DisplayName("題目是否隱藏")]
        public bool IsDelete { get; set; }
    }
}