﻿using System.Diagnostics;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Entities;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Extensions;
using Aspenlaub.Net.GitHub.CSharp.Pegh.Interfaces;
using Aspenlaub.Net.GitHub.CSharp.Vulu.Interfaces;
using Microsoft.Extensions.Logging;

namespace Aspenlaub.Net.GitHub.CSharp.Vulu.Components;

public class ProcessRunner(ISimpleLogger simpleLogger, IMethodNamesFromStackFramesExtractor methodNamesFromStackFramesExtractor)
    : IProcessRunner {
    public void RunProcess(string executableFileName, string arguments, IFolder workingFolder, IErrorsAndInfos errorsAndInfos) {
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(ProcessRunner)))) {
            IList<string>? methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            simpleLogger.LogInformationWithCallStack($"Running {executableFileName} with arguments {arguments} in {workingFolder.FullName}", methodNamesFromStack);
            using (Process process = CreateProcess(executableFileName, arguments, workingFolder)) {
                try {
                    var outputWaitHandle = new AutoResetEvent(false);
                    var errorWaitHandle = new AutoResetEvent(false);
                    process.OutputDataReceived += (_, e) => OnDataReceived(e, outputWaitHandle, errorsAndInfos.Infos, LogLevel.Information);
                    process.ErrorDataReceived += (_, e) => OnDataReceived(e, errorWaitHandle, errorsAndInfos.Errors, LogLevel.Error);
                    process.Exited += (_, _) => simpleLogger.LogInformationWithCallStack("Process exited", methodNamesFromStack);
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit(int.MaxValue);
                    outputWaitHandle.WaitOne();
                    errorWaitHandle.WaitOne();
                } catch (Exception e) {
                    errorsAndInfos.Errors.Add($"Process failed: {e.Message}");
                    return;
                }
            }

            simpleLogger.LogInformationWithCallStack("Process completed", methodNamesFromStack);
        }
    }

    private void OnDataReceived(DataReceivedEventArgs e, EventWaitHandle waitHandle, ICollection<string> messages, LogLevel logLevel) {
        if (e.Data == null) {
            waitHandle.Set();
            return;
        }

        messages.Add(e.Data);
        using (simpleLogger.BeginScope(SimpleLoggingScopeId.Create(nameof(OnDataReceived)))) {
            IList<string>? methodNamesFromStack = methodNamesFromStackFramesExtractor.ExtractMethodNamesFromStackFrames();
            switch (logLevel) {
                case LogLevel.Warning:
                    simpleLogger.LogWarningWithCallStack(e.Data, methodNamesFromStack);
                    break;
                case LogLevel.Error:
                    simpleLogger.LogErrorWithCallStack(e.Data, methodNamesFromStack);
                    break;
                case LogLevel.Trace:
                case LogLevel.Debug:
                case LogLevel.Information:
                case LogLevel.Critical:
                case LogLevel.None:
                default:
                    simpleLogger.LogInformationWithCallStack(e.Data, methodNamesFromStack);
                    break;
            }
        }
    }

    private static Process CreateProcess(string executableFileName, string arguments, IFolder workingFolder) {
        return new() {
            StartInfo = {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = executableFileName,
                Arguments = arguments,
                WorkingDirectory = workingFolder.FullName,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
    }
}