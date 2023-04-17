using RippleTest;
using RippleTest.Broadcast;
using RippleTest.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TransactionDictionary>();
builder.Services.AddSingleton<IRippleService, RippleService>();
builder.Services.AddSingleton<IDogeService, DogeService>();
builder.Services.AddSingleton<ICryptoApiService, CryptoApiService>();
// builder.Services.AddSingleton<RippleBinaryObject>();

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