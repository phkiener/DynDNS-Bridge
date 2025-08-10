namespace DynDNS.Core.Infrastructure;

public sealed class CurrentAddressProvider(IHttpClientFactory httpClientFactory) : ICurrentAddressProvider
{
    public async Task<string?> GetIPv4AddressAsync()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://4.icanhazip.com/");

        return response.IsSuccessStatusCode ? (await response.Content.ReadAsStringAsync()).Trim() : null;
    }

    public async Task<string?> GetIPv6AddressAsync()
    {
        var client = httpClientFactory.CreateClient();
        var response = await client.GetAsync("https://6.icanhazip.com/");

        return response.IsSuccessStatusCode ? (await response.Content.ReadAsStringAsync()).Trim() : null;
    }
}
