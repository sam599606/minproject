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

        public lessonController(lessonService lessonservice, memberService memberservice)
        {
            _lessonservice = lessonservice;
            _memberservice = memberservice;
        }
        #region 新增課程
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("create")]
        public IActionResult Createlesson(lesson create)
        {
            member createmember = _memberservice.GetDataByAccount(create.Account);
            if (createmember != null)
            {
                _lessonservice.Insertlesson(create);
                return Ok("新增成功");
            }
            else
            {
                return BadRequest();
            }
        }
        #endregion
        #region 更新課程
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("edit")]
        public IActionResult Editlesson(lesson edit)
        {
            lesson data = _lessonservice.GetDataById(edit.LessonID);
            member editmember = _memberservice.GetDataByAccount(edit.Account);
            if (editmember != null)
            {
                if (data != null)
                {
                    _lessonservice.Editlesson(edit);
                    return Ok("更新成功");
                }
                else
                {
                    return BadRequest("查無課程");
                }
            }
            else
            {
                return BadRequest("無此帳號");
            }
        }
        #endregion

        #region 刪除課程
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "teacher")]
        [HttpPost("delete")]
        public IActionResult Deletelesson(lesson delete)
        {
            lesson data = _lessonservice.GetDataById(delete.LessonID);
            member deletemember = _memberservice.GetDataByAccount(delete.Account);
            if (deletemember != null)
            {
                if (data != null)
                {
                    _lessonservice.Deletelesson(delete.LessonID);
                    return Ok("刪除成功");
                }
                else
                {
                    return BadRequest("無此課程");
                }
            }
            else
            {
                return BadRequest("無此帳號");
            }
        }
        #endregion
    }
}
