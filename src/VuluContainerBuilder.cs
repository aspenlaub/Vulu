using Aspenlaub.Net.GitHub.CSharp.Pegh.Components;
using Aspenlaub.Net.GitHub.CSharp.Vulu.Components;
using Aspenlaub.Net.GitHub.CSharp.Vulu.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Vulu;

public static class VuluContainerBuilder {
    public static ContainerBuilder UseVuluAndPegh(this ContainerBuilder builder) {
        builder.UsePegh("Vulu", new DummyCsArgumentPrompter());
        builder.RegisterType<ProcessRunner>().As<IProcessRunner>();
        builder.RegisterType<VuluInstaller>().As<IVuluInstaller>();

        return builder;
    }
}