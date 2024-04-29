using System.Net;

namespace SimpleHTTPServer {
    public class Program {
        static async Task Main(string[] args) {
            string[] prefixes = { "http://localhost:5000/", "http://localhost:8080/", "http://localhost:5000/idk/" };
            /* StartServer(prefixes); */

            AsyncServer asyncServer = new AsyncServer(prefixes);
            await asyncServer.StartServerAsync();
        }

        static void StartServer(string[] prefixes) {
            HttpListener listener = new HttpListener();

            foreach (string prefix in prefixes)
                listener.Prefixes.Add(prefix);

            listener.Start();
            Console.WriteLine($"Listening on {prefixes[0]}");

            try {
                while (true) {
                    HttpListenerContext context = listener.GetContext();
                    ProcessRequest(context);
                    Console.WriteLine("-----------------------------------------------------");
                }
            }
            catch (HttpListenerException e) {
                Console.WriteLine(e);
            }
            finally {
                listener.Stop();
            }
        }

        static void ProcessRequest(HttpListenerContext context) {
            Console.WriteLine("Request received!");

            HttpListenerRequest request = context.Request;
            Console.WriteLine($"URL: {request.Url}\nHTTP: {request.HttpMethod}\nInputStream: {request.InputStream}\nEncoding: {request.ContentEncoding}\nUser agent: {request.UserAgent}");
            
            string requestMessage = ReadMessage(request);
            Console.WriteLine($"Received message: {requestMessage}");

            HandleResponse(context, "Hello, from server!");
        }

        static void HandleResponse(HttpListenerContext context, string message) {
            (HttpListenerResponse response, byte[] buffer) = CreateResponse(context, message);
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        static (HttpListenerResponse, byte[]) CreateResponse(HttpListenerContext context, string message) {
            HttpListenerResponse response = context.Response;

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;

            return (response, buffer);
        }

        static string ReadMessage(HttpListenerRequest request) {
            using (Stream body = request.InputStream) {
                using (StreamReader reader = new StreamReader(body, request.ContentEncoding)) {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
