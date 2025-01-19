using Microsoft.AspNetCore.SignalR;

namespace QMTGroup.Web2
{
    public class VideoHub : Hub
    {
        public async Task SendFrame(string base64Image)
        {
            await Clients.All.SendAsync("ReceiveFrame", base64Image);
        }
    }
}
