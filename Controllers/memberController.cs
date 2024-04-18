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

        #region new註冊
        [HttpPost]
        [AllowAnonymous]
        [Route("nRegister")]
        public IActionResult nRegister([FromBody] member Register)
        {
            if (_memberservice.repectAccount(Register.Account))
            {
                string hashedPassword = _memberservice.HashPassword(Register.Password);

                var validationCode = _mailservice.GetValidateCode();

                // 使用從 MailService 中獲取的驗證碼和哈希後的密碼
                member insert = new member
                {
                    Account = Register.Account,
                    Password = hashedPassword,
                    Email = Register.Email,
                    AuthCode = validationCode,
                    Role = "Student",
                    IsDelete = false
                };

                // 組合驗證連結的URL，這裡使用 Url.Action 來生成 URL
                string verifyUrl = $"{Request.Scheme}://{Request.Host}/api/Members/EmailValidate?Account={insert.Account}&AuthCode={insert.AuthCode}";

                // 讀取郵件模板
                // string tempMail = await System.IO.File.ReadAllTextAsync("RegisterEmailTemplate.html");
                string templPath = Path.Combine("RegisterEmailTemplate.html");
                string mailTemplate = System.IO.File.ReadAllText(templPath);

                // 將驗證碼插入郵件模板中
                string mailBody = _mailservice.GetRegisterMailBody(mailTemplate, Register.Account, verifyUrl);

                // 設定郵件標題
                // string mailSubject = "會員註冊確認信";

                // 寄送郵件
                _mailservice.SendRegisterMail(mailBody, Register.Email);

                // _context.Members.Add(insert);
                // await _context.SaveChangesAsync();

                _memberservice.Register(insert);
                return Ok("會員註冊成功，請至Email收信");
            }
            else
            {
                return BadRequest("此帳號已被註冊");
            }
        }
        #endregion


        #region Email驗證
        [HttpGet]
        [Route("EmailValidate")]
        public IActionResult EmailValidate(string Account, string AuthCode)
        {
            // 在這裡處理驗證連結的邏輯
            // 驗證 AuthCode 是否正確
            var user = _memberservice.GetDataByAccount(Account);
            if (user != null && user.AuthCode == AuthCode)
            {
                // 更新資料庫中的 AuthCode，設為 null
                _memberservice.UpdateAuthCode(Account);
                return Ok("驗證成功");
            }
            else
            {
                return BadRequest("驗證失敗");
            }
        }

        #endregion

        #region 註冊
        [AllowAnonymous]
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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        #region 登出
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return BadRequest("請登入");
            }
            else
            {
                Response.Cookies.Delete("jwtToken");
                return Ok("登出成功");
            }
        }

        #endregion
    }
}