using Examination.Helper;
using Examination.IServices;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔥 เพิ่ม CORS ตรงนี้
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var options = ConfigurationOptions.Parse("localhost:6379");
options.AbortOnConnectFail = false;

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(options));

builder.Services.AddSingleton<RedisHelper>();
builder.Services.AddScoped<IExaminationService, ExaminationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔥 เปิดใช้ CORS ตรงนี้ (สำคัญ)
app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

app.Run();