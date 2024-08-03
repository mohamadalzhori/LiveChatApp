using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Server.Data;
using Server.Data.Models;

namespace Server.Hubs;

public class ChatHub(AppDbContext _context) : Hub
{
    private static ConcurrentDictionary<string, string> userConnections = new ConcurrentDictionary<string, string>();

    public override async Task OnConnectedAsync()
    {
        var username = Context.GetHttpContext().Request.Query["username"];
        userConnections[username] = Context.ConnectionId;
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var username = Context.GetHttpContext().Request.Query["username"];
        userConnections.TryRemove(username, out _);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string fromUser, string toUser, string message)
    {
        if (userConnections.TryGetValue(toUser, out var connectionId))
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", fromUser, message);
        }

        // if the user is offline, save the message to the database
        
        _context.Messages.Add(new Message
        {
            FromUser = fromUser,
            ToUser = toUser,
            Content = message
        });


        await _context.SaveChangesAsync();
    }
}