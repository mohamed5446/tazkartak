using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Hangfire;
using Microsoft.OpenApi.Models;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.Dashboard;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tazkartk.API.MiddleWares;
using Tazkartk.Application;
using Tazkartk.Application.DTO.Response;
using Tazkartk.Infrastructure;

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
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

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


builder.Services.AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();


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
            ClockSkew = TimeSpan.Zero
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
