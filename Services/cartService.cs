using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using minproject.Models;
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
            Book Data = new Book();
            string sql = @"SELECT * FROM Book m inner join Lesson d on m.LessonID = d.LessonID WHERE BookID = @Id";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Data.BookID = Convert.ToInt32(dr["BookID"]);
                    Data.LessonID = Convert.ToInt32(dr["LessonID"]);
                    Data.StartTime = Convert.ToDateTime(dr["StartTime"]);
                    Data.EndTime = Convert.ToDateTime(dr["EndTime"]);
                }
            }
            catch (Exception e)
            {
                Data = null;
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
        public List<Book> GetBooks(Book order, string Account)
        {
            List<Book> DataList = new List<Book>();
            string sql = @"SELECT * FROM Book WHERE IsOpen=@IsOpen and Account=@Account";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@IsOpen", true);
                cmd.Parameters.AddWithValue("@Account", Account);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Book Data = new Book();
                    Data.BookID = Convert.ToInt32(dr["BookID"]);
                    Data.Account = dr["Account"].ToString();
                    Data.LessonID = Convert.ToInt32(dr["LessonID"]);
                    Data.StartTime = Convert.ToDateTime(dr["StartTime"]);
                    Data.EndTime = Convert.ToDateTime(dr["EndTime"]);
                    Data.IsOpen = Convert.ToBoolean(dr["IsOpen"]);
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
    }
}