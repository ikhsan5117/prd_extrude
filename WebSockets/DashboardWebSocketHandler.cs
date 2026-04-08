using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using VelastoProductionSystem.Data;

namespace VelastoProductionSystem.WebSockets
{
    public static class DashboardWebSocketHandler
    {
        private const int SendIntervalMs = 2500;

        public static async Task HandleAsync(WebSocket socket, IServiceProvider services)
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    using var scope = services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var readings = db.ProductionReadings
                        .OrderByDescending(r => r.ReadingTime)
                        .Take(12)
                        .Select(r => new
                        {
                            time = r.ReadingTime.ToString("HH:mm"),
                            temp = r.HeadTempInner ?? 0m,
                            speed = r.HoseSpeed ?? 0m
                        })
                        .ToList();

                    readings.Reverse();

                    var payload = new
                    {
                        labels = readings.Select(r => r.time).ToArray(),
                        datasets = new[]
                        {
                            new
                            {
                                label = "Head Temp Inner",
                                data = readings.Select(r => r.temp).ToArray(),
                                borderColor = "#3b82f6",
                                backgroundColor = "rgba(59,130,246,0.15)"
                            },
                            new
                            {
                                label = "Hose Speed",
                                data = readings.Select(r => r.speed).ToArray(),
                                borderColor = "#10b981",
                                backgroundColor = "rgba(16,185,129,0.15)"
                            }
                        }
                    };

                    var json = JsonSerializer.Serialize(payload);
                    var bytes = Encoding.UTF8.GetBytes(json);

                    await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);

                    if (socket.CloseStatus.HasValue)
                    {
                        break;
                    }

                    await Task.Delay(SendIntervalMs, CancellationToken.None);
                }
            }
            catch (WebSocketException)
            {
                // Connection aborted by client or network error.
            }
            finally
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
            }
        }
    }
}
