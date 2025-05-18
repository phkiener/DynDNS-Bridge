# Hetzner DynDNS-Bridge for Fritz!Box

Tiny tool to automatically update DNS records in the Hetzner DNS Console when your Fritz!Box' IP address changes.

## Setup

Run the webhost either directly or through docker. Ensure that you have an environment variable `DDNSBRIDGE_API_KEY` which contains the API Key
for the Hetzner DNS Console.

In your Fritz!Box, head to the DynDNS configuration and enter the following:

- Provider: custom
- Update URL: http://<your-ip>:<your-port>/update/<zone>/<name>?v4=<ipaddr>&v6=<ip6addr>
- Domain: Not used, just enter whatever
- Username: Not used, just enter whatever
- Password: Not used, just enter whatever

Done! It should "just work" now.
