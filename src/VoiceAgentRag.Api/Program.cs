using VoiceAgentRag.Api.Middleware;
using VoiceAgentRag.Api.Realtime;
using VoiceAgentRag.Application;
using VoiceAgentRag.Application.Abstractions.Realtime;
using VoiceAgentRag.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSignalR();

builder.Services.AddScoped<IRealtimeNotifier, SignalRRealtimeNotifier>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("DemoCors", policy =>
    {
        policy
            .WithOrigins("http://localhost:5035", "http://localhost:5050")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("DemoCors");

app.MapControllers();
app.MapHub<VoiceAgentHub>("/hubs/voice-agent");
app.Run();