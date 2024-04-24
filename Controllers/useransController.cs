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
            question createquestion = _questionservice.GetDataById(create.QuestionID);
            if (createquestion != null)
            {
                _useransservice.Insertans(create, this.account);
                return Ok("新增成功");
            }
            else
            {
                return BadRequest("無此題目");
            }
        }
        #endregion
        #region 更新答案
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("edit")]
        public IActionResult Edituserans(userans edit)
        {
            question editquestion = _questionservice.GetDataById(edit.QuestionID);
            if (editquestion != null)
            {
                if (edit != null)
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
        #endregion
        #region 對或錯
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("check")]
        public IActionResult TrueOrFlase(userans check)
        {
            string checkstr = _useransservice.trueORflase(check);
            return Ok(checkstr);
        }
        #endregion
        #region  取得使用者答案
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetAns")]
        public IActionResult GetUserAns()
        {
            List<userans> datalist = _useransservice.GetUserAns(this.account);
            if (datalist != null)
            {
                return Ok(datalist);
            }
            else
            {
                return NotFound("沒有作答紀錄");
            }
        }
        #endregion
        #region 計算得分
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("GetScore")]
        public IActionResult GetScore([FromBody] userans ans)
        {
            int score = _useransservice.GetScore(this.account);
            return Ok(score);
        }
        #endregion

    }
}
