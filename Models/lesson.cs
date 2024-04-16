using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using minproject.Models.member;

namespace minproject.Models.lesson
{
    public class lesson
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [DisplayName("課程編號")]
        public int LessonID { get; set; }
        [DisplayName("老師帳號")]
        public string? Account { get; set; }
        [DisplayName("課程科目")]
        public int? Type { get; set; }
        [DisplayName("課程價格")]
        public int? Price { get; set; }
        [DisplayName("課程介紹")]
        public string? Content { get; set; }
        [DisplayName("課程影片")]
        public string? Video { get; set; }
        [DisplayName("課程年分")]
        public int? Year { get; set; }
        [DisplayName("課程新增時間")]
        public DateTime? CreateTime { get; set; }
        [DisplayName("課程修改時間")]
        public DateTime? EditTime { get; set; }
    }
}