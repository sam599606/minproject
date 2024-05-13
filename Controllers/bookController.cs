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
            Book data = _cartService.GetDataById(remove.BookID);
            if (data == null)
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
            List<Book> datalist = _cartService.GetBooks(order, this.account);
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

        #region 查詢購物車內物品（帶分頁）
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("GetCart")]
        public IActionResult GetCart(int NowPage = 1, int ItemNum = 5)
        {
            try
            {
                List<Book> cartItems = _cartService.GetCartItems(this.account);
                cartItems = cartItems.Where(item => item.EndTime == null).ToList();

                int totalItems = cartItems.Count;
                int MaxPage = (int)Math.Ceiling((double)totalItems / ItemNum);

                List<Book> currentPageItems = cartItems.Skip((NowPage - 1) * ItemNum).Take(ItemNum).ToList();

                PaginationInfo paginationInfo = new PaginationInfo
                {
                    NowPage = NowPage,
                    TotalItems = totalItems,
                    ItemNum = ItemNum,
                    TotalPages = MaxPage
                };

                // 返回包含分頁資訊的結果
                return Ok(new { CartItems = currentPageItems, PaginationInfo = paginationInfo });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "發生錯誤：" + ex.Message);
            }
        }
        #endregion



    }
}
