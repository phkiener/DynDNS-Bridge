using DynDNS.Network;
using Microsoft.AspNetCore.Components;

namespace DynDNS.WebInterface;

public sealed partial class Root(ICurrentAddressProvider currentAddress) : ComponentBase
{
}
