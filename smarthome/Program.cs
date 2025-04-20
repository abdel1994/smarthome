using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the containe

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


// registreer de chromecasrService als singleton, dus er is maar 1 instantie van de service in de applicatie.
builder.Services.AddSingleton<CastService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
                    
        });
});
  

var app = builder.Build();



// Configure the HTTP request pipeline.
    app.UseCors("AllowAllOrigins");
    app.UseSwagger();
    app.UseSwaggerUI();



app.UseStaticFiles();
app.UseHttpsRedirection();

app.MapControllers();
app.Run();

