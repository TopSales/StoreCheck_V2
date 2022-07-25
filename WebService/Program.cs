var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(cors => cors
   .WithOrigins("http://localhost:7149", "https://localhost:7149",
                  "http://ws.storecheck.pro/", "https://ws.storecheck.pro/",
                  "http://ws.storecheck.tech/", "https://ws.storecheck.tech/")
   .AllowAnyMethod()
   .AllowAnyHeader()
   //.AllowAnyOrigin()
   .SetIsOriginAllowed(origin => true)
   .AllowCredentials()
);

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
