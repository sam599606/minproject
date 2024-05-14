using Microsoft.Data.SqlClient;
using minproject.Models;
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
        public void Insertans(userans newans, string Account)
        {
            string sql = @"INSERT INTO UserAnswer (QuestionID,Account,UserAnswer) VALUES (@QuestionID,@Account,@UserAnswer)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@QuestionID", newans.QuestionID);
                cmd.Parameters.AddWithValue("@Account", Account);
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

        #region 新增答案和考試紀錄
        public void InsertExamRecord(userans newans, string Account)
        {
            ExamHistory newExam = new ExamHistory
            {
                Account = Account,
                ExamDate = DateTime.Now,
                Score = 0
            };

            // 新增考試紀錄到資料庫
            //            int examId = _examHistoryService.Insert(newExam);

            //            InsertUserAnswer(newans, examId);
        }

        private void InsertUserAnswer(userans newans, int examId)
        {
            string sql = @"INSERT INTO UserAnswer (ExamHistoryID, QuestionID, UserAnswer) VALUES (@ExamHistoryID, @QuestionID, @UserAnswer)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ExamHistoryID", examId); // 使用考試紀錄的 ID
                cmd.Parameters.AddWithValue("@QuestionID", newans.QuestionID);
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
                    //                    Data.Account = dr["Account"].ToString();
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
            string sql = @"UPDATE UserAnswer SET UserAnswer=@UserAnswer WHERE UserAnsID = @Id";
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
            question que = _questionservice.GetDataById(check.QuestionID);
            userans ans = GetDataById(check.UserAnsID);
            if (que.Answer == ans.UserAnswer)
            {
                string sql = @"UPDATE UserAnswer SET TrueorFlase=@True WHERE UserAnsID = @Id";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@True", true);
                    cmd.Parameters.AddWithValue("@Id", check.UserAnsID);
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
                return "答對";
            }
            else
            {
                string sql = @"UPDATE UserAnswer SET TrueorFlase=@Flase WHERE UserAnsID = @Id";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Flase", false);
                    cmd.Parameters.AddWithValue("@Id", check.UserAnsID);
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
                return "答錯";
            }
        }
        #endregion
        #region 使用者答案清單
        public List<userans> GetUserAns(string Account)
        {
            List<userans> datalist = new List<userans>();
            userans Data = new userans();
            string sql = @"SELECT * FROM UserAnswer Where Account =@Account";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Account", Account);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Data.UserAnsID = Convert.ToInt32(dr["UserAnsID"]);
                    Data.QuestionID = Convert.ToInt32(dr["QuestionID"]);
                    //                    Data.Account = dr["Account"].ToString();
                    Data.UserAnswer = dr["UserAnswer"].ToString();
                    Data.TrueorFlase = Convert.ToBoolean(dr["TrueorFlase"]);
                    datalist.Add(Data);
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
            return datalist;
        }
        #endregion
        #region 計算得分
        public int GetScore(string Account)
        {
            List<userans> AnsList = GetUserAns(Account);
            int trueCount = 0;
            foreach (var ans in AnsList)
            {
                if (ans.TrueorFlase == true)
                {
                    trueCount++;
                }
            }
            return trueCount;
        }
        #endregion

        // #region 清空資料
        // public void ClearUserAnswers()
        // {
        //     string sql = @"DELETE FROM UserAnswer";

        //     try
        //     {
        //         conn.Open();
        //         SqlCommand cmd = new SqlCommand(sql, conn);
        //         cmd.ExecuteNonQuery();
        //     }
        //     catch (Exception e)
        //     {
        //         throw new Exception(e.Message.ToString());
        //     }
        //     finally
        //     {
        //         conn.Close();
        //     }
        // }
        // #endregion
    }
}