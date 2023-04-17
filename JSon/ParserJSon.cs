using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace CodexAssistant.JSon
{
    public static class ParserJSon
    {
        public static string ExtraerJSON(string textoCompleto)
        {
          
            string pattern = @"\{(?:[^{}]|(?<o>\{)|(?<-o>\}))+(?(o)(?!))\}";
            Match match = Regex.Match(textoCompleto, pattern);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                throw new ArgumentException("No se encontró un JSON válido en la entrada proporcionada.");
            }

        }

        public static string FormatJsonString(string jsonString)
        {
            try
            {
                dynamic parsedJson = JsonConvert.DeserializeObject(jsonString);

                return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            catch
            {
                // Si no es un JSON válido, devuelva la cadena original
                return jsonString;
            }
        }
    }
}
