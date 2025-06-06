﻿using System.Text.Json;
using System.Text.Json.Nodes;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Vulu.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Vulu.Components;

public class VuluInstaller(IProcessRunner processRunner) : IVuluInstaller {
    private readonly IFolder _WorkingFolder = new Folder(Path.GetTempPath()).SubFolder(nameof(VuluInstaller));

    public bool Install(Action<string> onMessageOut, Action<string> onErrorMessageOut) {
        _WorkingFolder.CreateIfNecessary();
        return CheckIfNodeJsIsInstalled(onMessageOut, onErrorMessageOut) &&
               CheckIfNpmIsInstalled(onMessageOut, onErrorMessageOut) &&
               InstallAngularCliIfNecessary(onMessageOut, onErrorMessageOut) &&
               InstallYarnIfNecessary(onMessageOut, onErrorMessageOut) &&
               InstallNpxIfNecessary(onMessageOut, onErrorMessageOut);
    }

    private bool CheckIfNodeJsIsInstalled(Action<string> onMessageOut, Action<string> onErrorMessageOut) {
        onMessageOut("Check if node.js is installed..");
        var errorsAndInfos = new ErrorsAndInfos();
        string executable = _WorkingFolder.FullName + @"\nodejs.cmd";
        File.WriteAllText(executable, "node -v");
        processRunner.RunProcess(executable, "", _WorkingFolder, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            errorsAndInfos.Errors.ToList().ForEach(onErrorMessageOut);
            onMessageOut("Download node.js at https://nodejs.org/en");
            return false;
        }

        var versionInfos = errorsAndInfos.Infos.Where(i => i.StartsWith("v", StringComparison.InvariantCulture)).ToList();
        if (versionInfos.Count != 1) {
            onErrorMessageOut("Version info not found or not unique");
            return false;
        }

        if (!Version.TryParse(versionInfos[0].Substring(1), out Version? version)) {
            onErrorMessageOut("Version could not be parsed: " + versionInfos[0]);
            return false;
        }

        if (version.Major < 22) {
            onErrorMessageOut("Version is too low: " + version);
            return false;
        }

        onMessageOut("Done");
        return true;
    }

    private bool CheckIfNpmIsInstalled(Action<string> onMessageOut, Action<string> onErrorMessageOut) {
        onMessageOut("Check if npm is installed..");
        var errorsAndInfos = new ErrorsAndInfos();
        string executable = _WorkingFolder.FullName + @"\npm_version.cmd";
        File.WriteAllText(executable, "npm version --silent");
        processRunner.RunProcess(executable, "", _WorkingFolder, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            errorsAndInfos.Errors.ToList().ForEach(onErrorMessageOut);
            onMessageOut("Download npm at https://nodejs.org/en");
            return false;
        }

        var jsonList = errorsAndInfos.Infos.ToList();
        jsonList.RemoveRange(0, 2);
        string jsonString = string.Join(' ', jsonList);
        jsonString = jsonString.Replace("{   ", "{   \"");
        jsonString = jsonString.Replace(": '", "\": \"");
        jsonString = jsonString.Replace("' }", "\" }");
        jsonString = jsonString.Replace("',   ", "\", \"");
        JsonNode? node = JsonSerializer.Deserialize<JsonNode>(jsonString);
        if (node?["npm"] is null) {
            onErrorMessageOut("Could not find npm version");
            return false;
        }

        if (!Version.TryParse(node["npm"]?.ToString(), out Version? version)) {
            onErrorMessageOut("Version could not be parsed: " + node["npm"]);
            return false;
        }

        if (version.Major < 10) {
            onErrorMessageOut("Version is too low: " + version);
            return false;
        }

        onMessageOut("Done");
        return true;
    }

