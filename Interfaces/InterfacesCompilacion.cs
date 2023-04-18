using System.Diagnostics;

namespace CodexAssistant.Interfaces
{
    public interface IProjectCreator
    {
        bool Create(string appName, string parentDirectory, string tipo);
    }

    public interface IProjectBuilder
    {
        (bool success, string output, string error) Build(string projectDirectory);
    }

    public interface IOutputHandler
    {
        void HandleOutput(object sender, DataReceivedEventArgs data);
        void HandleError(object sender, DataReceivedEventArgs data);
    }

    public interface ITestProjectCreator
    {
        void Create(string testProjectName, string parentDirectory, string mainProjectPath);
    }
}
