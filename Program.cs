using System.Net;

namespace SimpleHTTPServer {
    public class Program {
        static async Task Main(string[] args) {
            string prefix = "http://localhost:5000/";
            
            AsyncServer server = new AsyncServer(prefix);
            server.Route("/rei", "GET");

            await server.StartServerAsync();
        }
    }
}
