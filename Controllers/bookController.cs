using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using minproject.Models;
using minproject.Models.member;
using minproject.Services;
using minproject.Services.memberService;
using minproject.ViewModels;

namespace minproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly memberService _memberservice;
        private readonly cartService _cartService;
        private readonly string account;


        public BookController(cartService cartService, memberService memberservice, IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _memberservice = memberservice;
            account = httpContextAccessor.HttpContext.User.Identity.Name;
        }

        #region 加入一筆訂單
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("AddToCart")]
        public IActionResult AddToCart(Book add)
        {
            _cartService.AddToCart(add.LessonID, this.account);
            return Ok("課程已成功加入購物車");
        }
        #endregion
        #region 刪除一筆訂單
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("RemoveFromCart")]
        public IActionResult RemoveFromCart(Book remove)
        {
            if (remove != null)
            {
                _cartService.RemoveFromCart(remove.BookID);
                return Ok("已從購物車中移除課程");
            }
            else
            {
                return BadRequest("購物車中無此課程");
            }
        }
        #endregion
        #region 下訂單
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("Order")]
        public IActionResult Order(Book order)
        {
            if (order != null)
            {
                _cartService.OrdersToBook(order.BookID);
                return Ok("已購買課程，現在可以開始上課了");
            }
            else
            {
                return BadRequest("尚未選擇課程");

            }
        }
        #endregion

        #region  查詢已購買課程
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("GetBooks")]
        public IActionResult GetBooks([FromBody] Book order)
        {
            List<Book> datalist = _cartService.GetBooks(order);
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
