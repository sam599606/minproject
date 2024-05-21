using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minproject.Models.member;
using minproject.Models.question;
using minproject.Services.memberService;
using minproject.Services.questionService;
using minproject.ViewModels;
using minproject.ViewModels.editquestion;

namespace minproject.Controllers.questionController
{
    [ApiController]
    [Route("api/[controller]")]
    public class questionController : ControllerBase
    {
        private readonly questionService _questionservice;
        private readonly memberService _memberservice;
        private readonly string account;

        public questionController(questionService questionservice, memberService memberservice, IHttpContextAccessor httpContextAccessor)
        {
            _questionservice = questionservice;
            _memberservice = memberservice;
            account = httpContextAccessor.HttpContext.User.Identity.Name;
        }
        #region 新增題目
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("import")]
        public IActionResult ImportQuestion([FromForm] IFormFile file)
        {
            if (file != null || file.Length != 0)
            {
                if (Path.GetExtension(file.FileName).ToLower() == ".xlsx")
                {
                    var filePath = Path.GetTempFileName();

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var questions = _questionservice.ImporQuestionFromExcel(filePath);
                    _questionservice.Insertquestion(this.account, questions);

                    return Ok("成功上傳");

                }
                else
                {
                    return BadRequest("請上傳 Excel 檔案");
                }
            }
            else
            {
                return BadRequest("沒有上傳檔案");
            }
        }
        #endregion
        #region 更新題目
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("edit")]
        public async Task<IActionResult> Editquestion([FromBody] editquestion edit)
        {
            Console.Write("rgsreg");
            Console.WriteLine(edit);
            question data = _questionservice.GetDataById(edit.question.QuestionID);
            if (data != null)
            {
                if (edit.Image != null)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetFileName(edit.Image.FileName);
                    var path = Path.Combine("~/questionimg", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await edit.Image.CopyToAsync(stream);
                    }
                    edit.question.Image = fileName;
                    _questionservice.Editquestion(edit.question);
                    return Ok("更新成功");
                }
                else
                {
                    _questionservice.Editquestion(edit.question);
                    return Ok("更新成功");
                }
            }
            else
            {
                return BadRequest("無此題目");
            }
        }
        #endregion
        // #region 更新題目
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        // [HttpPost("edit")]
        // public IActionResult Editquestion(question edit)
        // {
        //     if (edit != null)
        //     {
        //         _questionservice.Editquestion(edit);
        //         return Ok("更新成功");
        //     }
        //     else
        //     {
        //         return BadRequest("無此題目");
        //     }
        // }
        // #endregion
        #region 隱藏題目
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("hide")]
        public IActionResult Hidequestion(question hide)
        {
            question data = _questionservice.GetDataById(hide.QuestionID);
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
        #endregion
        #region  透過科目年份取得試卷
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Quiz")]
        public IActionResult GetQuiz([FromBody] question quiz)
        {
            List<question> datalist = _questionservice.GetQuiz(quiz);
            if (datalist != null)
            {
                return Ok(datalist);
            }
            else
            {
                return BadRequest("沒有題目");
            }
        }
        #endregion

        #region  取得全部試卷
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("AllQuiz")]
        public IActionResult GetAllQuiz([FromBody] question quiz)
        {
            List<question> datalist = _questionservice.GetAllQuiz(quiz);
            if (datalist != null)
            {
                return Ok(datalist);
            }
            else
            {
                return BadRequest("沒有題目");
            }
        }
        #endregion

    }
}