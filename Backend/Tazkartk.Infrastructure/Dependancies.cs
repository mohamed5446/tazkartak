using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tazkartk.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Tazkartk.Infrastructure.Helpers;
using Tazkartk.Application.Repository;
using Tazkartk.Infrastructure.Repositories;
using Hangfire;
using Tazkartk.Domain.Models;
using Tazkartk.Infrastructure.Google;
using Tazkartk.Infrastructure.Token;
using Tazkartk.Infrastructure.Caching;
using Tazkartk.Infrastructure.excel;
using Tazkartk.Infrastructure.BackgroundJob;
using Tazkartk.Infrastructure.Email;
using Tazkartk.Infrastructure.Cloudinary;
using Tazkartk.Application.Interfaces.External;
using Tazkartk.Infrastructure.Services.PaymentGateway.Paymob;


namespace Tazkartk.Infrastructure
{
    public static class Dependancies
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection Services,IConfiguration Configuration)
        {
            Services.AddScoped<IUserRepository,UserRepository>();
            Services.AddScoped<ITripRepository,TripRepository>();
            Services.AddScoped<ICompanyRepository,CompanyRepository>();
            Services.AddScoped<ITicketRepository,TicketRepository>();
            Services.AddScoped<IPaymentRepository,PaymentRepository>();
            Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            Services.AddScoped<IUnitOfWork,UnitOfWork>();

            //Services.AddScoped<IGenericRepository, GenericRepository>();

           
            Services.AddScoped<ICachingService, RedisService>();
            Services.AddScoped<IGoogleAuthService, GoogleAuthService>();

            Services.AddScoped<IPaymentGateway, PaymobService>();
            Services.AddTransient<IEmailService, EmailService>();
            Services.AddScoped<IEmailBodyService,EmailBodyService>();
            Services.AddScoped<IPhotoService, CloudinaryService>();
            Services.AddScoped<ITokenService, JWTService>();
            Services.AddScoped<IBackgroundService, HangfireService>();
            Services.AddScoped<IExcelService, ExcelService>();

            // Services.AddScoped<IStripeService, StripeService>();
            //builder.Services.AddTransient<ISMSService,SMSService>();
            //Services.AddScoped<ITapService, TapService>();
            // Services.AddScoped<ICloudinaryService, CloudinaryService>();

            Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            });
            Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("redis");
                options.InstanceName = "Tazkartk_";
                options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                {
                    EndPoints = { "pleasing-beagle-10882.upstash.io:6379" },
                    Password = "ASqCAAIjcDEzY2RkMjZkYmE4MWY0ODIyOGY1ZDFhOTg4M2IxMDk3OHAxMA",
                    Ssl = true,
                    AbortOnConnectFail = false
                };
            });
            Services.AddIdentityCore<Account>(options =>
              {
                  options.User.AllowedUserNameCharacters = null;
              }).AddRoles<IdentityRole<int>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection")));


            Services.AddHangfireServer();
            Services.Configure<JWT>(Configuration.GetSection("JWT"));
            Services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            Services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            Services.Configure<PaymobSettings>(Configuration.GetSection("Paymob"));
            Services.Configure<GoogleAuthSettings>(Configuration.GetSection("Google"));
            Services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            Services.Configure<TwilioSettings>(Configuration.GetSection("Twilio"));

           

            return Services;
        }
    }
}
