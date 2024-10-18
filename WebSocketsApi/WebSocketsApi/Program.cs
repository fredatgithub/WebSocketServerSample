using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Enable WebSockets
app.UseWebSockets();

app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await HandleWebSocketConnection(context, webSocket);
    }
    else
    {
        await next();
    }
});

// Map routes or endpoints if needed
// app.MapControllers(); // For API controller routes

app.Run();

// This is the WebSocket handler
async Task HandleWebSocketConnection(HttpContext context, WebSocket webSocket)
{
    var buffer = new byte[1024 * 4];
    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    while (!result.CloseStatus.HasValue)
    {
        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine("Received WebSocket Server: " + message);
        // Echo the message back to the client
        var serverMessage = Encoding.UTF8.GetBytes("I am Server " + message);
        await webSocket.SendAsync(new ArraySegment<byte>(serverMessage, 0, serverMessage.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    }

    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
}
