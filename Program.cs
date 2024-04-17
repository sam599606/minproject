using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using minproject.Services.JwtService;
using minproject.Services.lessonService;
using minproject.Services.MailService;
using minproject.Services.memberService;
using minproject.Services.questionService;
using minproject.Services.useransService;

//獲取根目錄路徑(包含應用程式執行檔案的目錄)
var rootDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
Console.WriteLine($"Root Directory: {rootDirectory}");

//配置 ASP.NET Core 應用程式
var builder = WebApplication.CreateBuilder(args);

// 創建生成器並設置基本路徑為根目錄、添加 appsettings.json 文件
var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(rootDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// 創建配置對象
var configuration = configurationBuilder.Build();



// 向服務容器中添加 MVC 控制器服務
builder.Services.AddControllers();

// 向服務容器中添加HttpContextAccessor服務
builder.Services.AddHttpContextAccessor();

// 添加授權服務。默認所有用戶都必須是已驗證的使用者。
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});





builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //驗證JWT令牌的簽章是否正確
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),
        ValidateIssuer = true,//驗證發行者
        ValidIssuer = configuration["Jwt:Issuer"],//預期的發行者
        ValidateAudience = true,//驗證接收者
        ValidAudience = configuration["Jwt:Audience"],//預期的發行者預期接收者
        ClockSkew = TimeSpan.Zero,//時效性
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async ctx => //觸發事件自訂邏輯
        {
            // 獲取角色
            var roleClaim = ctx.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleClaim))
            {
                ctx.Fail("Unauthorized");
            }
            // 身分判斷
            if (roleClaim != "student" && roleClaim != "teacher" && roleClaim != "admin")
            {
                ctx.Fail("Unauthorized");
            }

            await Task.CompletedTask;
        }
    };
});

builder.Services.AddMvc(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy =>
    {
        policy.RequireRole("admin");
    });
    options.AddPolicy("student", policy =>
    {
        policy.RequireRole("student");
    });
    options.AddPolicy("teacher", policy =>
    {
        policy.RequireRole("teacher");
    });
});

// 添加 Endpoints API Explorer 服務、Swagger 生成器服務
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 讀取數據庫連接字符串並將 SqlConnection 注入
var connectionString = configuration.GetConnectionString("DB");
builder.Services.AddSingleton<SqlConnection>(_ => new SqlConnection(connectionString));


// 創建使用的Service
builder.Services.AddSingleton<memberService>();
builder.Services.AddSingleton<lessonService>();
builder.Services.AddSingleton<MailService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<questionService>();
builder.Services.AddSingleton<useransService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

// 設定CORS
app.UseCors(builder => builder
    .WithOrigins("http://127.0.0.1:5555", "http://127.0.0.1:5500") // 允許的源網址
    .AllowAnyMethod() // 允許任何HTTP方法
    .AllowAnyHeader() // 允許任何標頭
    .AllowCredentials()); // 允許傳送身分驗證cookie


app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();//新增


app.MapControllers();

app.Run();


