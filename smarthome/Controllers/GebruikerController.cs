using InTheHand.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InTheHand.Net.Bluetooth;
using Sharpcaster;
using Sharpcaster.Interfaces;
using Sharpcaster.Models.Media;
using Sharpcaster.Models;
using Linux.Bluetooth;
using Sharpcaster.Channels;
using System.Diagnostics;

[ApiController]
[Route("api/[controller]")]
public class GebruikerController : ControllerBase
{
    private readonly CastService _castService;
    private readonly ILogger<GebruikerController> _logger;

    public GebruikerController(ILogger<GebruikerController> logger, CastService castService)
    {
        _castService = castService;
        _logger = logger;
    }

    
    [HttpGet("ScanBluetooth")]
    public async Task<IActionResult> ScanApparaten()
    {

        var startInfo = new ProcessStartInfo
        {
            FileName = "/usr/bin/expect",
            Arguments = "/app/scan.expect",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo)!;

        string output = await process.StandardOutput.ReadToEndAsync();
        output += await process.StandardError.ReadToEndAsync();

        _logger.LogInformation("Bluetooth stdout/stderr:\n" + output);

        var lines = output
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Where(line => line.Trim().StartsWith("Device "))
            .Select(line =>
            {
                var parts = line.Split(' ', 3);
                return new
                {
                    mac = parts[1],
                    name = parts.Length > 2 ? parts[2] : "Unknown"
                };
            })
            .ToList();

        if (!lines.Any())
        {
            _logger.LogWarning("Geen Bluetooth-apparaten gevonden.");
        }

        return Ok(lines );
    }
   


    [HttpPost("Scan Netwerk")]
    public async Task<IActionResult> GetNetworkDevices()
    {
        IChromecastLocator locator = new MdnsChromecastLocator();
        var source = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        var chromecasts = await locator.FindReceiversAsync(source.Token);

        foreach (var device in chromecasts)
        {
            Console.WriteLine(device);
        }

        var chromecast = chromecasts.FirstOrDefault();
        if (chromecast == null)
            return NotFound("Geen Chromecast gevonden.");

        _ = await _castService.Client.ConnectChromecast(chromecast);
        _ = await _castService.Client.LaunchApplicationAsync("B3419EF5");

        return Ok(chromecasts);
    }

    [HttpPost("Play ChromeCast")]
    public async Task<IActionResult> Play()
    {
        
            var media = new Media
            {
                ContentUrl = "http://192.168.2.2:5600/Audio/Athan.mp3"
            };
            _ = await _castService.Client.MediaChannel.LoadAsync(media, true);

            return Ok(media);
    }
       
    

    [HttpPost("PlayBluetooth")]
    public async Task<IActionResult> PlayBluetooth()
    {
        var audioPath = "/app/Audio/Athan.mp3";
        var psi = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"mplayer '{audioPath}'\"",
            WorkingDirectory = "/tmp",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = await Task.Run(() => Process.Start(psi));
        if (process == null)
        {
            return StatusCode(500, "Failed to start the audio playback process.");
        }

        return Ok(new { message = "Audio playback started successfully." });
    }
            

    

    [HttpPost("Volume")]
    public async Task<IActionResult> Volume([FromBody] float vol)
    {
        _ = await _castService.Client.ReceiverChannel.SetVolume(vol);
        return Ok();
    }

    [HttpPost("ConnectBluetooth")]
    public async Task<IActionResult> ConnectBluetooth([FromBody] string macAddress)
    {
    // expect doc!
    
    
    var psi = new ProcessStartInfo
    {
        FileName  = "/usr/bin/expect",
        Arguments = $"/app/connect.expect {macAddress}",
        RedirectStandardOutput = true,
        RedirectStandardError  = true,
        UseShellExecute = false,
        CreateNoWindow  = true
    };

    using var process = Process.Start(psi)!;

    var stdOutTask = process.StandardOutput.ReadToEndAsync();
    var stdErrTask = process.StandardError.ReadToEndAsync();
    if (!process.WaitForExit(30000))
    {
        process.Kill(entireProcessTree: true);
        return StatusCode(504, "Timeout tijdens Bluetooth-connectie.");
    }
    var outp = await stdOutTask;
    var errp  = await stdErrTask;
    _logger.LogInformation(outp);

    if (!string.IsNullOrEmpty(errp)) _logger.LogWarning(errp);
        var ok = process.ExitCode == 0;
        return ok
            ? Ok(new { result="success", output=outp})
            : StatusCode(500, new { result="failure", output=outp, error=errp });

    }

    
}
