using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using minproject.ViewModels;

namespace minproject.Services
{
    public class cartService
    {
        private readonly SqlConnection conn;
        public cartService(SqlConnection connection)
        {
            conn = connection;
        }
        #region 加入購物車
        public bool AddToCart(int LessonID)
        {
            string sql = @"INSERT INTO Book (LessonID) VALUES (@LessonID)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@LessonID", LessonID);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 從購物車中刪除
        public bool RemoveFromCart(int BookID)
        {
            string sql = "DELETE FROM Book WHERE BookID = @BookID";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@BookID", BookID);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 下單
        public void AddOrdersToBookSave(string account)
        {
            string sql = @"
    INSERT INTO BookSave (BookID, Account)
    SELECT BookID, @Account
    FROM Book 
    WHERE EndTime IS NULL
";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Account", account);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }


        public void PlaceOrder()
        {
            string sql = @"UPDATE Book SET StartTime = @StartTime, EndTime = @EndTime WHERE EndTime IS NULL";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@StartTime", DateTime.Now);
                cmd.Parameters.AddWithValue("@EndTime", DateTime.Now.AddYears(1)); // 假設訂閱一年後到期
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
        }

        #endregion
    }
}