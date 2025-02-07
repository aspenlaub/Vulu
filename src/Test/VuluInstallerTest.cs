using Aspenlaub.Net.GitHub.CSharp.Vulu.Components;

namespace Aspenlaub.Net.GitHub.CSharp.Vulu.Test;

[TestClass]
public sealed class VuluInstallerTest {
    [TestMethod]
    public void CanCallVuluInstaller() {
        var sut = new VuluInstaller();
        sut.Install(_ => { }, Assert.Fail);
    }
}
