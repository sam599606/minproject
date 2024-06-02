using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using minproject.Models;
using minproject.Models.lesson;
using minproject.ViewModels;
using OfficeOpenXml.Drawing.Chart;

namespace minproject.Services
{
    public class cartService
    {
        private readonly SqlConnection conn;
        public cartService(SqlConnection connection)
        {
            conn = connection;
        }
        #region 加入一筆訂單
        public void AddToCart(int LessonID, string Account)
        {
            string sql = @"INSERT INTO Book (Account,LessonID,IsOpen) VALUES (@Account,@LessonID,@IsOpen)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@LessonID", LessonID);
                cmd.Parameters.AddWithValue("@Account", Account);
                cmd.Parameters.AddWithValue("@IsOpen", false);
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
        #region 查一筆訂單
        public Book GetDataById(int Id)
        {
            Book Data = null;
            string sql = @"SELECT * FROM Book m inner join Lesson d on m.LessonID = d.LessonID WHERE BookID = @Id";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Data = new Book
                    {
                    BookID = Convert.ToInt32(dr["BookID"]),
                    LessonID = Convert.ToInt32(dr["LessonID"]),
                    StartTime = dr["StartTime"] != DBNull.Value ? Convert.ToDateTime(dr["StartTime"]) : DateTime.MinValue,
                    EndTime = dr["EndTime"] != DBNull.Value ? Convert.ToDateTime(dr["EndTime"]) : DateTime.MinValue
                    };
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return Data;
        }
        #endregion
        #region 刪除一筆訂單
        public void RemoveFromCart(int BookID)
        {
            string sql = "DELETE FROM Book WHERE BookID = @BookID";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@BookID", BookID);
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

        #region 下單(加入訂閱時間和結束時間)
        public void OrdersToBook(int BookId)
        {
            string sql = @"UPDATE Book SET StartTime=@StartTime,EndTime=@EndTime,IsOpen=@IsOpen WHERE BookID=@BookID";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
                cmd.Parameters.AddWithValue("@BookID", BookId);
                cmd.Parameters.AddWithValue("@StartTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@EndTime", DateTime.Now.AddYears(1).ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@IsOpen", true);
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
        #region 查詢已購買課程
        public List<Bought> GetBooks(Book order, string Account)
        {
            List<Bought> DataList = new List<Bought>();
            string sql = 
            @"SELECT Lesson.Type,Lesson.Content,Lesson.Year,Lesson.Video,Book.StartTime,Book.EndTime
            FROM Book
            inner join Lesson on Book.LessonID = Lesson.LessonID WHERE Book.IsOpen=@IsOpen and Book.Account=@Account";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@IsOpen", true);
                cmd.Parameters.AddWithValue("@Account", Account);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Bought Data = new Bought();
                    Data.StartTime = dr["StartTime"] != DBNull.Value ? (DateTime?)dr["StartTime"] : null;
                        Data.EndTime = dr["EndTime"] != DBNull.Value ? (DateTime?)dr["EndTime"] : null;
                        Data.Type = dr["Type"] != DBNull.Value ? (int?)dr["Type"] : null;
                        Data.Content = dr["Content"] != DBNull.Value ? dr["Content"].ToString() : null;
                        Data.Video = dr["Video"] != DBNull.Value ? dr["Video"].ToString() : null;
                        Data.Year = dr["Year"] != DBNull.Value ? (int?)dr["Year"] : null;
                    DataList.Add(Data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return DataList;
        }
        #endregion

        #region 獲取購物清單
        public List<Book> GetCartItems(string account)
        {
            List<Book> cartItems = new List<Book>();
            string sql = @"SELECT * FROM Book WHERE Account = @Account AND IsOpen = @IsOpen";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Account", account);
                cmd.Parameters.AddWithValue("@IsOpen", false);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Book data = new Book();
                    data.BookID = Convert.ToInt32(dr["BookID"]);
                    data.LessonID = Convert.ToInt32(dr["LessonID"]);
                    data.Account = dr["Account"].ToString();
                    data.StartTime = dr["StartTime"] != DBNull.Value ? Convert.ToDateTime(dr["StartTime"]) : (DateTime?)null;
                    data.EndTime = dr["EndTime"] != DBNull.Value ? Convert.ToDateTime(dr["EndTime"]) : (DateTime?)null;
                    data.IsOpen = Convert.ToBoolean(dr["IsOpen"]);
                    cartItems.Add(data);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return cartItems;
        }
        #endregion

    }
}