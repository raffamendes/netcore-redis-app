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
    string? redisPassword = Environment.GetEnvironmentVariable("REDIS_PASS");
    string? redisUser = Environment.GetEnvironmentVariable("REDIS_USER");
    Console.WriteLine(redisConnection);
    Console.WriteLine(redisPassword);
    Console.WriteLine(redisUser);
    ConfigurationOptions configOptions = ConfigurationOptions.Parse(redisConnection).Apply(c=>c.CertificateValidation+=(_, _, _, _) => true);
    configOptions.User = redisUser;
    configOptions.Password = redisPassword;
    configOptions.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(redisConnection);
});

builder.Services.AddHealthChecks();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

app.MapControllers();

app.MapHealthChecks("/healthz");

app.Run();