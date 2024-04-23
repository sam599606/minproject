using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minproject.Models.member;
using minproject.Models.question;
using minproject.Models.userans;
using minproject.Services.memberService;
using minproject.Services.questionService;
using minproject.Services.useransService;
using minproject.ViewModels;

namespace minproject.Controllers.useransController
{
    [ApiController]
    [Route("api/[controller]")]
    public class useransController : ControllerBase
    {
        private readonly useransService _useransservice;
        private readonly questionService _questionservice;
        private readonly memberService _memberservice;
        private readonly string account;

        public useransController(useransService useransservice, questionService questionservice, memberService memberservice, IHttpContextAccessor httpContextAccessor)
        {
            _useransservice = useransservice;
            _questionservice = questionservice;
            _memberservice = memberservice;
            account = httpContextAccessor.HttpContext.User.Identity.Name;

        }
        #region 新增答案
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("create")]
        public IActionResult Createuserans(userans create)
        {
            member createmember = _memberservice.GetDataByAccount(this.account);
            question createquestion = _questionservice.GetDataById(create.QuestionID);
            if (createmember != null)
            {
                if (createquestion != null)
                {
                    _useransservice.Insertans(create);
                    return Ok("新增成功");
                }
                else
                {
                    return BadRequest("無此題目");
                }
            }
            else
            {
                return BadRequest("無此帳號");
            }
        }
        #endregion
        #region 更新答案
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("edit")]
        public IActionResult Edituserans(userans edit)
        {
            userans data = _useransservice.GetDataById(edit.UserAnsID);
            question editquestion = _questionservice.GetDataById(edit.QuestionID);
            member editmember = _memberservice.GetDataByAccount(this.account);
            if (editmember != null)
            {
                if (editquestion != null)
                {
                    if (data != null)
                    {
                        _useransservice.Editans(edit);
                        return Ok("更新成功");
                    }
                    else
                    {
                        return BadRequest("無此答案");
                    }
                }
                else
                {
                    return BadRequest("無此題目");
                }
            }
            else
            {
                return BadRequest("無此帳號");
            }
        }
        #endregion

        #region 開始測驗
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("StartQuiz")]
        public IActionResult StartQuiz(StartQuizViewModel Quiz)
        {
            member user = _memberservice.GetDataByAccount(this.account);
            if (user == null)
            {
                return BadRequest("無此帳號");
            }

            List<question> questions = _questionservice.GetQuiz(Quiz.question);

            if (questions.Count > 0)
            {
                return Ok(questions);
            }
            else
            {
                return NotFound("找不到任何題目");
            }
        }
        #endregion


    }
}
