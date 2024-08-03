using Client;
using Grpc.Net.Client;
using Microsoft.AspNetCore.SignalR.Client;

Console.Clear();

Console.Write("Enter your username: ");
var username = Console.ReadLine();

string receiver = SetReceiver();

Console.WriteLine("Connected to the chat hub.");

var consoleWidth = Console.WindowWidth;

await GetHistory();


var connection = new HubConnectionBuilder()
    .WithUrl($"https://localhost:7160/livechat?username={username}")
    .Build();

connection.On<string, string>("ReceiveMessage", (fromUser, message) =>
{
    if (receiver == fromUser)
    {
        PrintHisMessage(fromUser, message);
    }
    else
    {
       NewNotification(fromUser); 
    }
});

await connection.StartAsync();


while (true)
{
    var key = Console.ReadKey();

    if (key.Key == ConsoleKey.R && key.Modifiers == ConsoleModifiers.Control)
    {
        Console.Clear();
        receiver = SetReceiver();
        await GetHistory();
    }
    else
    {
        var message = key.Key.ToString() + Console.ReadLine();

        if (string.IsNullOrEmpty(message)) continue;

        // Erase the typed message
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop);


        // Show your message
        PrintMyMessage(message);

        await connection.InvokeAsync("SendMessage", username, receiver, message);
    }
}


static string SetReceiver()
{
    Console.Write("Enter receiver username: ");
    return Console.ReadLine();
}

void PrintMyMessage(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"You: {message}".PadLeft(consoleWidth));
    Console.ForegroundColor = ConsoleColor.White; 
}

void PrintHisMessage(string fromUser, string message)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{fromUser}: {message}");
    Console.ForegroundColor = ConsoleColor.White;
}

void NewNotification(string fromUser)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"You have a new message from {fromUser}!");
    Console.ForegroundColor = ConsoleColor.White; 
}

async Task GetHistory()
{ 
    
    // Create a gRPC channel
    var channel = GrpcChannel.ForAddress("https://localhost:7160"); // Adjust the URL to match your server

// Create a client for the MessageHistory service
    var client = new MessageHistory.MessageHistoryClient(channel);

// Prepare the request
    var request = new HistoryRequest
    {
        FromUser = username,
        ToUser = receiver
    };

// Call the GetMessageHistory method
    var response = await client.GetMessageHistoryAsync(request);

    foreach (var message in response.Messages)
    {
        if (message.FromUser == username)
        {
            PrintMyMessage(message.Content);
        }
        else
        {
           PrintHisMessage(message.FromUser, message.Content); 
        } 
    }
}