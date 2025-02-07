namespace Aspenlaub.Net.GitHub.CSharp.Vulu.Interfaces;

public interface IVuluInstaller {
    bool Install(Action<string> onMessageOut, Action<string> onErrorMessageOut);
}