    private bool InstallAngularCliIfNecessary(Action<string> onMessageOut, Action<string> onErrorMessageOut) {
        onMessageOut("Install angular cli if necessary..");
        var errorsAndInfos = new ErrorsAndInfos();
        string ngVersionExecutable = _WorkingFolder.FullName + @"\ng_version.cmd";
        File.WriteAllText(ngVersionExecutable, "ng version");
        processRunner.RunProcess(ngVersionExecutable, "", _WorkingFolder, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            errorsAndInfos = new ErrorsAndInfos();
            string ngInstallExecutable = _WorkingFolder.FullName + @"\ng_install.cmd";
            File.WriteAllText(ngInstallExecutable, "npm i -g @angular/cli");
            processRunner.RunProcess(ngInstallExecutable, "", _WorkingFolder, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                errorsAndInfos.Errors.ToList().ForEach(onErrorMessageOut);
                onErrorMessageOut("Angular cli could not be installed");
                return false;
            }

            errorsAndInfos = new ErrorsAndInfos();
            processRunner.RunProcess(ngVersionExecutable, "", _WorkingFolder, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                onErrorMessageOut("Angular cli could not be installed");
                return false;
            }
        }

        string? versionInfo = errorsAndInfos.Infos.FirstOrDefault(i => i.StartsWith("Angular CLI:"));
        if (versionInfo is null) {
            onErrorMessageOut("Could not find angular cli version");
            return false;
        }

        versionInfo = versionInfo.Substring(versionInfo.IndexOf(':') + 1);
        if (!Version.TryParse(versionInfo, out Version? version)) {
            onErrorMessageOut("Version could not be parsed: " + versionInfo);
            return false;
        }

        if (version.Major < 19) {
            onErrorMessageOut("Version is too low: " + version);
            return false;
        }

        onMessageOut("Done");
        return true;
    }

    private bool InstallYarnIfNecessary(Action<string> onMessageOut, Action<string> onErrorMessageOut) {
        onMessageOut("Install yarn if necessary..");

        var errorsAndInfos = new ErrorsAndInfos();
        string yarnVersionExecutable = _WorkingFolder.FullName + @"\yarn_version.cmd";
        File.WriteAllText(yarnVersionExecutable, "yarn -v");
        processRunner.RunProcess(yarnVersionExecutable, "", _WorkingFolder, errorsAndInfos);
        if (errorsAndInfos.AnyErrors()) {
            errorsAndInfos = new ErrorsAndInfos();
            string yarnInstallExecutable = _WorkingFolder.FullName + @"\yarn_install.cmd";
            File.WriteAllText(yarnInstallExecutable, "npm install --global yarn");
            processRunner.RunProcess(yarnInstallExecutable, "", _WorkingFolder, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                errorsAndInfos.Errors.ToList().ForEach(onErrorMessageOut);
                onErrorMessageOut("Yarn could not be installed");
                return false;
            }

            errorsAndInfos = new ErrorsAndInfos();
            processRunner.RunProcess(yarnVersionExecutable, "", _WorkingFolder, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                onErrorMessageOut("Yarn could not be installed");
                return false;
            }
        }

        string versionInfo = errorsAndInfos.Infos[2];
        if (!Version.TryParse(versionInfo, out Version? version)) {
            onErrorMessageOut("Version could not be parsed: " + versionInfo);
            return false;
        }

        if (version.Major < 1 || version is { Major: 1, Minor: < 22 }) {
            onErrorMessageOut("Version is too low: " + version);
            return false;
        }

        onMessageOut("Done");
        return true;
    }

    private bool InstallNpxIfNecessary(Action<string> onMessageOut, Action<string> onErrorMessageOut) {
        onMessageOut("Install npx if necessary..");

        if (!IsNpxInstalled()) {
            var errorsAndInfos = new ErrorsAndInfos();
            string npxInstallExecutable = _WorkingFolder.FullName + @"\npx_install.cmd";
            File.WriteAllText(npxInstallExecutable, "npm i -g npx");
            processRunner.RunProcess(npxInstallExecutable, "", _WorkingFolder, errorsAndInfos);
            if (errorsAndInfos.AnyErrors()) {
                errorsAndInfos.Errors.ToList().ForEach(onErrorMessageOut);
                onErrorMessageOut("Npx could not be installed");
                return false;
            }
            if (!IsNpxInstalled()) {
                onErrorMessageOut("Npx could not be installed");
                return false;
            }
        }

        onMessageOut("Done");
        return true;
    }

    private bool IsNpxInstalled() {
        var errorsAndInfos = new ErrorsAndInfos();
        string npxWhereExecutable = _WorkingFolder.FullName + @"\npx_where.cmd";
        File.WriteAllText(npxWhereExecutable, "where npx.cmd");
        processRunner.RunProcess(npxWhereExecutable, "", _WorkingFolder, errorsAndInfos);
        bool installed = !errorsAndInfos.AnyErrors();
        if (!installed) {
            return installed;
        }

        IFolder? folder = new Folder(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
            .SubFolder("npm");
        string npxCmdExpectedLocation = folder.FullName + @"\npx.cmd";
        installed = errorsAndInfos.Infos.Any(i
            => i.Contains(npxCmdExpectedLocation, StringComparison.InvariantCultureIgnoreCase));

        return installed;
    }
}
