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

        #region 新增考試紀錄及答案

        public void InsertExamRecord(List<userans> answers, string account, int score)
        {
            string insertExamHistorySql = @"INSERT INTO ExamHistory (Account, ExamDate, Score) VALUES (@Account, @ExamDate, @Score); SELECT SCOPE_IDENTITY();";
            string insertUserAnswerSql = @"INSERT INTO UserAnswer (ExamHistoryID, QuestionID, UserAnswer, TrueorFlase) VALUES (@ExamHistoryID, @QuestionID, @UserAnswer, @TrueorFlase)";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(insertExamHistorySql, conn);
                cmd.Parameters.AddWithValue("@Account", account);
                cmd.Parameters.AddWithValue("@ExamDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@Score", score);

                int examHistoryId = Convert.ToInt32(cmd.ExecuteScalar());

                foreach (var answer in answers)
                {
                    cmd = new SqlCommand(insertUserAnswerSql, conn);
                    cmd.Parameters.AddWithValue("@ExamHistoryID", examHistoryId);
                    cmd.Parameters.AddWithValue("@QuestionID", answer.QuestionID);
                    cmd.Parameters.AddWithValue("@UserAnswer", answer.UserAnswer);
                    cmd.Parameters.AddWithValue("@TrueorFlase", answer.TrueorFlase);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #region 查詢考試歷史

        public List<ExamHistory> GetExamHistory(string account)
        {
            List<ExamHistory> examHistoryList = new List<ExamHistory>();
            string sql = @"SELECT * FROM ExamHistory WHERE Account = @Account";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Account", account);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ExamHistory examHistory = new ExamHistory
                    {
                        ExamHistoryID = Convert.ToInt32(dr["ExamHistoryID"]),
                        Account = dr["Account"].ToString(),
                        ExamDate = Convert.ToDateTime(dr["ExamDate"]),
                        Score = Convert.ToInt32(dr["Score"])
                    };
                    examHistoryList.Add(examHistory);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                conn.Close();
            }

            return examHistoryList;
        }
        #endregion
        #region 查詢考試詳細資訊

        public List<userans> GetExamDetails(int examHistoryId)
        {
            List<userans> userAnswers = new List<userans>();
            string sql = @"SELECT * FROM UserAnswer WHERE ExamHistoryID = @ExamHistoryID";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ExamHistoryID", examHistoryId);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    userans answer = new userans
                    {
                        UserAnsID = Convert.ToInt32(dr["UserAnsID"]),
                        ExamHistoryID = Convert.ToInt32(dr["ExamHistoryID"]),
                        QuestionID = Convert.ToInt32(dr["QuestionID"]),
                        UserAnswer = dr["UserAnswer"].ToString(),
                        TrueorFlase = Convert.ToBoolean(dr["TrueorFlase"])
                    };
                    userAnswers.Add(answer);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                conn.Close();
            }

            return userAnswers;
        }
        #endregion
    }
}