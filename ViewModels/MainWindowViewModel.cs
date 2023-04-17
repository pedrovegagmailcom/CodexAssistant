using CodexAssistant.Compilacion;
using CodexAssistant.Interfaces;
using CodexAssistant.JSon;
using CodexAssistant.Modelo;
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
            jsonListaTareas = ParserJSon.ExtraerJSON(jsonListaTareas);
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
            IOutputHandler outputHandler = new OutputHandler();
            IProjectCreator _projectCreator = new ProjectCreator(outputHandler);


            //string parentDirectory = @"C:\Users\pvega\pruebasNet";
            //_projectCreator.Create(AppName, parentDirectory);
            //string mainProjectPath = Path.Combine(parentDirectory, appName, $"{appName}.csproj");


            BuildResult buildResult = new BuildResult((true, "", ""));
            Estado estadoActual = Estado.MandarTarea;

            bool compilacionExitosa = false;
            Mensaje msg = new Mensaje();


            while (estadoActual != Estado.Finalizar)
            {
                switch (estadoActual)
                {
                    case Estado.MandarTarea:
                        var result = await MandarTarea(SelectedTaskItem);
                        
                        break;


                }
            }

            Console.WriteLine("¡La función y los tests se ejecutaron correctamente!");
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
