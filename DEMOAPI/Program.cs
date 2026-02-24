using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text;
using EmployeeApi.Models;
using EmployeeApi.Services;
using EmployeeApi.Repositories;
using EmployeeApi.GrpcServices;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IEmployeeProjectRepository, EmployeeProjectRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

// Data Protection (for URL encryption)
builder.Services.AddDataProtection();
builder.Services.AddSingleton<IUrlEncryptionService, UrlEncryptionService>();

// Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordValidator, PasswordValidator>();
builder.Services.AddScoped<IEmployeeMapper, EmployeeMapper>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEmployeeProjectService, EmployeeProjectService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Controllers
builder.Services.AddControllers();

// gRPC
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200", "http://localhost:5127")

                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowAngular");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// gRPC endpoints
app.MapGrpcService<EmployeeGrpcService>();
app.MapGrpcService<DepartmentGrpcService>();
app.MapGrpcService<NotificationGrpcService>();
app.MapGrpcReflectionService();

app.Run();
