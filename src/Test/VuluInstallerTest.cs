using Aspenlaub.Net.GitHub.CSharp.Vulu.Components;
using Aspenlaub.Net.GitHub.CSharp.Vulu.Interfaces;
using Autofac;

namespace Aspenlaub.Net.GitHub.CSharp.Vulu.Test;

[TestClass]
public class VuluInstallerTest {
    [TestMethod]
    public void CanCallVuluInstaller() {
        IContainer container = new ContainerBuilder().UseVuluAndPegh().Build();
        var sut = new VuluInstaller(container.Resolve<IProcessRunner>());
        Assert.IsTrue(sut.Install(_ => { }, Assert.Fail));
    }
}
