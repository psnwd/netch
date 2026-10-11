namespace Netrix.Interfaces;

public interface IController
{
    public string Name { get; }

    public Task StopAsync();
}