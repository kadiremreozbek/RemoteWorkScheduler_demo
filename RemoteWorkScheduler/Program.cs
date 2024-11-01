using Microsoft.EntityFrameworkCore;
using RemoteWorkScheduler.DbContexts;
using RemoteWorkScheduler.Services;
using Serilog;
using AutoMapper;
using RemoteWorkScheduler.Profiles;
using System.Text;
using RemoteWorkScheduler.Validators;
using RemoteWorkScheduler.Models;
using FluentValidation;
using RemoteWorkScheduler.AppService;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IValidator<EmployeeForUpdateDto>, EmployeeUpdateValidator>();
builder.Services.AddScoped<IValidator<TeamForUpdateDto>, TeamUpdateValidator>();
builder.Services.AddScoped<IValidator<EmployeeForCreationDto>,EmployeeCreationValidator>();
builder.Services.AddScoped<IValidator<TeamForCreationDto>, TeamCreationValidator>();
builder.Services.AddScoped<IValidator<RemoteLogForCreationDto>, RemoteLogCreationValidator>();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddScoped<IReWoSeRepository, ReWoSeRepository>();
builder.Services.AddScoped<IEmployeeAppService, EmployeeAppService>();
builder.Services.AddScoped<ITeamAppService, TeamAppService>();
builder.Services.AddScoped<IRemoteLogAppService, RemoteLogAppService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<RemoteWorkSchedulerContext>(dbContextOptions =>
    dbContextOptions.UseSqlite(builder.Configuration["ConnectionStrings:ReWoSeDBConnectionString"]));

// Register AutoMapper and scan for all profiles in the current domain's assemblies
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
