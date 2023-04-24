using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodexAssistant.Modelo
{

    public enum Comandos
    {
        CreateProject,
        GetFile,
        UpdateFile,
        AddFile,
        GetContext,
        AddReference
    }

    public class CommandData
    {
       
        public string command { get; set; }
        public string arguments { get; set; }
        public string comments { get; set; }
        public string content { get; set; }
    }
}
