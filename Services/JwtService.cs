using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace minproject.Services.JwtService
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JwtService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _config = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        #region 產生 JWT token
        public string GenerateJwtToken(string Account, string Role)
        {
            var secretKey = _config["Jwt:SecretKey"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireTimeInMinutes = int.Parse(_config["Jwt:ExpireTimeInMinutes"]);

            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, Account),
                new Claim(ClaimTypes.Role, Role)
            });


            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            // var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims.Claims,
            expires: DateTime.UtcNow.AddMinutes(expireTimeInMinutes),
            signingCredentials: credentials
        );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
        #region 解析 JWT token
        public ClaimsPrincipal ValidateJwtToken(string token)
        {
            try
            {
                var secretKey = _config["Jwt:SecretKey"];
                var issuer = _config["Jwt:Issuer"];
                var audience = _config["Jwt:Audience"];

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuerSigningKey = true
                };

                SecurityToken validatedToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                // 將 ClaimsPrincipal 對象分配給 IHttpContextAccessor 實例中的 HttpContext.User 屬性
                _httpContextAccessor.HttpContext.User = principal;
                return principal;

            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}