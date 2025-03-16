using System.Net;

namespace SimpleHTTPServer {
    public class Program {
        static async Task Main(string[] args) {
            string prefix = "http://localhost:5000/";
            AsyncServer server = new AsyncServer(prefix);

            server.Route("/cat", "GET", async (HttpListenerRequest req, HttpListenerResponse res) => {
                await server.RenderPage(req, res, "cat.html");
            });

            server.Route("/dog", "GET", async (HttpListenerRequest req, HttpListenerResponse res) => {
                await server.RenderPage(req, res, "dog.html");
            });

            server.Route("/dog", "POST", async (HttpListenerRequest req, HttpListenerResponse res) => {
                string body = await server.GetRequestBody(req);
                Console.WriteLine(body);
                await server.Send(res, "idk");
            });

            await server.StartServerAsync();
        }
    }
}
