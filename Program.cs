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

            server.Route("/fur/hello", "POST", async (HttpListenerRequest req, HttpListenerResponse res) => {
                string reqBody = await server.GetRequestBody(req);
                
                if (reqBody == "Astolfo")
                    Console.WriteLine("BEST BOY!");
                else
                    Console.WriteLine(";(");
                
                await server.Send(res, "Received!");
            });

            await server.StartServerAsync();
        }
    }
}
