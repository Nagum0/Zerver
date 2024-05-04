using System.Net;
using SimpleHTTPServer;

public class Program {
    static async Task Main(string[] args) {
        string prefix = "http://localhost:5000/";
        AsyncServer server = new AsyncServer(prefix);
        server.DebugMode = true;

        server.Route("/rei", "GET", async (Request req, Response res) => {
            await res.RenderPage("rei.html");
        });

        server.Route("/fur", "GET", async (Request req, Response res) => {
            await res.RenderPage("fur.html");
        });

        server.Route("/fur", "POST", async (Request req, Response res) => {
            string body = await req.GetRequestBody();
            
            if (body == "1234")
                res.Redirect("/rei");
            else 
                await res.Send("idk");
        });

        await server.StartServerAsync();
    }
}
