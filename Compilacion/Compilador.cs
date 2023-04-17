using CodexAssistant.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace CodexAssistant.Compilacion
{
    

    public class OutputHandler : IOutputHandler
    {
        public void HandleOutput(object sender, DataReceivedEventArgs data)
        {
            Console.WriteLine(data.Data);
        }

        public void HandleError(object sender, DataReceivedEventArgs data)
        {
            Console.WriteLine(data.Data);
        }
    }

    public class ProjectCreator : IProjectCreator
    {
        private readonly IOutputHandler _outputHandler;

        public ProjectCreator(IOutputHandler outputHandler)
        {
            _outputHandler = outputHandler;
        }

        public void Create(string appName, string parentDirectory)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"new console -o {appName} --verbosity quiet",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = parentDirectory
            };

            ExecuteProcess(processStartInfo);
        }

        private void ExecuteProcess(ProcessStartInfo processStartInfo)
        {
            using (Process process = new Process { StartInfo = processStartInfo })
            {
                process.OutputDataReceived += _outputHandler.HandleOutput;
                process.ErrorDataReceived += _outputHandler.HandleError;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }
    }

    public class ProjectBuilder : IProjectBuilder
    {
        private readonly IOutputHandler _outputHandler;

        public ProjectBuilder(IOutputHandler outputHandler)
        {
            _outputHandler = outputHandler;
        }

        public (bool success, string output, string error) Build(string projectDirectory)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "build --verbosity quiet",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = projectDirectory
            };

            return ExecuteProcess(processStartInfo);
        }

        private (bool success, string output, string error) ExecuteProcess(ProcessStartInfo processStartInfo)
        {
            StringBuilder outputData = new StringBuilder();
            StringBuilder errorData = new StringBuilder();

            using (Process process = new Process { StartInfo = processStartInfo })
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    _outputHandler.HandleOutput(sender, e);
                    outputData.AppendLine(e.Data);
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    _outputHandler.HandleError(sender, e);
                    errorData.AppendLine(e.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                return (process.ExitCode == 0, outputData.ToString(), errorData.ToString());
            }
        }
    }


   

    public class NUnitTestProjectCreator : ITestProjectCreator
    {
        private readonly IOutputHandler _outputHandler;
        private string _parentDirectory;

        public NUnitTestProjectCreator(IOutputHandler outputHandler)
        {
            _outputHandler = outputHandler;
        }

        public void CreateTestProject(string testProjectName, string parentDirectory, string mainProjectPath)
        {
            CreateNUnitTestProject(testProjectName, parentDirectory);
            AddReferenceToMainProject(testProjectName, mainProjectPath);
        }

        private void CreateNUnitTestProject(string testProjectName, string parentDirectory)
        {
            _parentDirectory = parentDirectory;
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"new nunit -o {testProjectName}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = parentDirectory
            };

            ExecuteProcess(processStartInfo);
        }

        private void AddReferenceToMainProject(string testProjectName, string mainProjectPath)
        {
            string testProjectDirectory = Path.Combine(_parentDirectory, $"{testProjectName}");
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"add {testProjectDirectory} reference {mainProjectPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = _parentDirectory
            };

            ExecuteProcess(processStartInfo);
        }


        private void ExecuteProcess(ProcessStartInfo processStartInfo)
        {
            using (Process process = new Process { StartInfo = processStartInfo })
            {
                process.OutputDataReceived += _outputHandler.HandleOutput;
                process.ErrorDataReceived += _outputHandler.HandleError;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }
        }
    }

    public interface ITestRunner
    {
        bool RunTests(string testProjectDirectory);
    }

    public class NUnitTestRunner : ITestRunner
    {
        private readonly IOutputHandler _outputHandler;

        public NUnitTestRunner(IOutputHandler outputHandler)
        {
            _outputHandler = outputHandler;
        }

        public bool RunTests(string testProjectDirectory)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "test --verbosity quiet",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = testProjectDirectory
            };

            return ExecuteProcess(processStartInfo);
        }


        private bool ExecuteProcess(ProcessStartInfo processStartInfo)
        {
            using (Process process = new Process { StartInfo = processStartInfo })
            {
                process.OutputDataReceived += _outputHandler.HandleOutput;
                process.ErrorDataReceived += _outputHandler.HandleError;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                return process.ExitCode == 0;
            }
        }

    }
}
