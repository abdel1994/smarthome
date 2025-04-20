using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartFront;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Zet de base address op de domein/port (en optioneel subpad) waar je API draait.
builder.Services.AddScoped(sp => new HttpClient 
{
    BaseAddress = new Uri("http://192.168.2.2:8080/") 
    // Of http://localhost:8080/api/, afhankelijk van jouw endpoint-structuur
});

var app = builder.Build();
await app.RunAsync();


