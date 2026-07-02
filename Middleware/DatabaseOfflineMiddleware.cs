using Microsoft.Data.SqlClient;

namespace VelastoProductionSystem.Middleware
{
    /// <summary>
    /// Middleware yang menangkap error koneksi database (SQL Server tidak dapat dijangkau)
    /// dan mengarahkan pengguna ke halaman offline yang informatif.
    /// </summary>
    public class DatabaseOfflineMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseOfflineMiddleware> _logger;

        public DatabaseOfflineMiddleware(RequestDelegate next, ILogger<DatabaseOfflineMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) when (IsDbConnectionError(ex))
            {
                _logger.LogWarning("[DatabaseOffline] Koneksi database gagal: {Message}", ex.Message);

                // Jika request adalah AJAX/API → kembalikan JSON error
                if (IsAjaxRequest(context.Request))
                {
                    context.Response.StatusCode = 503;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(
                        "{\"success\":false,\"offline\":true,\"message\":\"Server database tidak dapat dijangkau. Gunakan mode offline.\"}");
                    return;
                }

                // Jika request adalah navigasi biasa → redirect ke halaman offline
                context.Response.Redirect("/offline.html");
            }
        }

        private static bool IsDbConnectionError(Exception ex)
        {
            // Telusuri seluruh inner exception chain
            var current = ex;
            while (current != null)
            {
                if (current is SqlException sqlEx)
                {
                    // Semua error code yang berkaitan dengan koneksi / jaringan
                    // -2=timeout, 0=generic, 2=server not found, 19=physical connection unusable,
                    // 53=network error, 64=connection terminated, 233=handshake,
                    // 10053/10054/10060/10061=socket errors
                    if (sqlEx.Number is -2 or 0 or 2 or 19 or 53 or 64 or 233 or 10053 or 10054 or 10060 or 10061)
                        return true;

                    // Cek teks pesan untuk kasus lainnya
                    var msg = sqlEx.Message;
                    if (msg.Contains("unreachable", StringComparison.OrdinalIgnoreCase) ||
                        msg.Contains("socket", StringComparison.OrdinalIgnoreCase) ||
                        msg.Contains("transport-level", StringComparison.OrdinalIgnoreCase) ||
                        msg.Contains("Physical connection is not usable", StringComparison.OrdinalIgnoreCase) ||
                        msg.Contains("server was not found", StringComparison.OrdinalIgnoreCase) ||
                        msg.Contains("network-related", StringComparison.OrdinalIgnoreCase) ||
                        msg.Contains("instance-specific", StringComparison.OrdinalIgnoreCase))
                        return true;
                }

                if (current is System.Net.Sockets.SocketException)
                    return true;

                current = current.InnerException;
            }
            return false;
        }

        private static bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                   request.Headers["Accept"].ToString().Contains("application/json") ||
                   (request.ContentType?.Contains("application/json") == true);
        }
    }

    // Extension method agar mudah didaftarkan di Program.cs
    public static class DatabaseOfflineMiddlewareExtensions
    {
        public static IApplicationBuilder UseDatabaseOfflineFallback(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DatabaseOfflineMiddleware>();
        }
    }
}
