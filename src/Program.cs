using Aspenlaub.Net.GitHub.CSharp.Vulu.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Vulu;

public class Program {
    public static void Main() {
        IContainer container = new ContainerBuilder().UseVuluAndPegh().Build();
        IVuluInstaller installer = container.Resolve<IVuluInstaller>();
        installer.Install(Console.WriteLine, Console.Error.WriteLine);
    }
}