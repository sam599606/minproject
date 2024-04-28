using System.Text;
using Microsoft.Data.SqlClient;
using minproject.Models.lesson;

namespace minproject.Services.lessonService
{
    public class lessonService
    {
        private readonly SqlConnection conn;
        public lessonService(SqlConnection connection)
        {
            conn = connection;
        }
        #region 新增課程
        public void Insertlesson(lesson newlesson, string Account)
        {
            string sql = @"INSERT INTO Lesson (Account,Type,Price,Content,Video,Year,CreateTime) VALUES (@Account,@Type,@Price,@Content,@Video,@Year,@CreateTime)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Account", Account);
                cmd.Parameters.AddWithValue("@Type", newlesson.Type);
                cmd.Parameters.AddWithValue("@Price", newlesson.Price);
                cmd.Parameters.AddWithValue("@Content", newlesson.Content);
                cmd.Parameters.AddWithValue("@Video", newlesson.Video);
                cmd.Parameters.AddWithValue("@Year", newlesson.Year);
                cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
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

        #region 課程新增驗證
        public bool CheckExistingLesson(int type, int year, string account)
        {
            bool isExist = false;
            string sql = @"SELECT COUNT(*) FROM Lesson WHERE Type = @Type AND Year = @Year AND Account = @Account";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Account", account);

                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    isExist = true;
                }
            }
            catch (Exception e)
            {
                throw new Exception("檢查課程是否存在失敗：" + e.Message);
            }
            finally
            {
                conn.Close();
            }

            return isExist;
        }

        #endregion



        #region 查一筆資料
        public lesson GetDataById(int Id)
        {
            lesson Data = new lesson();
            string sql = @"SELECT * FROM Lesson m inner join Members d on m.Account = d.Account WHERE LessonID = @Id";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Data.Account = dr["Account"].ToString();
                    Data.Type = Convert.ToInt32(dr["Type"]);
                    Data.Price = Convert.ToInt32(dr["Price"]);
                    Data.Content = dr["Content"].ToString();
                    Data.Video = dr["Video"].ToString();
                    Data.Year = Convert.ToInt32(dr["Year"]);
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    if (Data.EditTime != null)
                    {
                        Data.EditTime = Convert.ToDateTime(dr["EditTime"]);
                    }
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
        #region 修改課程
        public void Editlesson(lesson Editlesson)
        {
            string sql = @"UPDATE Lesson SET Type=@Type,Price=@Price,Content=@Content,Video=@Video,Year=@Year,EditTime=@EditTime WHERE LessonID = @Id";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Type", Editlesson.Type);
                cmd.Parameters.AddWithValue("@Price", Editlesson.Price);
                cmd.Parameters.AddWithValue("@Content", Editlesson.Content);
                cmd.Parameters.AddWithValue("@Video", Editlesson.Video);
                cmd.Parameters.AddWithValue("@Year", Editlesson.Year);
                cmd.Parameters.AddWithValue("@EditTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@Id", Editlesson.LessonID);
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
        #region 刪除課程
        public void Deletelesson(int Id)
        {
            string sql = @"DELETE FROM Lesson WHERE LessonID = @Id";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", Id);
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

        #region 根據帳號獲取課程
        public List<lesson> GetLessons(string Account)
        {
            List<lesson> lessonlist = new List<lesson>();
            string sql = @"SELECT * FROM Lesson Where Account =@Account";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Account", Account);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lesson Data = new lesson();
                    Data.LessonID = Convert.ToInt32(dr["LessonID"]);
                    Data.Account = dr["Account"].ToString();
                    Data.Type = Convert.ToInt32(dr["Type"]);
                    Data.Price = Convert.ToInt32(dr["Price"]);
                    Data.Content = dr["Content"].ToString();
                    Data.Video = dr["Video"].ToString();
                    Data.Year = Convert.ToInt32(dr["Year"]);
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    if (Data.EditTime != null)
                    {
                        Data.EditTime = Convert.ToDateTime(dr["EditTime"]);
                    }
                    lessonlist.Add(Data);
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
            return lessonlist;
        }
        #endregion

        #region 重複檢查
        public bool CheckExistingLesson(int type, int year)
        {
            string sql = @"SELECT COUNT(*) FROM Lesson WHERE Type = @Type AND Year = @Year";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Type", type);
                cmd.Parameters.AddWithValue("@Year", year);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
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

        #region 獲取所有課程
        public List<lesson> GetAllLessons()
        {
            List<lesson> lessons = new List<lesson>();
            string sql = "SELECT * FROM Lesson";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lesson lessonItem = new lesson
                    {
                        LessonID = Convert.ToInt32(dr["LessonID"]),
                        Account = dr["Account"].ToString(),
                        Type = Convert.ToInt32(dr["Type"]),
                        Price = Convert.ToInt32(dr["Price"]),
                        Content = dr["Content"].ToString(),
                        Video = dr["Video"].ToString(),
                        Year = Convert.ToInt32(dr["Year"]),
                        CreateTime = Convert.ToDateTime(dr["CreateTime"]),
                        EditTime = dr["EditTime"] != DBNull.Value ? Convert.ToDateTime(dr["EditTime"]) : (DateTime?)null
                    };
                    lessons.Add(lessonItem);
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
            return lessons;
        }
        #endregion
        #region 查詢課程
        public List<lesson> Searchlesson(int? Type, int? Year)
        {
            List<lesson> lessonlist = new List<lesson>();
            string sql = @"SELECT * FROM Lesson ";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lesson Data = new lesson();
                    Data.LessonID = Convert.ToInt32(dr["LessonID"]);
                    Data.Account = dr["Account"].ToString();
                    Data.Type = Convert.ToInt32(dr["Type"]);
                    Data.Price = Convert.ToInt32(dr["Price"]);
                    Data.Content = dr["Content"].ToString();
                    Data.Video = dr["Video"].ToString();
                    Data.Year = Convert.ToInt32(dr["Year"]);
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    if (Data.EditTime != null)
                    {
                        Data.EditTime = Convert.ToDateTime(dr["EditTime"]);
                    }
                    lessonlist.Add(Data);
                }
                if (Type != null) lessonlist = lessonlist.Where(x => x.Type == Type).ToList();
                if (Year != null) lessonlist = lessonlist.Where(y => y.Year == Year).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
            finally
            {
                conn.Close();
            }
            return lessonlist;
        }
        #endregion
    }
}