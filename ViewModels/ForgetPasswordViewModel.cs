using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace minproject.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [DisplayName("帳號")]
        [Required(ErrorMessage = "請輸入帳號")]
        [StringLength(30, ErrorMessage = "帳號不可超過30字")]
        public string Account { get; set; } = string.Empty;

        [DisplayName("Email")]
        [Required(ErrorMessage = "請輸入Email")]
        [StringLength(200, ErrorMessage = "Email不可超過200字")]
        [EmailAddress(ErrorMessage = "這不是Email格式")]
        public string Email { get; set; } = string.Empty;
    }
}