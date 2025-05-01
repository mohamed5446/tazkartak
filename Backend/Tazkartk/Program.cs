using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tazkartk.Data;
using Tazkartk.Helpers;
using Tazkartk.Models;
using Tazkartk.Services;
using Tazkartk.Email;
using Tazkartk.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Hangfire;
using Microsoft.OpenApi.Models;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.Dashboard;
using Tazkartk.Google;
using Tazkartk.MiddleWares;
using Tazkartk.DTO.Response;
using System.Text.Json;
using Tazkartk.Models.Enums;
using System.Text.Json.Serialization;
using Tazkartk.Caching;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
}); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors();
builder.Services.AddSwaggerGen(options => 
{
    options.EnableAnnotations();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddScoped<ITripService, TripService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymobService, PaymobService>();
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddScoped<ICachingService,CachingService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();


//builder.Services.Configure<ApiBehaviorOptions>(options
//    => options.SuppressModelStateInvalidFilter = true);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("redis");
    options.InstanceName = "Tazkartk_";
    //options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
    //{
    //    EndPoints = { "pleasing-beagle-10882.upstash.io:6379" },
    //    Password = "ASqCAAIjcDEzY2RkMjZkYmE4MWY0ODIyOGY1ZDFhOTg4M2IxMDk3OHAxMA",
    //    Ssl = true,
    //    AbortOnConnectFail = false
    //};
});
builder.Services.AddIdentityCore<Account>(options =>
{
    options.User.AllowedUserNameCharacters = null;
}).AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.Configure<PaymobSettings>(builder.Configuration.GetSection("Paymob"));
builder.Services.Configure<GoogleAuthSettings>(builder.Configuration.GetSection("Google"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = (actionContext) =>
        {
            var errors = actionContext.ModelState
            .Where(ms => ms.Value.Errors.Count > 0)
            .SelectMany(e => e.Value.Errors)
            .Select(em => em.ErrorMessage)
            .ToList();

            string message= string.Join("; ", errors);
            var validationErrorResponse = ApiResponse<string>.Error(message);

            return new BadRequestObjectResult(validationErrorResponse);
        };
    });
    builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
        };
        o.Events=  new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse(); 
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<string>.Error("Unauthorized",StatusCode.Unauthorized);

                return context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        };
    });
builder.Services.AddHangfire(config =>
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<ExceptionHandlingMiddleware>();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c=>c.DefaultModelsExpandDepth(-1));
}
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.DefaultModelsExpandDepth(-1));
}

app.UseHttpsRedirection();



app.UseCors(c => c
        .SetIsOriginAllowed(origin => true) 
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());


app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[]
    {
        new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
        {
            RequireSsl = false,
            SslRedirect = false,
            LoginCaseSensitive = false,
            Users = new[]
            {
                new BasicAuthAuthorizationUser
                {
                    Login = "admin",
                    PasswordClear = "test",
                },
            },
        }),
    },
    IsReadOnlyFunc = (DashboardContext context) => true,
});
//app.MapHangfireDashboard("/hangfire");
app.Run();
