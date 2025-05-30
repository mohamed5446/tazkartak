using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tazkartk.Application.Interfaces;
using Tazkartk.Application.Services;

namespace Tazkartk.Application
{
    public static class Dependandcies
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services,IConfiguration Configuration)
        {
            Services.AddScoped<ITripService, TripService>();
            Services.AddScoped<IAuthService, AuthService>();
            Services.AddScoped<IUserService, UserService>();
            Services.AddScoped<ICompanyService, CompanyService>(); 
            Services.AddScoped<IPaymentService, PaymentService>();
            Services.AddScoped<IBookingService, BookingService>();
           // Services.AddScoped<ITokenService,ITokenService>();

            Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    //        Services.AddHangfire(config =>
    //config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    //      .UseSimpleAssemblyNameTypeSerializer()
    //      .UseRecommendedSerializerSettings()
    //      .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection")));
    //       Services.AddHangfireServer();





            return Services;
        }
    }
}
