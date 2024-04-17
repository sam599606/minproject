// var builder = WebApplication.CreateBuilder(args);

// //創建生成器並設置基本路徑為根目錄、添加 appsettings.json 文件
// var configurationBuilder = new ConfigurationBuilder()
//     .SetBasePath(rootDirectory)
//     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// // 創建配置對象
// var configuration = configurationBuilder.Build();

// //向服務容器中添加 MVC 控制器服務
// builder.Services.AddControllers();
// builder.Services.AddControllersWithViews();

// static IHostBuilder CreateHostBuilder(string[] args) =>
//         Host.CreateDefaultBuilder(args)
//             .ConfigureServices((hostContext, services) =>
//             {
//                 // 添加HttpContextAccessor服務
//                 services.AddHttpContextAccessor();
//             });

// //添加授權服務。默認所有用戶都必須是已驗證的使用者。
// builder.Services.AddAuthorization(options =>
// {
//     options.DefaultPolicy = new AuthorizationPolicyBuilder()
//         .RequireAuthenticatedUser()
//         .Build();
// });

// builder.Services.AddAuthentication(options =>
// {
//     //JWT身分驗證方案的預設名稱

//     //當API收到請求時，ASP.NET Core將自動使用JWT身分驗證方案對請求進行身分驗證
//     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//     //自動使用JWT身分驗證方案來回應401
//     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
// })
//                .AddJwtBearer(
//                    JwtBearerDefaults.AuthenticationScheme,
//                    options =>
//                    {
//                        //設定驗證參數
//                        options.TokenValidationParameters = new TokenValidationParameters
//                        {
//                            //驗證JWT令牌的簽章是否正確
//                            ValidateIssuerSigningKey = true,
//                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"])),
//                            ValidateIssuer = true,//驗證發行者
//                            ValidIssuer = configuration["Jwt:Issuer"],//預期的發行者
//                            ValidateAudience = true,//驗證接收者
//                            ValidAudience = configuration["Jwt:Audience"],//預期的發行者預期接收者
//                            ClockSkew = TimeSpan.Zero,//時效性
//                        };

//                        options.Events = new JwtBearerEvents
//                        {
//                            OnTokenValidated = async ctx => //觸發事件自訂邏輯
//                            {
//                                // 獲取角色
//                                var roleClaim = ctx.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
//                                if (string.IsNullOrEmpty(roleClaim))
//                                {
//                                    ctx.Fail("Unauthorized");
//                                }
//                                // 身分判斷
//                                if (roleClaim != "student" && roleClaim != "teacher" && roleClaim != "admin")
//                                {
//                                    ctx.Fail("Unauthorized");
//                                }

//                                await Task.CompletedTask;
//                            }
//                        };
//                    });


// //添加 Endpoints API Explorer 服務、Swagger 生成器服務
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();


// // 讀取數據庫連接字符串將 SqlConnection 注入
// var connectionString = configuration.GetConnectionString("DB");
// builder.Services.AddSingleton<SqlConnection>(_ => new SqlConnection(connectionString));

// // 創建使用的Service
// builder.Services.AddSingleton<memberService>();
// builder.Services.AddSingleton<lessonService>();
// builder.Services.AddSingleton<MailService>();
// builder.Services.AddSingleton<JwtService>();


// // 創建應用程式對象
// var app = builder.Build();

// // Configure the HTTP request pipeline.
// // 配置開發階段的 Swagger
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
// app.UseStaticFiles();

// //啟用靜態檔案的服務
// app.UseStaticFiles();

// // 設定CORS
// app.UseCors(builder => builder
//     .WithOrigins("http://127.0.0.1:5555", "http://127.0.0.1:5500") // 允許的源網址
//     .AllowAnyMethod() // 允許任何HTTP方法
//     .AllowAnyHeader() // 允許任何標頭
//     .AllowCredentials()); // 允許傳送身分驗證cookie


// // 啟用 HTTPS 重新導向
// app.UseHttpsRedirection();

// // 啟用路由
// app.UseRouting();

// // 啟用身份驗證和授權
// app.UseAuthorization();
// app.UseAuthentication();

// // 將請求映射到控制器
// app.MapControllers();

// // 啟動應用程式
// app.Run();
