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
        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("arguments")]
        public string Arguments { get; set; }

        [JsonProperty("comments")]
        public string Comentarios { get; set; }

        [JsonProperty("content")]
        public string Contenido { get; set; }
    }
}
