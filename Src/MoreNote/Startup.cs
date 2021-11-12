﻿using Autofac;
using Masuit.Tools.Core.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Morenote.Framework.Filter.Global;
using MoreNote.Logic.DB;
using MoreNote.Logic.Entity.ConfigFile;
using MoreNote.Logic.Service;
using MoreNote.Logic.Service.PasswordSecurity;
using MoreNote.Logic.Service.Segmenter;
using System;

namespace MoreNote
{
    public class Startup
    {
        private WebSiteConfig config;
        private readonly IWebHostEnvironment _env;

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this._env = env;
            ConfigFileService configFileService = new ConfigFileService();
            config = configFileService.WebConfig;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;//关闭GDPR规范
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;

            });

            //解决Multipart body length limit 134217728 exceeded
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            //随机图片API初始化程序
            if (config != null && config.Spiders != null && config.Spiders.CanCrawlerWorker)
            {
                services.AddHostedService<MoreNoteWorkerService.RandomImagesCrawlerWorker>();
            }
            if (config != null && config.PublicAPI != null && config.PublicAPI.CanRandomImageAPI)
            {
                services.AddHostedService<MoreNoteWorkerService.UpdataImageURLWorker>();
                //网络分析和权重
                //services.AddHostedService<MoreNoteWorkerService.AnalysisOfNetwork>();
            }

            //增加数据库
            var connection = config.PostgreSql.Connection;
            services.AddEntityFrameworkNpgsql();
            services.AddDbContextPool<DataContext>((serviceProvider, optionsBuilder) =>
            {
                optionsBuilder.UseNpgsql(connection, b => b.MigrationsAssembly("MoreNote.Logic"));
                optionsBuilder.UseInternalServiceProvider(serviceProvider);
            });
            // services.AddDbContextPool<CarModelContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SQL")));
            //使用分布式内存
            services.AddDistributedMemoryCache();
            ////使用Redis分布式缓存
            //services.AddDistributedRedisCache(options =>
            //{
            //    options.Configuration = "localhost";
            //});
            //增加Session
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.Cookie.Name = "SessionID";
                options.IdleTimeout = TimeSpan.FromDays(30);//过期时间
                options.Cookie.HttpOnly = true;//设为HttpOnly 阻止js脚本读取
                options.Cookie.Domain = config.APPConfig.Domain;//
                options.Cookie.SameSite = SameSiteMode.Lax;//
            });
            //这样可以将HttpContext注入到控制器中。
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddAuthentication("CookieAuthentication")
            //                .AddCookie("CookieAuthentication", config =>
            //                {
            //                    config.Cookie.Name = "UserLoginCookie";
            //                    config.LoginPath = "/Auth/login";

            //                });
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/Auth/login");
                options.AccessDeniedPath = new PathString("/Auth/login");
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("EmployeeOnly", policy =>
                {
                    //policy.AddAuthenticationSchemes("Cookie, Bearer");
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin");
                    policy.RequireClaim("EmployeeNumber");
                });
            });
            // services.AddMvc();
            // services.AddSingleton<IAuthorizationFilter, InspectionInstallationFilter>();
            services.AddMvc(option =>
            {
                option.Filters.Add<InspectionInstallationFilter>();
            });
            services.AddBundling()
                    .UseDefaults(_env)
                    .UseNUglify()
                    .EnableMinification()
                    .EnableChangeDetection()
                    .EnableCacheHeader(TimeSpan.FromHours(1));

            services.AddSevenZipCompressor();
            services.AddResumeFileResult();

            // DependencyInjectionService.IServiceProvider = services.BuildServiceProvider();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //依赖注入的对象
            builder.RegisterType<AccessService>();
            builder.RegisterType<AlbumService>();
            builder.RegisterType<APPStoreInfoService>();
            builder.RegisterType<AttachService>()
                .OnActivated(e => e.Instance.NoteService = e.Context.Resolve<NoteService>())
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<AuthService>().OnActivated(e =>
            {
                var authService = e.Instance;
                authService.UserService = e.Context.Resolve<UserService>();
                authService.TokenSerivce = e.Context.Resolve<TokenSerivce>();
            });
            builder.RegisterType<BlogService>()
                .OnActivated(e =>
                {
                    var blogService = e.Instance;
                    blogService.NoteService = e.Context.Resolve<NoteService>();
                    blogService.NoteContentService = e.Context.Resolve<NoteContentService>();
                    blogService.UserService = e.Context.Resolve<UserService>();
                    blogService.ConfigService = e.Context.Resolve<ConfigService>();
                    blogService.CommonService = e.Context.Resolve<CommonService>();
                })
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<CommonService>();
            builder.RegisterType<ConfigFileService>().SingleInstance();//单例模式 配置文件服务
            builder.RegisterType<ConfigService>();
            builder.RegisterType<EmailService>();
            builder.RegisterType<GoogleAuthenticatorService>();
            builder.RegisterType<GroupService>();
            builder.RegisterType<InitServices>();
            builder.RegisterType<NotebookService>().OnActivated(e =>
            {
                var instance = e.Instance;
                instance.UserService = e.Context.Resolve<UserService>();
            });
            builder.RegisterType<NoteContentHistoryService>();
            builder.RegisterType<NoteContentService>().OnActivated(e =>
            {
                var instance = e.Instance;
                instance.NoteImageService = e.Context.Resolve<NoteImageService>();
                instance.NoteService = e.Context.Resolve<NoteService>();
            });
            builder.RegisterType<NoteFileService>();
            builder.RegisterType<NoteImageService>();
            builder.RegisterType<NoteService>()
                .OnActivated(e =>
                {
                    var instance = e.Instance;
                    instance.ConfigService = e.Context.Resolve<ConfigService>();
                    instance.BlogService = e.Context.Resolve<BlogService>();
                    instance.NoteImageService = e.Context.Resolve<NoteImageService>();
                    instance.NoteImageService = e.Context.Resolve<NoteImageService>();
                    instance.AttachService = e.Context.Resolve<AttachService>();
                    instance.CommonService = e.Context.Resolve<CommonService>();
                    instance.UserService = e.Context.Resolve<UserService>();
                    instance.InitServices = e.Context.Resolve<InitServices>();
                    instance.NotebookService = e.Context.Resolve<NotebookService>();
                    instance.TagService = e.Context.Resolve<TagService>();
                    instance.NoteContentService = e.Context.Resolve<NoteContentService>();
                    instance.ShareService = e.Context.Resolve<ShareService>();
                })
                .InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<PwdService>();
            builder.RegisterType<RandomImageService>();
            builder.RegisterType<SessionService>();
            builder.RegisterType<ShareService>();
            builder.RegisterType<SpamService>().OnActivated(e =>
            {
                var instance = e.Instance;
                instance.ConfigFileService = e.Context.Resolve<ConfigFileService>();
            });
            builder.RegisterType<SuggestionService>();
            builder.RegisterType<TagService>().OnActivated(e =>
            {
                var instance = e.Instance;
                instance.NoteService = e.Context.Resolve<NoteService>();
                instance.UserService = e.Context.Resolve<UserService>();
            });
            builder.RegisterType<ThemeService>();
            builder.RegisterType<TokenSerivce>();
            builder.RegisterType<TrashService>().OnActivated(e =>
            {
                var instance = e.Instance;
                instance.noteService = e.Context.Resolve<NoteService>();
                instance.AttachService = e.Context.Resolve<AttachService>();
                instance.NoteContentService = e.Context.Resolve<NoteContentService>();
                instance.NotebookService = e.Context.Resolve<NotebookService>();
            });
            builder.RegisterType<UpgradeService>();

            builder.RegisterType<UserService>().OnActivated(e =>
            {
                var instance = e.Instance;
                instance.BlogService = e.Context.Resolve<BlogService>();
                instance.EmailService = e.Context.Resolve<EmailService>();
            })
            .InstancePerLifetimeScope()
            .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<Sha256PasswordStore>()
                .As<IPasswordStore>();
            builder.RegisterType<JiebaSegmenterService>()
         .As<JiebaSegmenterService>();
            //过滤器
            builder.RegisterType<CheckLoginFilter>();
            builder.RegisterType<CheckTokenFilter>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseDeveloperExceptionPage();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            //使用session 注册
            app.UseSession();
            //app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            //监控接口耗时情况
            //app.UseTimeMonitorMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}