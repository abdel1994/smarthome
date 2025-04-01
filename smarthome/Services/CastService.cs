using Sharpcaster;
using Sharpcaster.Channels;

public class CastService
{
    private readonly ChromecastClient _client;
    // constructor to initialize the Chromecast client
    public CastService()
    {
        _client = new ChromecastClient();
       // _client.ConnectChromecast("").GetAwaiter().GetResult(); // Connect to the Chromecast device, getAwaiter to wait for the task to complete and Get result to get the result of the task
    }

    public ChromecastClient Client => _client; // property to get the Chromecast client, => is a shorthand for defining a property that returns the value of a private field
}