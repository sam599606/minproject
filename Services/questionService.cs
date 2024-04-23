using Microsoft.Data.SqlClient;
using minproject.Models.question;
using minproject.Models.userans;
using OfficeOpenXml;

namespace minproject.Services.questionService
{
    public class questionService
    {
        private readonly SqlConnection conn;
        public questionService(SqlConnection connection)
        {
            conn = connection;
        }
        #region excel
        public List<question> ImporQuestionFromExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            List<question> questions = new List<question>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assuming your data is in the first worksheet

                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    var question = new question
                    {
                        Type = int.Parse(worksheet.Cells[row, 1].Value.ToString()),
                        QuestionNum = int.Parse(worksheet.Cells[row, 2].Value.ToString()),
                        Content = worksheet.Cells[row, 3].Value.ToString(),
                        Image = worksheet.Cells[row, 4].Value.ToString(),
                        Answer = worksheet.Cells[row, 5].Value.ToString(),
                        Solution = worksheet.Cells[row, 6].Value.ToString(),
                        Year = int.Parse(worksheet.Cells[row, 7].Value.ToString())
                    };

                    questions.Add(question);
                }
            }
            return questions;
        }
        #endregion
        #region 新增題目
        public void Insertquestion(string Account,List<question> newquestion)
        {
            string sql = @"INSERT INTO Questions (Account,Type,QuestionNum,Content,Image,Answer,Solution,Year,CreateTime,IsDelete) VALUES (@Account,@Type,@QuestionNum,@Content,@Image,@Answer,@Solution,@Year,@CreateTime,@IsDelete)";

            foreach (var data in newquestion)
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Account", Account);
                    cmd.Parameters.AddWithValue("@Type", data.Type);
                    cmd.Parameters.AddWithValue("@QuestionNum", data.QuestionNum);
                    cmd.Parameters.AddWithValue("@Content", data.Content);
                    cmd.Parameters.AddWithValue("@Image", data.Image);
                    cmd.Parameters.AddWithValue("@Answer", data.Answer);
                    cmd.Parameters.AddWithValue("@Solution", data.Solution);
                    cmd.Parameters.AddWithValue("@Year", data.Year);
                    cmd.Parameters.AddWithValue("@CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@IsDelete", '0');
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


        }
        #endregion
        #region 查一筆資料
        public question GetDataById(int Id)
        {
            question Data = new question();
            string sql = @"SELECT * FROM Questions m inner join Members d on m.Account = d.Account WHERE QuestionID = @Id";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Data.QuestionID = Convert.ToInt32(dr["QuestionID"]);
                    Data.Account = dr["Account"].ToString();
                    Data.Type = Convert.ToInt32(dr["Type"]);
                    Data.QuestionNum = Convert.ToInt32(dr["QuestionNum"]);
                    Data.Content = dr["Content"].ToString();
                    Data.Image = dr["Image"].ToString();
                    Data.Answer = dr["Answer"].ToString();
                    Data.Solution = dr["Solution"].ToString();
                    Data.Year = Convert.ToInt32(dr["Year"]);
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    if (Data.EditTime != null)
                    {
                        Data.EditTime = Convert.ToDateTime(dr["EditTime"]);
                    }
                    Data.IsDelete = Convert.ToBoolean(dr["IsDelete"]);
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
        #region 修改題目
        public void Editquestion(question Editquestion)
        {
            string sql = @"UPDATE Questions SET Account=@Account,QuestionNum=@QuestionNum,Type=@Type,Content=@Content,Image=@Image,Answer=@Answer,Solution=@Solution,Year=@Year,EditTime=@EditTime WHERE QuestionID = @Id";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Account", Editquestion.Account);
                cmd.Parameters.AddWithValue("@Type", Editquestion.Type);
                cmd.Parameters.AddWithValue("@QuestionNum", Editquestion.QuestionNum);
                cmd.Parameters.AddWithValue("@Content", Editquestion.Content);
                cmd.Parameters.AddWithValue("@Image", Editquestion.Image);
                cmd.Parameters.AddWithValue("@Answer", Editquestion.Answer);
                cmd.Parameters.AddWithValue("@Solution", Editquestion.Solution);
                cmd.Parameters.AddWithValue("@Year", Editquestion.Year);
                cmd.Parameters.AddWithValue("@EditTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@IsDelete", Editquestion.IsDelete);
                cmd.Parameters.AddWithValue("@Id", Editquestion.QuestionID);
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
        #region 隱藏題目
        public void Hidequestion(int Id)
        {
            string sql = @"UPDATE Questions SET IsDelete=@IsDelete WHERE QuestionID = @Id";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@IsDelete", '1');
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
        #region 透過科目年分取得試卷
        public List<question> GetQuiz(question quiz)
        {
            List<question> DataList = new List<question>();
            string sql = @"SELECT * FROM Questions WHERE Type = @Type and Year = @Year and IsDelete=@IsDelete";
            question Data = new question();
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Type", quiz.Type);
                cmd.Parameters.AddWithValue("@Year", quiz.Year);
                cmd.Parameters.AddWithValue("@IsDelete", false);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Data.QuestionID = Convert.ToInt32(dr["QuestionID"]);
                    Data.Account = dr["Account"].ToString();
                    Data.Type = Convert.ToInt32(dr["Type"]);
                    Data.QuestionNum = Convert.ToInt32(dr["QuestionNum"]);
                    Data.Content = dr["Content"].ToString();
                    Data.Image = dr["Image"].ToString();
                    Data.Answer = dr["Answer"].ToString();
                    Data.Solution = dr["Solution"].ToString();
                    Data.Year = Convert.ToInt32(dr["Year"]);
                    Data.CreateTime = Convert.ToDateTime(dr["CreateTime"]);
                    if (Data.EditTime != null)
                    {
                        Data.EditTime = Convert.ToDateTime(dr["EditTime"]);
                    }
                    Data.IsDelete = Convert.ToBoolean(dr["IsDelete"]);
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