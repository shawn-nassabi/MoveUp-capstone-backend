using health_app_backend;
using health_app_backend.Jobs;
using health_app_backend.Mappings;
using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;
using health_app_backend.Repositories;
using health_app_backend.Services;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;

var builder = WebApplication.CreateBuilder(args);

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHealthDataRepository, HealthDataRepository>();
builder.Services.AddScoped<IDemographicBenchmarkRepository, DemographicBenchmarkRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IDataTypeRepository, DataTypeRepository>();
builder.Services.AddScoped<IUserBenchmarkRecordRepository, UserBenchmarkRecordRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();
builder.Services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
builder.Services.AddScoped<IClanRepository, ClanRepository>();
builder.Services.AddScoped<IClanMemberRepository, ClanMemberRepository>();
builder.Services.AddScoped<IClanJoinRequestRepository, ClanJoinRequestRepository>();
builder.Services.AddScoped<IClanChallengeRepository, ClanChallengeRepository>();
builder.Services.AddScoped<IClanChallengeProgressRepository, ClanChallengeProgressRepository>();
builder.Services.AddScoped<IDailySyncRecordRepository, DailySyncRecordRepository>();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHealthDataService, HealthDataService>();
builder.Services.AddScoped<IDemographicBenchmarkService, DemographicBenchmarkService>();
builder.Services.AddScoped<IUserBenchmarkRecordService, UserBenchmarkRecordService>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IClanService, ClanService>();
builder.Services.AddScoped<IDataTypeService, DataTypeService>();
builder.Services.AddScoped<IBlockchainService, BlockchainService>();

// Add Hosted Services
builder.Services.AddHostedService<DataSyncJob>();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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