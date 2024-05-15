using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace minproject.Models.userans
{
    public class userans
    {
        [DisplayName("使用者答案編號")]
        public int UserAnsID { get; set; }
        [DisplayName("使用者答案編號")]
        public int ExamHistoryID { get; set; }
        [DisplayName("考試編號")]
        public int QuestionID { get; set; }
        [DisplayName("使用者答案")]
        public string? UserAnswer { get; set; }
        [DisplayName("對和錯")]
        public bool? TrueorFlase { get; set; }
    }
}