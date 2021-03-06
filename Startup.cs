
using SeedApi.Data;
using SeedApi.Helpers;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SeedApi.Services;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SeedApi {
    public class Startup {
        public Startup(IConfiguration configuration, IWebHostEnvironment env) {
            Environment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.Configure<WebEncoderOptions>(options => {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All); // 한글이 인코딩되는 문제 해결
            });

            services.Configure<ApiBehaviorOptions>(options => {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddCors();

            // 서비스들 추가
            services.AddScoped<IssueEmployeeService>();
            services.AddScoped<IssueCategoryService>();

            services.AddControllers(options =>
                options.Filters.Add(new HttpResponseExceptionFilter())
            ).AddJsonOptions(options => { // JSON 카멜케이스 - DictionaryKeyPolicy (ModelState Error)는 반영안됨?
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            });

            if (Environment.IsDevelopment()) {
                var sever = Configuration.GetConnectionString("Server");
                if (sever != null && sever == "Sqlite") {
                    services.AddDbContext<SeedApiContext>(
                        options => options.UseLazyLoadingProxies().UseSqlite(Configuration.GetConnectionString("SqliteContext")));
                } else {
                    services.AddDbContext<SeedApiContext>(
                        options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("SqlServerContext")));
                }
            } else {
                services.AddDbContext<SeedApiContext>(
                    options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("SqlServerContext")));
            }

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory) {
            ApplicationLogging.ConfigureLogger(loggerFactory);

            Util._logger = ApplicationLogging.CreateLogger("Util");
            EntityHelper._logger = ApplicationLogging.CreateLogger("EntityHelper");

            if (env.IsDevelopment()) {
                app.UseExceptionHandler("/error-local-development");
            } else {
                app.UseExceptionHandler("/error");
            }

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}