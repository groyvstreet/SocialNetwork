using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

namespace PostService.API.Extensions
{
    public static class WebHostBuilderExtensions
    {
        public static void ConfigureKestrel(this IWebHostBuilder builder, IConfiguration configuration)
        {
            builder.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, 80, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });

                options.Listen(IPAddress.Any, 8080, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });

                options.Listen(IPAddress.Any, 443, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;

                    var certificate = configuration.GetSection("Certificate");
                    var path = certificate.GetSection("Path").Get<string>()!;
                    var password = certificate.GetSection("Password").Get<string>();

                    listenOptions.UseHttps(path, password);
                });
            });
        }
    }
}
