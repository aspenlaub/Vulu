using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Vulu.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Vulu.Components;

public class VuluInstaller(IProcessRunner processRunner) : IVuluInstaller {
    private readonly IFolder _WorkingFolder = new Folder(Path.GetTempPath()).SubFolder(nameof(VuluInstaller));

    public bool Install(Action<string> onMessageOut, Action<string> onErrorMessageOut) {
        return CheckIfNodeJsIsInstalled(onMessageOut, onErrorMessageOut)
            && InstallNpmIfNecessary(onMessageOut, onErrorMessageOut)
            && InstallAngularCliIfNecessary(onMessageOut, onErrorMessageOut)
            && InstallYarnIfNecessary(onMessageOut, onErrorMessageOut);
    }

    private bool CheckIfNodeJsIsInstalled(Action<string> onMessageOut, Action<string> onErrorMessageOut) {
        onMessageOut("Check if node.js is installed..");
        var errorsAndInfos = new ErrorsAndInfos();
        processRunner.RunProcess("node", "-v", _WorkingFolder, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            errorsAndInfos.Errors.ToList().ForEach(onErrorMessageOut);
            onMessageOut("Download node.js at https://nodejs.org/en");
            return false;
        }
        onMessageOut("Done");
        return true;
    }

    private bool InstallNpmIfNecessary(Action<string> onMessageOut, Action<string> onErrorMessageOut) {
        onMessageOut("Install npm if necessary..");
        onMessageOut("Done");
        return true;
    }

    private bool InstallAngularCliIfNecessary(Action<string> onMessageOut, Action<string> onErrorMessageOut) {
        onMessageOut("Install angular cli if necessary..");
        onMessageOut("Done");
        return true;
    }

    private bool InstallYarnIfNecessary(Action<string> onMessageOut, Action<string> onErrorMessageOut) {
        onMessageOut("Install yarn if necessary..");
        onMessageOut("Done");
        return true;
    }

}
