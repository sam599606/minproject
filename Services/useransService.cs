using Microsoft.Data.SqlClient;
using minproject.Models.lesson;
using minproject.Models.member;
using minproject.Models.question;
using minproject.Models.userans;
using minproject.Services.memberService;
using minproject.Services.questionService;

namespace minproject.Services.useransService
{
    public class useransService
    {

        private readonly questionService.questionService _questionservice;
        private readonly SqlConnection conn;
        public useransService(SqlConnection connection, questionService.questionService questionservice)
        {
            conn = connection;
            _questionservice = questionservice;
        }
        #region 新增答案
        public void Insertans(userans newans)
        {
            string sql = @"INSERT INTO UserAnswer (QuestionID,Account,UserAnswer) VALUES (@QuestionID,@Account,@UserAnswer)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@QuestionID", newans.QuestionID);
                cmd.Parameters.AddWithValue("@Account", newans.Account);
                cmd.Parameters.AddWithValue("@UserAnswer", newans.UserAnswer);
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
        #region 查一筆資料
        public userans GetDataById(int Id)
        {
            userans Data = new userans();
            string sql = @"SELECT * FROM UserAnswer m INNER JOIN Members d ON m.Account = d.Account INNER JOIN Questions q ON m.QuestionID = q.QuestionID WHERE UserAnsID = @Id";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Data.UserAnsID = Convert.ToInt32(dr["UserAnsID"]);
                    Data.QuestionID = Convert.ToInt32(dr["QuestionID"]);
                    Data.Account = dr["Account"].ToString();
                    Data.UserAnswer = dr["UserAnswer"].ToString();

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
        #region 修改答案
        public void Editans(userans Editans)
        {
            string sql = @"UPDATE UserAnswer SET UserAnswer=@UserAnswer WHERE UserAnsId = @Id";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@UserAnswer", Editans.UserAnswer);
                cmd.Parameters.AddWithValue("@Id", Editans.UserAnsID);
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

        #region 確認使用者是否答對
        public string trueORflase(userans check)
        {
            userans data = GetDataById(check.UserAnsID);
            question question = _questionservice.GetDataById(check.QuestionID);
            if (question.Answer == data.UserAnswer)
            {
                return "答對了";
            }
            else
            {
                return "答錯了";
            }
        }
        #endregion

        #region 清空資料
        public void ClearUserAnswers()
        {
            string sql = @"DELETE FROM UserAnswer";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
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