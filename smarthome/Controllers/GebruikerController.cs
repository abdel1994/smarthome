using InTheHand.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smarthome.Data;
using smarthome.Models;
using InTheHand.Net.Bluetooth;
using Sharpcaster;
using Sharpcaster.Interfaces;
using Sharpcaster.Models.Media;
using Sharpcaster.Models;
using Linux.Bluetooth;
using Sharpcaster.Channels;
using smarthome;



[ApiController]
[Route("api/[controller]")]

public class GebruikerController: ControllerBase
{
    private readonly CastService _castService;  // service to connect to the chromecast device.
    private readonly ILogger<GebruikerController> _logger;
    private readonly ApiDBContext _context;

    // constructor to initialize the logger and the database context and the cast service.

    public GebruikerController(ILogger<GebruikerController> logger, ApiDBContext context, CastService castService)
    {
        _castService = castService;
        _logger = logger;
        _context = context;        
    }
  
    
    [HttpGet( "GetGebruikers")]
    public async Task<IActionResult> Get()
    {
        var gebruiker = new Gebruiker()
        {
            Naam = "Jan",
            Email = "CitroenMelisse@live.nl",
            Wachtwoord = "Welkom123!"
        };

        _context.Gebruikers.Add(gebruiker);
        await _context.SaveChangesAsync();

        var allDrivers = await _context.Gebruikers.ToListAsync();
        return Ok(allDrivers);
              
    }

    [HttpPost("GetBluetooth")]
    public async Task<IActionResult> GetBluetooth() 
    {
        BluetoothClient client = new BluetoothClient();
        var devices = await Task.Run(() => client.DiscoverDevices());
        foreach (var device in devices)
        {
            Console.WriteLine(device.DeviceName);
        }
        return Ok(devices);
    }

    [HttpPost("GetNetworkDevices")]
    public async Task<IActionResult> GetNetworkDevices()
    {
        

        IChromecastLocator Locator = new MdnsChromecastLocator();
        var source = new CancellationTokenSource(TimeSpan.FromSeconds(15));

        var chromecasts = await Locator.FindReceiversAsync(source.Token);
        Console.WriteLine("" + chromecasts.FirstOrDefault());
        foreach (var devices in chromecasts)
        {
            Console.WriteLine(devices);
        }

        var chromecast = chromecasts.First();
        
        _ = await _castService.Client.ConnectChromecast(chromecast);
        _ = await  _castService.Client.LaunchApplicationAsync("B3419EF5");
        

        var media = new Media
        {
            ContentUrl = "https://media.sd.ma/assabile/adhan_3435370/0bf83c80b583.mp3"
        };
        

        _ = await _castService.Client.MediaChannel.LoadAsync(media, true);
       
      
        
        return Ok(chromecasts);
    }

     [HttpPost("Volume")]
    public async Task<IActionResult> Volume([FromBody] float vol)
    {
        
        
        _ = await _castService.Client.ReceiverChannel.SetVolume(vol);
        return Ok();
    } 


    // bluetooth functionaliteit
    [HttpPost("ConnecttoDevice")]
    public async Task<IActionResult> ConnectDevice([FromBody]string device)
    {
        var client = new BluetoothClient();
        var deviceInfo = await Task.Run(() => client.DiscoverDevices());
        var tagetDevice = deviceInfo.FirstOrDefault(d => d.DeviceName == device);
        if (tagetDevice == null)
        {
            return NotFound("geen bluetooth device gevonden met de naam: " + device);
        }

        try
        {
            await Task.Run(() => client.Connect(tagetDevice.DeviceAddress, BluetoothService.AudioSink));
            return Ok("Connected to device: " + device);
        }
        catch (Exception e)
        {
            return StatusCode(500, "fout bij verbinden met device: " + device + " foutmelding: " + e.Message);
        }
       
    }
}
