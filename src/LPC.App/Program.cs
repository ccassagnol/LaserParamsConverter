/*
 * This file is part of LaserParamsConverter (cross-platform fork).
 * Licensed under the GNU General Public License, version 3.
 *
 * Local-only Blazor Server host. Binds to 127.0.0.1 on an ephemeral port and
 * launches the user's default browser. Cross-platform: no UI toolkit
 * dependency; the OS-native browser is the UI.
 */

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Pick a free ephemeral port so multiple instances don't collide.
var port = GetFreePort();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Loopback, port);
});

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var url = $"http://127.0.0.1:{port}/";
Console.WriteLine($"LaserParamsConverter is running at {url}");

// Auto-launch the default browser (Windows / macOS / Linux).
if (Environment.GetEnvironmentVariable("LPC_NO_BROWSER") != "1")
{
    _ = Task.Run(() => LaunchBrowser(url));
}

app.Run();

return 0;

static int GetFreePort()
{
    var listener = new TcpListener(IPAddress.Loopback, 0);
    listener.Start();
    try
    {
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }
    finally
    {
        listener.Stop();
    }
}

static void LaunchBrowser(string url)
{
    try
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
    }
    catch
    {
        // Browser launch is best-effort; the URL is also printed to the console.
    }
}
