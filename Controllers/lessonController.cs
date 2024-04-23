using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using minproject.Models.lesson;
using minproject.Models.member;
using minproject.Services.lessonService;
using minproject.Services.memberService;

namespace minproject.Controllers.lessonController
{
    [ApiController]
    [Route("api/[controller]")]
    public class lessonController : ControllerBase
    {
        private readonly lessonService _lessonservice;
        private readonly memberService _memberservice;
        private readonly string account;

        public lessonController(lessonService lessonservice, memberService memberservice, IHttpContextAccessor httpContextAccessor)
        {
            _lessonservice = lessonservice;
            _memberservice = memberservice;
            account = httpContextAccessor.HttpContext.User.Identity.Name;

        }
        #region 新增課程
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("create")]
        public IActionResult Createlesson(lesson create)
        {
            _lessonservice.Insertlesson(create);
            return Ok("新增成功");
        }
        #endregion
        #region 更新課程
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("edit")]
        public IActionResult Editlesson(lesson edit)
        {
            if (edit != null)
            {
                _lessonservice.Editlesson(edit);
                return Ok("更新成功");
            }
            else
            {
                return BadRequest("查無課程");
            }
        }
        #endregion

        #region 刪除課程
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("delete")]
        public IActionResult Deletelesson(lesson delete)
        {
            if (delete != null)
            {
                _lessonservice.Deletelesson(delete.LessonID);
                return Ok("刪除成功");
            }
            else
            {
                return BadRequest("無此課程");
            }
        }
        #endregion

        #region 獲取所有課程
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("GetLesson")]
        public IActionResult GetLessons()
        {
            var lessons = _lessonservice.GetLessons(this.account);
            if (lessons != null && lessons.Any())
            {
                return Ok(lessons);
            }
            else
            {
                return NotFound("沒有課程");
            }
        }
        #endregion

    }
}
