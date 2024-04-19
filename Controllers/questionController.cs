using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minproject.Models.member;
using minproject.Models.question;
using minproject.Services.memberService;
using minproject.Services.questionService;
using minproject.ViewModels.addquestion;

namespace minproject.Controllers.questionController
{
    [ApiController]
    [Route("api/[controller]")]
    public class questionController : ControllerBase
    {
        private readonly questionService _questionservice;
        private readonly memberService _memberservice;

        public questionController(questionService questionservice, memberService memberservice)
        {
            _questionservice = questionservice;
            _memberservice = memberservice;
        }
        #region 新增題目
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("create")]
        public async Task<IActionResult> CreatequestionAsync(addquestion create)
        {
            member createmember = _memberservice.GetDataByAccount(create.newquestion.Account);
            if (createmember != null)
            {
                if (create.Image != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetFileName(create.Image.FileName);
                    var path = Path.Combine("~/questionimg", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await create.Image.CopyToAsync(stream);
                    }
                    create.newquestion.Image = fileName;
                    _questionservice.Insertquestion(create.newquestion);
                    return Ok("新增成功");
                }
                else
                {
                    _questionservice.Insertquestion(create.newquestion);
                    return Ok("新增成功");
                }
            }
            else
            {
                return BadRequest("無此帳號");
            }
        }
        #endregion
        #region 更新題目
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("edit")]
        public IActionResult Editquestion(question edit)
        {
            question data = _questionservice.GetDataById(edit.QuestionID);
            member editmember = _memberservice.GetDataByAccount(edit.Account);
            if (editmember != null)
            {
                if (data != null)
                {
                    _questionservice.Editquestion(edit);
                    return Ok("更新成功");
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

        #region 隱藏題目
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("hide")]
        public IActionResult Hidequestion(question hide)
        {
            question data = _questionservice.GetDataById(hide.QuestionID);
            member hidemember = _memberservice.GetDataByAccount(hide.Account);
            if (hidemember != null)
            {
                if (data != null)
                {
                    _questionservice.Hidequestion(hide.QuestionID);
                    return Ok("隱藏成功");
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
        #region  透過科目年份取得試卷
        public IActionResult GetQuiz(question quiz)
        {
            List<question> datalist = _questionservice.GetQuiz(quiz);
            return Ok(datalist);
        }
        #endregion
    }
}
