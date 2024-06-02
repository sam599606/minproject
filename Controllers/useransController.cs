using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minproject.Models;
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
                _useransservice.InsertExamRecord(create, this.account);
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
            question que = _questionservice.GetDataById(edit.QuestionID);
            userans ans = _useransservice.GetDataById(edit.UserAnsID);
            if (que != null)
            {
                if (ans != null)
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

        #region 新增考試紀錄
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("CreateExamRecord")]
        public IActionResult CreateExamRecord([FromBody] List<userans> answers)
        {
            if (answers == null || !answers.Any())
            {
                return BadRequest("答案不可為空");
            }

            int totalScore = 0;
            foreach (var answer in answers)
            {
                question question = _questionservice.GetDataById(answer.QuestionID);
                if (question == null)
                {
                    return BadRequest($"無此題目 ID: {answer.QuestionID}");
                }

                // Calculate score (example: 1 point for each correct answer)
                if (answer.TrueorFlase == true)
                {
                    totalScore += 1;
                }
            }

            _useransservice.InsertExamRecord(answers, this.account, totalScore);
            return Ok("新增考試紀錄成功");
        }
        #endregion

        #region 查詢考試歷史紀錄
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpGet("GetExamHistory")]
        public IActionResult GetExamHistory()
        {
            List<ExamHistory> examHistory = _useransservice.GetExamHistory(this.account);
            if (examHistory == null || !examHistory.Any())
            {
                return NotFound("沒有考試紀錄");
            }

            return Ok(examHistory);
        }
        #endregion

        #region 查詢考試詳細資訊
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpGet("GetExamDetails/{examHistoryId}")]
        public IActionResult GetExamDetails(int examHistoryId)
        {
            List<userans> examDetails = _useransservice.GetExamDetails(examHistoryId);
            if (examDetails == null || !examDetails.Any())
            {
                return NotFound("沒有找到該考試的詳細資訊");
            }
            return Ok(examDetails);
        }
        #endregion

    }
}
