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
            bool isExist = _lessonservice.CheckExistingLesson(create.Type ?? 0, create.Year ?? 0, this.account);
            if (isExist)
            {
                return BadRequest("已存在相同科目、年份和帳號的課程，無法新增。");
            }

            _lessonservice.Insertlesson(create, this.account);
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
                bool isExist = _lessonservice.CheckExistingLesson(edit.Type ?? 0, edit.Year ?? 0, this.account);
                if (isExist)
                {
                    return BadRequest("已存在相同科目、年份和帳號的課程，無法新增。");
                }

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

        #region 根據帳號獲取課程
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("GetLesson")]
        public IActionResult GetLessons()
        {
            List<lesson> datalist = _lessonservice.GetLessons(this.account);
            if (datalist != null)
            {
                return Ok(datalist);
            }
            else
            {
                return NotFound("沒有課程");
            }
        }
        #endregion

        #region 獲取所有課程
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("GetAllLessons")]
        public IActionResult GetAllLessons()
        {
            var lessons = _lessonservice.GetAllLessons();
            if (lessons != null && lessons.Any())
            {
                return Ok(lessons);
            }
            else
            {
                return NotFound("找不到任何課程");
            }
        }
        #endregion

    }
}
