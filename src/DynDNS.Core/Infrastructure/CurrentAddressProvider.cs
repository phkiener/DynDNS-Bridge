using Microsoft.Extensions.Logging;

namespace DynDNS.Core.Infrastructure;

public sealed class CurrentAddressProvider(IHttpClientFactory httpClientFactory, ILogger<CurrentAddressProvider> logger) : ICurrentAddressProvider
{
    public async Task<string?> GetIPv4AddressAsync()
    {
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://4.icanhazip.com/");

            return response.IsSuccessStatusCode ? (await response.Content.ReadAsStringAsync()).Trim() : null;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get IPv6 address.");
            return null;
        }
    }

    public async Task<string?> GetIPv6AddressAsync()
    {
        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://6.icanhazip.com/");

            return response.IsSuccessStatusCode ? (await response.Content.ReadAsStringAsync()).Trim() : null;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get IPv6 address.");
            return null;
        }
    }
}
