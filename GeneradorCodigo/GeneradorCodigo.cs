using OpenAI_API;
using System.IO;
using System.Threading.Tasks;

namespace ProblemSolver.GeneradorCodigo
{
    class GeneradorCodigo : IGeneradorCodigo
    {
        private string PromptSystemCodigo;
        private string PromptSystemTests;

        private readonly OpenAIAPI _api; 
        public GeneradorCodigo(OpenAIAPI api)
        {
            _api = api;
        }

        public async Task<string> GenerarCodigoFuncionAsync(string problema)
        {
            var conversation = _api.Chat.CreateConversation();
            conversation.AppendSystemMessage(PromptSystemCodigo);
            conversation.AppendUserInput(problema);
            string codigo = await conversation.GetResponseFromChatbotAsync();
            return codigo;
        }

        public async Task<string> GenerarCodigoTestsAsync(string problema, string codigoFuncion)
        {
            var conversation = _api.Chat.CreateConversation();
            conversation.AppendSystemMessage(PromptSystemTests);
            conversation.AppendUserInput(codigoFuncion);
            string codigo = await conversation.GetResponseFromChatbotAsync();
            return codigo;
        }

    }
}
