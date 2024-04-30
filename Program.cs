using System.Net;

namespace SimpleHTTPServer {
    public class Program {
        static async Task Main(string[] args) {
            string[] prefixes = { "http://localhost:5000/", "http://localhost:8080/", "http://localhost:5000/idk/" };
            AsyncServer asyncServer = new AsyncServer(prefixes);
            await asyncServer.StartServerAsync();
        }
    }
}
