using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ComuniApp.Api.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> _connections
            = new();
        public override Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            if (http != null && int.TryParse(http.Request.Query["userId"], out var userId))
            {
                var conns = _connections.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());
                conns[Context.ConnectionId] = 0;
                // notificar (opcional)
                Clients.All.SendAsync("UserConnected", userId);
            }

            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var kv in _connections)
            {
                if (kv.Value.TryRemove(Context.ConnectionId, out _))
                {
                    if (kv.Value.IsEmpty)
                        _connections.TryRemove(kv.Key, out _);

                    Clients.All.SendAsync("UserDisconnected", kv.Key);
                    break;
                }
            }

            return base.OnDisconnectedAsync(exception);
        }
        
        // Enviar mensaje: si toUserId == 0 => broadcast, sino envio dirigido
        public Task SendMessage(int fromUserId, int toUserId, string message)
        {
            var payload = new
            {
                fromUserId,
                toUserId,
                message,
                timestamp = DateTime.UtcNow
            };



            if (toUserId <= 0)
            {
                return Clients.All.SendAsync("ReceiveMessage", payload);
            }

            // Si el destinatario no está conectado, simplemente ignora o guarda para luego
            if (!_connections.TryGetValue(toUserId, out var targetConns))
            {
                Console.WriteLine($"⚠️ Usuario {toUserId} no conectado. Mensaje no entregado.");
                return Task.CompletedTask;
            }

            // si no está conectado el destinatario, opcional: fallback al emisor o guardar en BD (no implementado)
            return Task.CompletedTask;
        }
    }
}