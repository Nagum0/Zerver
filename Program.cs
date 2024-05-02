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
                await server.RenderPage(req, res, "fur.html");
            });

            server.Route("/fur", "POST", async (HttpListenerRequest req, HttpListenerResponse res) => {
                string body = await server.GetRequestBody(req);
                Console.WriteLine(body);
                await server.Send(res, "idk");
            });

            await server.StartServerAsync();
        }
    }
}
