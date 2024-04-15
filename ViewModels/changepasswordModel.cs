using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using minproject.Models.member;

namespace minproject.ViewModels.changepasswordModel
{
    public class changepasswordModel
    {
        public string Account{get;set;}
        [DisplayName("舊密碼")]
        [Required(ErrorMessage = "請輸入舊密碼")]
        public string Password { get; set; }
        [DisplayName("新密碼")]
        [Required(ErrorMessage = "請輸入新密碼")]
        public string newPassword { get; set; }
        [DisplayName("新密碼確認")]
        [Required(ErrorMessage = "請輸入新密碼")]
        public string newPasswordcheck { get; set; }
    }
}