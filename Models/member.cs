using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace minproject.Models.member
{
    public class member
    {
        [DisplayName("帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(30, ErrorMessage = "帳號不可超過30字")]
        public string Account { get; set; }
        [DisplayName("密碼")]
        public string? Password { get; set; }
        [DisplayName("Email")]
        [StringLength(200, ErrorMessage = "Email不可超過200字")]
        [EmailAddress(ErrorMessage = "這不是Email格式")]
        public string? Email { get; set; }
        [DisplayName("身分")]
        public string? Role { get; set; }
        public bool? IsDelete { get; set; }
    }
}