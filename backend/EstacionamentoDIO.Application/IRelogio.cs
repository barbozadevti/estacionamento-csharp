namespace EstacionamentoDIO.Application;

public interface IRelogio
{
    DateTime AgoraUtc { get; }
}

public class RelogioSistema : IRelogio
{
    public DateTime AgoraUtc => DateTime.UtcNow;
}
