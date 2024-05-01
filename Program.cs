using System.Net;

namespace SimpleHTTPServer {
    public class Program {
        static async Task Main(string[] args) {
            string prefix = "http://localhost:5000/";
            AsyncServer server = new AsyncServer(prefix);
            
            /*
                server.Route("/reviews", Method.GET, "");
            */

            await server.StartServerAsync();

            /* string[] prefixes = { "http://localhost:5000/", "http://localhost:8080/", "http://localhost:5000/idk/" };
            AsyncServerOLD asyncServer = new AsyncServerOLD(prefixes);
            await asyncServer.StartServerAsync(); */
        }
    }
}
