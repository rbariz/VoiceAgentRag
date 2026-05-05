using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace VoiceAgentRag.Api.Realtime
{
    public sealed class VoiceAgentHub : Hub
    {
        public async Task JoinOps()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "ops");
        }

        public async Task LeaveOps()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "ops");
        }
    }
}
