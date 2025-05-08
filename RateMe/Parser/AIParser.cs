using System.Net.Http;

namespace RateMe.Parser
{
    internal class AIParser
    {
        private HttpClient _client;

        // private static string DeepQwenUrl = "https://openrouter.ai/api/v1/chat/completions";

        public AIParser() {
            _client = new HttpClient();
        }

    }
}
