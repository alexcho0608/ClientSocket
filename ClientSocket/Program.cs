using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientSocket
{
    class Program
    {
        static void Main(string[] args)
        {
            var wscli = new ClientWebSocket();
            var tokSrc = new CancellationTokenSource();
            var task = wscli.ConnectAsync(new Uri("localhost:8181"), tokSrc.Token);
            task.Wait(); task.Dispose();
            Console.WriteLine("WebSocket to ws://localhost:8181 OPEN!");
            Console.WriteLine("SubProtocol: " + wscli.SubProtocol ?? "");

            Console.WriteLine(@"Type ""exit<Enter>"" to quit... ");
            for (var inp = Console.ReadLine(); inp != "exit"; inp = Console.ReadLine())
            {
                task = wscli.SendAsync(
                            new ArraySegment<byte>(Encoding.UTF8.GetBytes(inp)),
                            WebSocketMessageType.Text,
                            false,
                            tokSrc.Token
                        );
                task.Wait(); task.Dispose();
                Console.WriteLine("**** sent msg");
            }

            if (wscli.State == WebSocketState.Open)
            {
                task = wscli.CloseAsync(WebSocketCloseStatus.NormalClosure, "", tokSrc.Token);
                task.Wait(); task.Dispose();
            }
            tokSrc.Dispose();
            Console.WriteLine("WebSocket CLOSED");
        }
    }
}
