# Hetzner DynDNS-Bridge for Fritz!Box

Tiny tool to automatically update DNS records in the Hetzner DNS Console when your Fritz!Box' IP address changes.

## Setup

Run the webhost either directly or through docker. Ensure that you have an environment variable `DDNSBRIDGE_API_KEY` which contains the API Key
for the Hetzner DNS Console.

In your Fritz!Box, head to the DynDNS configuration and enter the following:

- Provider: custom
- Update URL: `http://<your-ip>:<your-port>/update/<zone>/<name>?v4=<ipaddr>&v6=<ip6addr>`
- Domain: Not used, just enter whatever
- Username: Not used, just enter whatever
- Password: Not used, just enter whatever

## Example

Run the webhost on any device in your local network; preferably one with a static IP address. I recommend explicitly setting the port or
URL for the webhost to listen to, e.g. by setting `DOTNET_HTTP_PORTS` to a fixed port. Let's say you're hosting on `192.168.0.200` on port `31337`.

`zone` is the `zone_id` used for your domain; check out your domain on Hetzner, then take a look at the URL:
`https://dns.hetzner.com/zone/oZ123REDACTED123` shows you the `zone` `"oZ123REDACTED123"`.

`name` is the prefix you want to set - if you want to route to `home.my-domain.com`, pass `"home"`.

A complete Update URL for this scenario would look as follows:
```
http://192.168.0.200:31337/update/oZ123REDACTED123/home?v4=<ipaddr>&v6=<ip6addr>
```

Done! It should "just work" now; your Fritz!Box will create an A-record for `home.my-domain.com` to your Fritz!Box' IPv4 address and an AAAA-record
for the same name to your Fritz!Box' IPv6 address.

## Security considerations

The API-Key is stored on the device as an environment variable. Since this project has the sole purpose of pointing the whole web at your home
network, make sure that nobody can access the API-Key just by connecting.
