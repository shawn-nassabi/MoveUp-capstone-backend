using health_app_backend;
using health_app_backend.Helpers;
using health_app_backend.Jobs;
using health_app_backend.Mappings;
using health_app_backend.Models;
using Microsoft.EntityFrameworkCore;
using health_app_backend.Repositories;
using health_app_backend.Services;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;

// WalletExporter.Main(); // Run once to generate wallets



var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

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
builder.Services.AddScoped<IPointsRewardHistoryRepository, PointsRewardHistoryRepository>();
builder.Services.AddScoped<ITokenRewardHistoryRepository, TokenRewardHistoryRepository>();

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
builder.Services.AddHostedService<RewardHistorySyncJob>();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error"); // global error handler
   // app.UseHsts();
}

// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     try
//     {
//         var dbContext = services.GetRequiredService<AppDbContext>();
//         await DbSeeder.SeedUsersAsync(dbContext);
//         Console.WriteLine("Seeding done");
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine($"❌ Error during seeding: {ex.Message}");
//     }
// }

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();