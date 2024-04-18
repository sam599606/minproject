using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using minproject.Models.member;
using minproject.ViewModels.changepasswordModel;

namespace minproject.Services.memberService
{
    public class memberService
    {
        private readonly SqlConnection conn;
        public memberService(SqlConnection connection)
        {
            conn = connection;
        }
        #region 註冊方法
        public void Register(member newmember)
        {
            newmember.Password = HashPassword(newmember.Password);
            string sql = @"INSERT INTO Members(Account,Password,Email,AuthCode,Role,IsDelete) VALUES (@Account,@Password,@Email,@AuthCode,@Role,@IsDelete)";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Account", newmember.Account);
                cmd.Parameters.AddWithValue("@Password", newmember.Password);
                cmd.Parameters.AddWithValue("@Email", newmember.Email);
                cmd.Parameters.AddWithValue("@Role", newmember.Role);
                cmd.Parameters.AddWithValue("@IsDelete", 0);
                cmd.Parameters.AddWithValue("@AuthCode", newmember.AuthCode);
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

        #region 更新用户数据
        public string UpdateUserData(member updatedMember)
        {
            member existingMember = GetDataByAccount(updatedMember.Account);

            if (existingMember != null)
            {
                string sql = @"UPDATE Members SET Account = @Account, Password = @Password, Email = @Email, Role = @Role, IsDelete = @IsDelete, AuthCode = @Authcode WHERE Account = @Account";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Account", existingMember.Account);
                    cmd.Parameters.AddWithValue("@Password", existingMember.Password);
                    cmd.Parameters.AddWithValue("@Email", existingMember.Email);
                    cmd.Parameters.AddWithValue("@Role", existingMember.Role);
                    cmd.Parameters.AddWithValue("@IsDelete", existingMember.IsDelete);
                    cmd.Parameters.AddWithValue("@AuthCode", existingMember.AuthCode);
                    cmd.ExecuteNonQuery();
                    return "";
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
            else
            {
                return "找不到要更新的使用者记录";
            }
        }
        #endregion

        #region 給密碼
        public string GetPassowrd()
        {
            string[] Code ={"A", "B", "C", "D", "E", "F", "G", "H", "I","J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                "a", "b", "c", "d", "e", "f", "g", "h", "i","j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" ,
                 "1", "2", "3", "4", "5", "6", "7", "8", "9"};
            string PasswordCode = string.Empty;
            Random rd = new Random();
            for (int i = 0; i < 10; i++)
            {
                PasswordCode += Code[rd.Next(Code.Count())];
            }
            return PasswordCode;
        }
        #endregion
        #region Hash密碼
        public string HashPassword(string Password)
        {
            string salt = "kdsnkvnakeav123";
            string saltandpassword = String.Concat(salt, Password);
            SHA256 sha256 = new SHA256Managed();
            byte[] PasswordData = Encoding.Default.GetBytes(saltandpassword);
            byte[] HashData = sha256.ComputeHash(PasswordData);
            string result = Convert.ToBase64String(HashData);
            return result;
        }
        #endregion
        #region 查一筆資料
        public member GetDataByAccount(string Account)
        {
            member Data = new member();
            string sql = @"SELECT * FROM Members WHERE Account = @Account";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Account", Account);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                Data.Account = dr["Account"].ToString();
                Data.Password = dr["Password"].ToString();
                Data.Email = dr["Email"].ToString();
                Data.Role = dr["Role"].ToString();
                Data.IsDelete = Convert.ToBoolean(dr["IsDelete"]);
                dr.Close();
            }
            catch
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

        public void UpdateAuthCode(string Account)
        {
            string sql = @"UPDATE Members SET AuthCode = null WHERE Account = @Account";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Account", Account);
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


        #region 帳號重複
        public bool repectAccount(string Account)
        {
            member checkmember = GetDataByAccount(Account);
            return (checkmember == null);
        }
        #endregion
        #region  登入
        public string Logincheck(string Account, string Password)
        {
            member Loginmember = GetDataByAccount(Account);
            if (Loginmember != null)
            {
                if (PasswordCheck(Loginmember, Password))
                {
                    return "";
                }
                else
                {
                    return "密碼輸入錯誤。";
                }
            }
            else
            {
                return "無此帳號，請重新確認或註冊。";
            }
        }
        #endregion
        #region 密碼確認
        public bool PasswordCheck(member checkmember, string Password)
        {
            bool result = checkmember.Password.Equals(HashPassword(Password));
            return result;
        }
        #endregion
        #region 更改密碼
        public string ChangePassword(string Account, string Password, string newPassword)
        {
            member changemember = GetDataByAccount(Account);
            if (PasswordCheck(changemember, Password))
            {
                changemember.Password = HashPassword(newPassword);
                string sql = @"UPDATE Members SET Password = @Password WHERE Account = @Account";
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Password", changemember.Password);
                    cmd.Parameters.AddWithValue("@Account", Account);
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
                return "";
            }
            else
            {
                return "舊密碼輸入錯誤";
            }
        }
        #endregion
        #region 忘記密碼
        public string ForgetPassword(member forgrtmember, string Email, string Password)
        {
            string HashData = HashPassword(Password);
            if (forgrtmember != null)
            {
                if (Email == forgrtmember.Email)
                {
                    string sql = @"UPDATE Members SET Password = @Password WHERE Account = @Account";
                    try
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@Password", HashData);
                        cmd.Parameters.AddWithValue("@Account", forgrtmember.Account);
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
                    return "";
                }
                else
                {
                    return "輸入的信箱與註冊的信箱不相符。";
                }
            }
            else
            {
                return "無此帳號，請重新確認或註冊。";
            }

        }
        #endregion
    }
}
