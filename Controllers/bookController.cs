using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using minproject.Models;
using minproject.Services;
using minproject.ViewModels;

namespace minproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly cartService _cartService;

        public BookController(cartService cartService)
        {
            _cartService = cartService;
        }

        #region 加入一筆訂單
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "student")]
        [HttpPost("AddToCart")]
        public IActionResult AddToCart(Book add)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                _cartService.AddToCart(add.LessonID);
                return Ok("課程已成功加入購物車");
            }
        }
        #endregion
        #region 刪除一筆訂單
        [AllowAnonymous]
        [HttpPost("RemoveFromCart")]
        public IActionResult RemoveFromCart(Book remove)
        {
            if (remove.BookID != null)
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
        [AllowAnonymous]
        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder(Booksave order)
        {
            try
            {
                _cartService.OrdersToBookSave(order);
                return Ok("已購買課程，現在可以開始上課了");
            }
            catch (Exception e)
            {
                return BadRequest("購買失敗，請重新下單");
            }
        }
        #endregion

        #region 查詢購物車內容

        #endregion
    }
}
