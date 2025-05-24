# Hetzner DynDNS-Bridge for Fritz!Box

Tiny tool to automatically update DNS records in the Hetzner DNS Console when your Fritz!Box' IP address changes.
Could work for other routers as well, provided they allow you to define a custom DynDNS server.

## Setup

For the host itself, you need to configure the following environment variables:
 - `DDNSBRIDGE_APIKEY` - your Hetzner DNS Console API key
 - `DDNSBRIDGE_DOMAINS` - a semicolon-separated list of domain names to configure, e.g. `foo.acme.com;bar.acme.com`
 - `DOTNET_URLS` - optional list of URLs to listen to, e.g. `http://192.168.0.200:8080`

Run the webhost either directly or through docker and make sure it's accessible from within your network, but not from the outside.
Check out the [docker-compose.yaml](./build/docker-compose.yaml) if you want to work with `docker compose`.

In your router, head to the DynDNS configuration and enter the following:

- Provider: custom
- Update URL: `http://<your-ip>:<your-port>/_update?v4=<ipaddr>&v6=<ip6addr>`
- Domain: Not used, just enter whatever
- Username: Not used, just enter whatever
- Password: Not used, just enter whatever

Note that if you ever _change_ `DDNSBRIDGE_DOMAINS`, any previously created entries will *not* be deleted automatically. This tool only looks
at the names you explicitly configure; everything else is ignored.

## Example

Let's say you want to host the DynDNS bridge on a device in your network with the static IP address `192.168.0.200`. You'd configure the following
environment variables before starting the host:

| Key                | Value                                            |
|--------------------|--------------------------------------------------|
| DDNSBRIDGE_APIKEY  | MySuperSecretKey                                 |
| DDNSBRIDGE_DOMAINS | public.me-at-home.com;travel.blog.me-at-home.com |
| DOTNET_URLS        | http://192.168.0.200:8080                        |

Once the host is up and running, head to your router and configure DynDNS. Set the URL to
`http://192.168.0.200:8080/_refresh?v4=<ipaddr>&v6=<ip6addr>`. Put some placeholders for Domain, Username and Password - we don't need those.

That's it! If done correctly, it should all just work right away. You should see two DNS entries each for `public.me-at-home.com` and for
`travel.blog.me-at-home.com`: an `A` record pointing at your public IPv4 address and an `AAAA` record pointing at your public IPv6 address.

## Security considerations

The API-Key is configured via environment variable. Since this project has the sole purpose of pointing the whole web at your home network,
make sure that nobody can access the API-Key just by connecting.
