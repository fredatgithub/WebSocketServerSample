// See https://aka.ms/new-console-template for more information
using System.Net.WebSockets;
using System.Text;

using (ClientWebSocket client = new ClientWebSocket())
{
    Uri serverUri = new Uri("wss://localhost:7137/");
    await client.ConnectAsync(serverUri, CancellationToken.None);
    Console.WriteLine("Connected to WebSocket server");


    var receiveTask = ReceiveMessagesAsync(client);
    var sendTask = SendMessagesAsync(client);

    await Task.WhenAll(sendTask, receiveTask);
}

static async Task SendMessagesAsync(ClientWebSocket client)
{
    while (client.State == WebSocketState.Open)
    {
        string message = Console.ReadLine();
        var messageBuffer = Encoding.UTF8.GetBytes(message);
        await client.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}

static async Task ReceiveMessagesAsync(ClientWebSocket client)
{
    var buffer = new byte[1024];
    while (client.State == WebSocketState.Open)
    {
        var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine("Received: " + message);
    }
}
