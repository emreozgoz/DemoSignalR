using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebAssembly;
using WebAssembly.SignalRConnections;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(x => new HttpClient { BaseAddress = new Uri("https://localhost:7032/") });
builder.Services.AddScoped<SignalRConnection>();

await builder.Build().RunAsync();
