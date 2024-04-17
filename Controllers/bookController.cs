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

        #region 加入購物車
        [AllowAnonymous]
        [HttpPost("AddToCart")]
        public IActionResult AddToCart(int LessonID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool result = _cartService.AddToCart(LessonID);

            if (result)
            {
                return Ok("課程已成功加入購物車");
            }
            else
            {
                return BadRequest("無法將課程添加到購物車");
            }
        }
        #endregion

        #region 從購物車中取出
        [AllowAnonymous]
        [HttpPost("RemoveFromCart")]
        public IActionResult RemoveFromCart(int BookID)
        {
            bool result = _cartService.RemoveFromCart(BookID);

            if (result)
            {
                return Ok("項目已從購物車中移除");
            }
            else
            {
                return BadRequest("無法從購物車中移除項目");
            }
        }
        #endregion

        #region 下訂單
        [AllowAnonymous]
        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder(string account)
        {
            try
            {
                // 先將所有 Book 資料表中 EndTime 不為 Null 的資料寫入 BookSave 資料表中
                _cartService.AddOrdersToBookSave(account);

                // 接著進行訂單的寫入，即寫入 Book 資料表中的 StartTime 和 EndTime
                _cartService.PlaceOrder();

                return Ok("訂單已成功下單並處理");
            }
            catch (Exception e)
            {
                return BadRequest($"訂單處理失敗: {e.Message}");
            }
            #endregion
        }
    }
}
