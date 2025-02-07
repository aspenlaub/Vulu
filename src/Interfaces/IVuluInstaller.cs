namespace Aspenlaub.Net.GitHub.CSharp.Vulu.Interfaces;

public interface IVuluInstaller {
    void Install(Action<string> onMessageOut, Action<string> onErrorMessageOut);
}