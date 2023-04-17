using System.Diagnostics;

namespace CodexAssistant.Interfaces
{
    public interface IProjectCreator
    {
        void Create(string appName, string parentDirectory);
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
        void CreateTestProject(string testProjectName, string parentDirectory, string mainProjectPath);
    }
}
