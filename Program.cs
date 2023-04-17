using CodexAssistant.Compilacion;
using CodexAssistant.Interfaces;
using Newtonsoft.Json;
using OpenAI_API;
using ProblemSolver.GeneradorCodigo;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class Mensaje
{
    public string CodigoRealizado { get; set; }
    public string CodigoCorregido { get; set; }
    public string Comentarios { get; set; }
    
}

public class BuildResult
{
    public bool Success { get; set; }
    public string Output { get; set; }
    public string Error { get; set; }



    public BuildResult((bool success, string output, string error) resBuild)
    {
        Success = resBuild.success;
        Output = resBuild.output;
        Error = resBuild.error;

    }
}


class Program
{

    

    public void Main()
    {
        OpenAIAPI openAIAPI = new OpenAIAPI("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");
        var api = openAIAPI;

        
       

        IGeneradorCodigo generadorCodigo = new GeneradorCodigo(api);

        // Crear instancias de ConsoleProjectCreator, ConsoleProjectBuilder y NUnitTestProjectCreator
        IOutputHandler outputHandler = new OutputHandler();
        IProjectCreator projectCreator = new ProjectCreator(outputHandler);
        IProjectBuilder projectBuilder = new ProjectBuilder(outputHandler);
        ITestProjectCreator testProjectCreator = new NUnitTestProjectCreator(outputHandler);

        var cicloProceso = new CicloProceso(generadorCodigo, projectCreator, projectBuilder, testProjectCreator);
        //await cicloProceso.EjecutarCicloAsync(problema, outputHandler);
    }
}

class CicloProceso
{
    private readonly IGeneradorCodigo _generadorCodigo;
    private readonly IProjectCreator _projectCreator;
    private readonly IProjectBuilder _projectBuilder;
    private readonly ITestProjectCreator _testProjectCreator;

    public CicloProceso(IGeneradorCodigo generadorCodigo, IProjectCreator projectCreator, IProjectBuilder projectBuilder, ITestProjectCreator testProjectCreator)
    {
        _generadorCodigo = generadorCodigo;
        _projectCreator = projectCreator;
        _projectBuilder = projectBuilder;
        _testProjectCreator = testProjectCreator;
    }
    //public async Task EjecutarCicloAsync(string problema, IOutputHandler outputHandler)
    //{
    //    string msgerror = "\nParece que este codigo tiene errores, corrigelos y devuelve el resultado en el json : \n";
    //    // Crear proyectos de consola y prueba
    //    string appName = "FuncionApp";
    //    string testProjectName = "FuncionApp.Tests";
    //    string parentDirectory = @"C:\Users\pvega\pruebasNet";

    //    Console.ForegroundColor = ConsoleColor.Blue;
    //    Console.WriteLine(problema + "\n");

    //    Console.ForegroundColor = ConsoleColor.DarkGray;
    //    _projectCreator.Create(appName, parentDirectory);
    //    string mainProjectPath = Path.Combine(parentDirectory, appName, $"{appName}.csproj");
    //    _testProjectCreator.CreateTestProject(testProjectName, parentDirectory, mainProjectPath);

    //    BuildResult buildResult = new BuildResult((true,"",""));
    //    Estado estadoActual = Estado.GenerarFuncion;
    //    string codigoFuncion = null;
    //    string codigoTests = null;
    //    bool compilacionExitosa = false;
    //    Mensaje msg = new Mensaje();


    //    while (estadoActual != Estado.Finalizar)
    //    {
    //        switch (estadoActual)
    //        {
    //            case Estado.GenerarFuncion:
    //                string mensajeParaChatbot;
    //                if (buildResult.Success == false)
    //                {
    //                    mensajeParaChatbot = problema + codigoFuncion + msgerror + buildResult.Output;
    //                }
    //                else
    //                {
    //                    mensajeParaChatbot = problema;
    //                }
    //                Console.ForegroundColor = ConsoleColor.Blue;
    //                Console.WriteLine("Generando Codigo. Esperando IA...");
    //                var chatbotResponse = await _generadorCodigo.GenerarCodigoFuncionAsync(mensajeParaChatbot);
    //                Console.ForegroundColor = ConsoleColor.DarkGray;
    //                Console.WriteLine(chatbotResponse);
    //                try
    //                {
    //                    msg = JsonConvert.DeserializeObject<Mensaje>(chatbotResponse);
    //                    if (msg.CodigoCorregido != null && msg.CodigoCorregido.Count() > 0)
    //                    {
    //                        codigoFuncion = msg.CodigoCorregido;
    //                    }
    //                    else
    //                    {
    //                        codigoFuncion = msg.CodigoRealizado;
    //                    }
    //                    estadoActual = Estado.CompilarFuncion;
    //                }
    //                catch (Exception e)
    //                {
    //                    Console.ForegroundColor = ConsoleColor.Red;
    //                    Console.WriteLine(e.Message );
    //                    buildResult.Success = false;
    //                    buildResult.Output= e.Message;
    //                    estadoActual = Estado.GenerarFuncion;
    //                }


