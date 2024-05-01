using System.Net;

namespace SimpleHTTPServer {
    public class Program {
        static async void test() {
            await Task.Delay(6000);
            Console.WriteLine("Hello I LOVE ANIME GORL");
        }

        static void test2() {
            Console.WriteLine("Right away baby!");
        }

        static async Task Main(string[] args) {
            string prefix = "http://localhost:5000/";
            
            AsyncServer server = new AsyncServer(prefix);
            server.Route("/rei", "GET", test);
            server.Route("/fur/yae", "GET", test2);

            await server.StartServerAsync();
        }
    }
}
