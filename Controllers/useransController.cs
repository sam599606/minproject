using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minproject.Models.member;
using minproject.Models.question;
using minproject.Models.userans;
using minproject.Services.memberService;
using minproject.Services.questionService;
using minproject.Services.useransService;

namespace minproject.Controllers.useransController
{
    [ApiController]
    [Route("api/[controller]")]
    public class useransController : ControllerBase
    {
        private readonly useransService _useransservice;
        private readonly questionService _questionservice;
        private readonly memberService _memberservice;

        public useransController(useransService useransservice, questionService questionservice, memberService memberservice)
        {
            _useransservice = useransservice;
            _questionservice = questionservice;
            _memberservice = memberservice;
        }
        #region 新增答案
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("create")]
        public IActionResult Createuserans(userans create)
        {
            member createmember = _memberservice.GetDataByAccount(create.Account);
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
            member editmember = _memberservice.GetDataByAccount(edit.Account);
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("start")]
        public IActionResult StartExam(int type, int year)
        {
            string account = HttpContext.User.Identity.Name;
            _useransservice.ClearUserAnswers();
            List<question> questions = _questionservice.GetQuestionsByTypeAndYear(type, year, account);
            // foreach (var q in questions)
            // {
            //     userans userAnswerRecord = new userans
            //     {
            //         QuestionID = q.QuestionID,
            //         Account = account
            //     };

            //     // 將記錄插入到 userans 資料表中
            //     _useransservice.InsertUserAnswer(userAnswerRecord);
            // }
            return Ok("考試開始，題目已加載！");
        }
    }
}