    //                break;

    //            case Estado.CompilarFuncion:
    //                string mainProjectFile = Path.Combine(parentDirectory, appName, "Program.cs");
    //                File.WriteAllText(mainProjectFile, codigoFuncion);
    //                var resBuild = _projectBuilder.Build(Path.Combine(parentDirectory, appName));
    //                buildResult = new BuildResult(resBuild);
    //                estadoActual = buildResult.Success ? Estado.GenerarTests : Estado.GenerarFuncion;
    //                if (buildResult.Success)
    //                {
    //                    Console.ForegroundColor = ConsoleColor.Green;
    //                    Console.WriteLine("Codigo generado correcto.");
    //                }
    //                break;



    //            case Estado.GenerarTests:
    //                if (buildResult.Success == false)
    //                {
    //                    mensajeParaChatbot = codigoTests + msgerror + buildResult.Output;
    //                }
    //                else
    //                {
    //                    mensajeParaChatbot = problema;
    //                }
    //                Console.ForegroundColor = ConsoleColor.Blue;
    //                Console.WriteLine("Generando Tests. Esperando IA...");
    //                chatbotResponse = await _generadorCodigo.GenerarCodigoTestsAsync(mensajeParaChatbot, codigoFuncion);
    //                Console.ForegroundColor = ConsoleColor.White;
    //                Console.WriteLine(chatbotResponse);

    //                try
    //                {
    //                    msg = JsonConvert.DeserializeObject<Mensaje>(chatbotResponse);
    //                    codigoTests = msg.CodigoRealizado;
    //                    estadoActual = Estado.CompilarTests;
    //                }
    //                catch (Exception e)
    //                {
    //                    Console.ForegroundColor = ConsoleColor.Red;
    //                    Console.WriteLine(e.Message);
    //                    buildResult.Success = false;
    //                    buildResult.Output = e.Message;
    //                    estadoActual = Estado.GenerarTests;
    //                }

    //                break;

    //            case Estado.CompilarTests:
    //                string testProjectFile = Path.Combine(parentDirectory, testProjectName, "UnitTest1.cs");
    //                File.WriteAllText(testProjectFile, codigoTests);

    //                resBuild = _projectBuilder.Build(Path.Combine(parentDirectory, testProjectName));
    //                buildResult = new BuildResult(resBuild);
    //                estadoActual = buildResult.Success ? Estado.EjecutarTests : Estado.GenerarTests;
    //                break;

    //            case Estado.EjecutarTests:


    //                // Ejecutar pruebas y verificar el resultado
    //                ProblemSolver.ITestRunner testRunner = new NUnitTestRunner(outputHandler);
    //                bool testsExitosos = testRunner.RunTests(Path.Combine(parentDirectory, testProjectName));
    //                estadoActual = testsExitosos ? Estado.Finalizar : Estado.ReportarFallo;
    //                break;


    //        }
    //    }

    //    Console.WriteLine("¡La función y los tests se ejecutaron correctamente!");
    //}
    string msgerror = "\nParece que este codigo tiene errores, corrigelos y devuelve el resultado en el json : \n";
    public async Task EjecutarCicloAsync(string problema, IOutputHandler outputHandler)
    {
        
        string appName = "FuncionApp";
        string testProjectName = "FuncionApp.Tests";
        string parentDirectory = @"C:\Users\pvega\pruebasNet";

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(problema + "\n");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        _projectCreator.Create(appName, parentDirectory);
        string mainProjectPath = Path.Combine(parentDirectory, appName, $"{appName}.csproj");
        _testProjectCreator.CreateTestProject(testProjectName, parentDirectory, mainProjectPath);

        BuildResult buildResult = new BuildResult((true, "", ""));
        Estado estadoActual = Estado.GenerarFuncion;
        string codigoFuncion = null;
        string codigoTests = null;

        while (estadoActual != Estado.Finalizar)
        {
            switch (estadoActual)
            {
                
                case Estado.GenerarFuncion:
                    estadoActual = await GenerarFuncionAsync(problema, buildResult, codigoFuncion);
                    break;

                case Estado.CompilarFuncion:
                    estadoActual = CompilarFuncion(parentDirectory, appName, codigoFuncion);
                    break;

                case Estado.GenerarTests:
                    estadoActual = await GenerarTestsAsync(problema, buildResult, codigoFuncion);
                    break;

                case Estado.CompilarTests:
                    estadoActual = CompilarTests(parentDirectory, testProjectName, codigoTests);
                    break;

                case Estado.EjecutarTests:
                    estadoActual = EjecutarTests(outputHandler, parentDirectory, testProjectName);
                    break;
            }
        }

        Console.WriteLine("¡La función y los tests se ejecutaron correctamente!");
    }

