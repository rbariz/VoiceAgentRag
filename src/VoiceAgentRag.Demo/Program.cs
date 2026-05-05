using VoiceAgentRag.Demo.Components;
using VoiceAgentRag.Demo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<VoiceAgentApiClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5234");
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();