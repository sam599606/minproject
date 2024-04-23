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
        public void AddToCart(int LessonID)
        {
            string sql = @"INSERT INTO Book (LessonID) VALUES (@LessonID)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@LessonID", LessonID);
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

        #region 下單
        public void OrdersToBookSave(Booksave order)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"INSERT INTO BookSave (BookID, Account) VALUES (@BookID, @Account)");
            sql.AppendLine(@"UPDATE Book SET StartTime=@StartTime,EndTime=@EndTime WHERE BookID=@BookID");
            //string sql = @"INSERT INTO BookSave (BookID, Account) SELECT BookID, @Account FROM Book  WHERE EndTime IS NULL";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
                cmd.Parameters.AddWithValue("@BookID", order.BookID);
                cmd.Parameters.AddWithValue("@Account", order.Account);
                // 下單時間
                cmd.Parameters.AddWithValue("@StartTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                // 下單時間加一年
                cmd.Parameters.AddWithValue("@EndTime", DateTime.Now.AddYears(1).ToString("yyyy-MM-dd HH:mm:ss"));
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

        // #region 查詢所有已購買課程
        // public List<Booksave> GetBooks(int Id)
        // {
        //     List<Booksave> DataList = new List<Booksave>();
        //     string sql = @"SELECT * FROM BookSave m inner join Members d on m.Account = d.Account WHERE BookID =@BookID";
        //     try
        //     {
        //         Booksave Data = new Booksave();
        //         conn.Open();
        //         SqlCommand cmd = new SqlCommand(sql, conn);
        //         cmd.Parameters.AddWithValue("@BookID",Id);
        //         SqlDataReader dr = cmd.ExecuteReader();
        //         while (dr.Read())
        //         {
        //             Data.BookID = Convert.ToInt32(dr["BookID"]);
        //             Data.Account = dr["Account"].ToString();
        //             Data.book.LessonID = Convert.ToInt32(dr["LessonID"]);
        //             Data.book.StartTime = Convert.ToDateTime(dr["StartTime"]);
        //             Data.book.EndTime = Convert.ToDateTime(dr["EndTime"]);
        //             DataList.Add(Data);
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         throw new Exception(e.Message.ToString());
        //     }
        //     finally
        //     {
        //         conn.Close();
        //     }
        //     return DataList;
        // }
        // #endregion
    }
}