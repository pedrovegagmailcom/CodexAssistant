﻿using CodexAssistant.Compilacion;
using CodexAssistant.Interfaces;
using CodexAssistant.JSon;
using CodexAssistant.Modelo;
using CodexAssistant.Utilidades;
using Newtonsoft.Json;
using OpenAI_API;
using Prism.Commands;
using Prism.Mvvm;
using ProblemSolver.GeneradorCodigo;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CodexAssistant.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private OpenAIAPI openAIAPI = new OpenAIAPI(Secrets.OpenAIKey);

        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public RichTextBox LogRichTextBox { get; set; }

        private string _appName = "MyFirtsWPFApp";
        private string _appDescription = "crea una aplicacion en c#/wpf que pida al usuario una url, se baje el doc y la muestre en pantalla";
        private ObservableCollection<TaskItem> _taskList;
        private string _logText;
        private string _debugText;

        private IGeneradorCodigo comChatGPT;
        private string _promptAppRequest;
        private string _promptTaskRequest;

        private IOutputHandler outputHandler;
        private IProjectCreator _projectCreator;
        private string parentDirectory = @"C:\Users\pvega\pruebasNet";

        #region Propiedades Vista
        private ObservableCollection<string> _logEntries;

        public ObservableCollection<string> LogEntries
        {
            get { return _logEntries; }
            set { SetProperty(ref _logEntries, value); }
        }

        public string AppName
        {
            get => _appName;
            set => SetProperty(ref _appName, value);
        }

        public string AppDescription
        {
            get => _appDescription;
            set => SetProperty(ref _appDescription, value);
        }

        public ObservableCollection<TaskItem> TaskList
        {
            get => _taskList;
            set => SetProperty(ref _taskList, value);
        }

        private TaskItem _selectedTaskItem;
        public TaskItem SelectedTaskItem
        {
            get { return _selectedTaskItem; }
            set
            {
                _selectedTaskItem = value;
                RaisePropertyChanged("SelectedTask");
            }
        }

        public string LogText
        {
            get => _logText;
            set => SetProperty(ref _logText, value);
        }

        public string DebugText
        {
            get => _debugText;
            set => SetProperty(ref _debugText, value);
        }

        public DelegateCommand Button1Command { get; }
        public DelegateCommand Button2Command { get; }
        public DelegateCommand Button3Command { get; }
        public DelegateCommand Button4Command { get; }
        #endregion


        public MainWindowViewModel()
        {
            TaskList = new ObservableCollection<TaskItem>();
            Button1Command = new DelegateCommand(ButtonAppRequest);
            Button2Command = new DelegateCommand(ButtonTaskRequest);
            Button3Command = new DelegateCommand(Button3Execute);
            Button4Command = new DelegateCommand(Button4Execute);
            LogEntries = new ObservableCollection<string>();
            LogEntries.CollectionChanged += LogEntries_CollectionChanged;
            CargarPrompts();

           outputHandler = new OutputHandler();
            _projectCreator = new ProjectCreator(outputHandler);

            comChatGPT = new GeneradorCodigo(openAIAPI);
        }

        private void CargarPrompts()
        {
            string carpetaSalidaProyecto = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _promptAppRequest = File.ReadAllText(carpetaSalidaProyecto + "\\Prompts\\Prompt AppRequest.txt");
            _promptTaskRequest = File.ReadAllText(carpetaSalidaProyecto + "\\Prompts\\Prompt TaskRequest.txt");
        
        }

        

        private async void ButtonAppRequest()
        {
            TaskList = await SolicitarListaTareas();
        }

        private async void ButtonTaskRequest()
        {
            await EjecutarCicloTarea();
        }

        public class TaskListaAux
        {
            public ObservableCollection<TaskItem> ListPlan { get; set; }
        }

        private async Task<ObservableCollection<TaskItem>> SolicitarListaTareas()
        {
            var conversation = openAIAPI.Chat.CreateConversation();
            conversation.AppendSystemMessage(_promptAppRequest);
            conversation.AppendUserInput("AppRequest:" + AppDescription);
            string jsonListaTareas = await conversation.GetResponseFromChatbotAsync();
            jsonListaTareas = ParserJSon.ExtraerJSONAppRequest(jsonListaTareas);
            TaskListaAux taskList = JsonConvert.DeserializeObject<TaskListaAux>(jsonListaTareas);

            ObservableCollection<TaskItem> listaTareas = taskList.ListPlan;
               
            return listaTareas;
        }

        enum Estado
        {
            MandarTarea,
            Finalizar
        }

        public async Task EjecutarCicloTarea()
        {
         
            BuildResult buildResult = new BuildResult((true, "", ""));
            Estado estadoActual = Estado.MandarTarea;

            bool compilacionExitosa = false;
            Mensaje msg = new Mensaje();


            while (estadoActual != Estado.Finalizar)
            {
                switch (estadoActual)
                {
                    case Estado.MandarTarea:
                        string result = await MandarTarea(SelectedTaskItem);
                        result = ParserJSon.ExtraerJSONTaskRequest(await MandarTarea(SelectedTaskItem));
                        var comando = JsonConvert.DeserializeObject<CommandData>(result);
                        bool res = ExecComando(comando);
                        if (res) {
                            MandarContextoProjecto();
                        }
                        break;


                }
            }

            Console.WriteLine("FIN");
        }

        private void MandarContextoProjecto()
        {
            var projectfiles = ProjectUtils.GetEditableFiles(parentDirectory + "\\" + AppName);


        }

        private bool ExecComando(CommandData comando)
        {
            Comandos command = ObtenerComando(comando);

            switch (command)
            {
                case Comandos.CreateProject:

                    return EjecutarCreateProject(comando.Arguments);

                    break;
                case Comandos.GetFile:
                    
                    return EjecutarGetFile();
                    
                    break;
                case Comandos.UpdateFile:
                    
                    return EjecutarUpdateFile();
                    
                    break;
                case Comandos.AddFile:
                    
                    return EjecutarAddFile();
                    
                    break;
                case Comandos.GetContext:
                    
                    return EjecutarGetContext();
                    
                    break;
                case Comandos.AddReference:
                   
                    return EjecutarAddReference();
                    
                    break;
                default:
                    throw new ArgumentException($"El comando '{comando}' no es válido.");
            }
        }


        public bool EjecutarCreateProject(string tipo)
        {
            return _projectCreator.Create(AppName, parentDirectory, tipo);
        }

        public bool EjecutarGetFile()
        {
            return false;
        }

        public bool EjecutarUpdateFile()
        {
            return false;
        }

        public bool EjecutarAddFile()
        {
            return false;
        }

        public bool EjecutarGetContext()
        {
            return false;
        }

        public bool EjecutarAddReference()
        {
            return false;
        }

        public static Comandos ObtenerComando(CommandData commandData)
        {
            string comandoStr = commandData.Command.ToLower();

            switch (comandoStr)
            {
                case "createproject":
                    return Comandos.CreateProject;
                case "getfile":
                    return Comandos.GetFile;
                case "updatefile":
                    return Comandos.UpdateFile;
                case "addfile":
                    return Comandos.AddFile;
                case "getcontext":
                    return Comandos.GetContext;
                case "addreference":
                    return Comandos.AddReference;
                default:
                    throw new ArgumentException($"El comando '{comandoStr}' no es válido.");
            }
        }
       



        private async Task<string> MandarTarea(TaskItem selectedTaskItem)
        {
            string jsonTarea = JsonConvert.SerializeObject(selectedTaskItem);
            var conversation = openAIAPI.Chat.CreateConversation();
            conversation.AppendSystemMessage(_promptTaskRequest);
            conversation.AppendSystemMessage(AppDescription);
            conversation.AppendUserInput("TaskRequest:" + jsonTarea);
            string respuesta = await conversation.GetResponseFromChatbotAsync();
            return respuesta;
        }

        private void Button3Execute()
        {
            // Implementar la lógica para el botón 3 aquí
        }

        private void Button4Execute()
        {
            // Implementar la lógica para el botón 4 aquí
        }

        private void LogEntries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (LogRichTextBox != null)
                {
                    string formattedJson = ParserJSon.FormatJsonString(e.NewItems[0].ToString());
                    LogRichTextBox.Document.Blocks.Add(new Paragraph(new Run(formattedJson)));
                    LogRichTextBox.ScrollToEnd();
                }
            }
        }

        
    }
}