    private async Task<Estado> GenerarFuncionAsync(string problema, BuildResult buildResult, string codigoFuncion)
    {
        string mensajeParaChatbot;
        if (buildResult.Success == false)
        {
            mensajeParaChatbot = problema + codigoFuncion + msgerror + buildResult.Output;
        }
        else
        {
            mensajeParaChatbot = problema;
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Generando Codigo. Esperando IA...");
        var chatbotResponse = await _generadorCodigo.GenerarCodigoFuncionAsync(mensajeParaChatbot);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(chatbotResponse);
        try
        {
            Mensaje msg = JsonConvert.DeserializeObject<Mensaje>(chatbotResponse);
            if (msg.CodigoCorregido != null && msg.CodigoCorregido.Count() > 0)
            {
                codigoFuncion = msg.CodigoCorregido;
            }
            else
            {
                codigoFuncion = msg.CodigoRealizado;
            }
            return Estado.CompilarFuncion;
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            buildResult.Success = false;
            buildResult.Output = e.Message;
            return Estado.GenerarFuncion;
        }
    }

    private Estado CompilarFuncion(string parentDirectory, string appName, string codigoFuncion)
    {
        string mainProjectFile = Path.Combine(parentDirectory, appName, "Program.cs");
        File.WriteAllText(mainProjectFile, codigoFuncion);
        var resBuild = _projectBuilder.Build(Path.Combine(parentDirectory, appName));
        BuildResult buildResult = new BuildResult(resBuild);
        if (buildResult.Success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Codigo generado correcto.");
            return Estado.GenerarTests;
        }
        else
        {
            return Estado.GenerarFuncion;
        }
    }

    private async Task<Estado> GenerarTestsAsync(string problema, BuildResult buildResult, string codigoFuncion)
    {
        string mensajeParaChatbot;
        if (buildResult.Success == false)
        {
            mensajeParaChatbot = codigoFuncion + msgerror + buildResult.Output;
        }
        else
        {
            mensajeParaChatbot = problema;
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Generando Tests. Esperando IA...");
        var chatbotResponse = await _generadorCodigo.GenerarCodigoTestsAsync(mensajeParaChatbot, codigoFuncion);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(chatbotResponse);

        try
        {
            Mensaje msg = JsonConvert.DeserializeObject<Mensaje>(chatbotResponse);
            //codigoTests = msg.CodigoRealizado;
            return Estado.CompilarTests;
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            buildResult.Success = false;
            buildResult.Output = e.Message;
            return Estado.GenerarTests;
        }
    }

    private Estado CompilarTests(string parentDirectory, string testProjectName, string codigoTests)
    {
        string testProjectFile = Path.Combine(parentDirectory, testProjectName, "UnitTest1.cs");
        File.WriteAllText(testProjectFile, codigoTests);

        var resBuild = _projectBuilder.Build(Path.Combine(parentDirectory, testProjectName));
        BuildResult buildResult = new BuildResult(resBuild);
        if (buildResult.Success)
        {
            return Estado.EjecutarTests;
        }
        else
        {
            return Estado.GenerarTests;
        }
    }

    private Estado EjecutarTests(IOutputHandler outputHandler, string parentDirectory, string testProjectName)
    {
        // Ejecutar pruebas y verificar el resultado
        ITestRunner testRunner = new NUnitTestRunner(outputHandler);
        bool testsExitosos = testRunner.RunTests(Path.Combine(parentDirectory, testProjectName));
        if (testsExitosos)
        {
            return Estado.Finalizar;
        }
        else
        {
            return Estado.GenerarTests; // Si los tests fallan, podrías considerar volver a generar los tests o reportar el fallo
        }
    }



    enum Estado
    {
        GenerarFuncion,
        CompilarFuncion,
        GenerarTests,
        CompilarTests,
        EjecutarTests,
        Finalizar,
        ReportarFallo
    }
}
