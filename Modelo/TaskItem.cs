using Newtonsoft.Json;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodexAssistant.Modelo
{
    public class TaskItem : BindableBase
    {
        private string _taskDescription;
        private string _comments;
        private string _status;

        [JsonProperty("task")]
        public string TaskName
        {
            get => _taskDescription;
            set => SetProperty(ref _taskDescription, value);
        }

        [JsonProperty("comentarios")]
        public string Comments
        {
            get => _comments;
            set => SetProperty(ref _comments, value);
        }

        [JsonProperty("estado")]
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value);
        }
    }

}
