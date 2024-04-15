using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using minproject.Models.member;
using minproject.Services.JwtService;
using minproject.Services.MailService;
using minproject.Services.memberService;
using minproject.ViewModels.changepasswordModel;

namespace minproject.Controllers.memberController
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class memberController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly memberService _memberservice;
        private readonly MailService _mailservice;
        private readonly JwtService _jwtservice;
        private readonly IWebHostEnvironment _env;

        public memberController(IConfiguration configuration, memberService memberservice, MailService mailservice, JwtService jwtservice, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _memberservice = memberservice;
            _mailservice = mailservice;
            _jwtservice = jwtservice;
            _env = env;
        }
        #region 註冊
        [HttpPost("register")]
        public IActionResult Register([FromBody] member Register)
        {
            if (_memberservice.repectAccount(Register.Account))
            {
                string password = _memberservice.GetPassowrd();
                Register.Password = password;
                _memberservice.Register(Register);
                string toMail = Register.Email;
                string templPath = Path.Combine("RegisterEmail.html");
                string mailTemplate = System.IO.File.ReadAllText(templPath);
                string userPassword = password;
                string userName = Register.Account;
                string mailBody = _mailservice.GetMailBody(mailTemplate, userName, userPassword);
                _mailservice.SendRegisterMail(mailBody, toMail);
                return Ok("註冊成功請去收信");
            }
            else
            {
                return BadRequest("此帳號已被註冊");
            }
        }
        #endregion
        #region 登入
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] member Login)
        {
            string Loginstr = _memberservice.Logincheck(Login.Account, Login.Password);
            member members = _memberservice.GetDataByAccount(Login.Account);
            if (string.IsNullOrEmpty(Loginstr))
            {
                // return Token
                var token = _jwtservice.GenerateJwtToken(members.Account, members.Role);
                Response.Cookies.Append("jwtToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1)
                });

                members.Password = null;
                string Message = "登入成功";
                return Ok(new { token, members, Message });
            }
            else
            {
                return BadRequest(Loginstr);
            }
        }
        #endregion
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("changepassword")]
        #region 變更密碼
        public IActionResult ChangePassword(changepasswordModel Change)
        {
            // var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            // return Ok(roleClaim);

            string changestr = _memberservice.ChangePassword(Change.Account, Change.Password, Change.newPassword);
            if (string.IsNullOrEmpty(changestr))
            {
                return Ok("密碼更改成功");
            }
            else
            {
                return BadRequest(changestr);
            }
        }
        #endregion
        [AllowAnonymous]
        [HttpPost("forgetpassword")]
        #region 忘記密碼
        public IActionResult ForgetPassword(member Forget)
        {
            string password = _memberservice.GetPassowrd();
            string forgetstr = _memberservice.ForgetPassword(Forget, Forget.Email, password);
            if (string.IsNullOrEmpty(forgetstr))
            {
                Forget.Password = password;
                string toMail = Forget.Email;
                string templPath = Path.Combine("ForgetPassword.html");
                string mailTemplate = System.IO.File.ReadAllText(templPath);
                string userPassword = password;
                string userName = Forget.Account;
                string mailBody = _mailservice.GetMailBody(mailTemplate, userName, userPassword);
                _mailservice.SendRegisterMail(mailBody, toMail);
                return Ok("已將新密碼發送至信箱");
            }
            else
            {
                return BadRequest(forgetstr);
            }
        }
        #endregion
    }
}