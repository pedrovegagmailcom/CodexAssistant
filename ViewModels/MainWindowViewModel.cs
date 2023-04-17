using CodexAssistant.JSon;
using CodexAssistant.Modelo;
using Newtonsoft.Json;
using OpenAI_API;
using Prism.Commands;
using Prism.Mvvm;
using ProblemSolver.GeneradorCodigo;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CodexAssistant.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private OpenAIAPI openAIAPI = new OpenAIAPI("");

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

        public MainWindowViewModel()
        {
            TaskList = new ObservableCollection<TaskItem>();
            Button1Command = new DelegateCommand(ButtonAppRequest);
            Button2Command = new DelegateCommand(Button2Execute);
            Button3Command = new DelegateCommand(Button3Execute);
            Button4Command = new DelegateCommand(Button4Execute);
            LogEntries = new ObservableCollection<string>();
            LogEntries.CollectionChanged += LogEntries_CollectionChanged;

            string rutaPromptAppRequest = "C:\\Users\\pvega\\source\\repos\\CodexAssistant\\Prompts\\Prompt AppRequest.txt";
            string rutaPromptTaskRequest = "C:\\Users\\pvega\\source\\repos\\CodexAssistant\\Prompts\\Prompt TaskRequest.txt";
            _promptAppRequest = File.ReadAllText(rutaPromptAppRequest);
            _promptTaskRequest = File.ReadAllText(rutaPromptTaskRequest);

            comChatGPT = new GeneradorCodigo(openAIAPI);
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

        private async void ButtonAppRequest()
        {
            TaskList = await SolicitarListaTareas();
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

        private void Button2Execute()
        {
            // Implementar la lógica para el botón 2 aquí
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
