using System.Net;

namespace SimpleHTTPServer {
    public class Program {
        static async Task Main(string[] args) {
            string prefix = "http://localhost:5000/";
            AsyncServer server = new AsyncServer(prefix);

            server.Route("/rei", "GET", async (HttpListenerRequest req, HttpListenerResponse res) => {
                await server.RenderPage(req, res, "rei.html");
            });

            server.Route("/fur", "GET", async (HttpListenerRequest req, HttpListenerResponse res) => {
                await server.RenderPage(req, res, "fa.html");
            });

            await server.StartServerAsync();
        }
    }
}
