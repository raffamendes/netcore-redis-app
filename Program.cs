using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

//configure redis

builder.Services.AddSingleton<IConnectionMultiplexer>(provider => {
    //string? redisConnection = builder.Configuration.GetConnectionString("RedisHost");
    string? redisConnection = Environment.GetEnvironmentVariable("REDIS_HOST");
    return ConnectionMultiplexer.Connect(redisConnection);
});

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.MapControllers();

app.Run();