using Netrix.Models.Modes;
using Netrix.Servers;

namespace Netrix.Interfaces;

public interface IModeController : IController
{
    public ModeFeature Features { get; }

    public Task StartAsync(Socks5Server server, Mode mode);
}