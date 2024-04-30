using System.Net;
using System.Text.Json;

namespace SimpleHTTPServer {
    public class AsyncServer {
        private string[] prefixes;
        private int requestCount;

        public AsyncServer(string[] prefixes) {
            this.prefixes = prefixes.Select(a => (string) a.Clone()).ToArray();
            requestCount = 0;
        }

        public async Task StartServerAsync() {
            HttpListener listener = new HttpListener();

            foreach (string prefix in prefixes)
                listener.Prefixes.Add(prefix);

            listener.Start();
            Console.WriteLine($"Listening on {string.Join(", ", prefixes)}");

            try {
                while (true) {
                    HttpListenerContext context = await listener.GetContextAsync();
                    requestCount++;
                    _ = ProcessRequestAsync(context);
                }
            }
            catch (HttpListenerException e) {
                Console.WriteLine(e.Message);
            }
            finally {
                listener.Close();
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context) {
            Console.WriteLine($"Request received - {requestCount}");

            HttpListenerRequest request = context.Request;
            string requestBody = await ReadMessageAsync(request);
            Task response;

            if (request.HttpMethod == "GET" && request.Url?.AbsolutePath == "/")
                response = HandleResponse(context, await ReadHtmlFile("frontend/index.html"), "text/html", HttpStatusCode.OK);
            else if (request.HttpMethod == "GET" && request.Url?.AbsolutePath == "/style.css")
                response = HandleResponse(context, await ReadHtmlFile("frontend/style.css"), "text/css", HttpStatusCode.OK);
            else if (request.HttpMethod == "GET" && request.Url?.AbsolutePath == "/index.js")
                response = HandleResponse(context, await ReadHtmlFile("frontend/index.js"), "text/javascript", HttpStatusCode.OK);
            else if (request.HttpMethod == "GET" && request.Url?.AbsolutePath == "/imgs/rei9.jpg")
                response = HandleResponse(context, await ReadImageFile("frontend/imgs/rei9.jpg"), "image/jpeg", HttpStatusCode.OK);
            else if (request.HttpMethod == "GET" && request.Url?.AbsolutePath == "/imgs/fur6.jpg")
                response = HandleResponse(context, await ReadImageFile("frontend/imgs/fur6.jpg"), "image/jpeg", HttpStatusCode.OK);
            else
                response = HandleResponse(context, "Hello, from server!");

            Console.WriteLine($"URL: {request.Url}\nHTTP: {request.HttpMethod}\nInputStream: {request.InputStream}\nEncoding: {request.ContentEncoding}\nUser agent: {request.UserAgent}");
            Console.WriteLine($"Received message: {requestBody}");
            await response;

            Console.WriteLine("\n-------------------------------------\n");
        }

        private async Task HandleResponse(HttpListenerContext context, byte[] content, string contentType = "image/jpeg", HttpStatusCode statusCode = HttpStatusCode.OK) {
            HttpListenerResponse response = context.Response;
            response.ContentType = contentType;
            response.ContentLength64 = content.Length;
            response.StatusCode = (int) statusCode;
            response.Headers.Add("Cache-Control", "no-cache");
            
            await response.OutputStream.WriteAsync(content, 0, content.Length);
        }

        private async Task HandleResponse(HttpListenerContext context, string content, string contentType = "text/plain", HttpStatusCode statusCode = HttpStatusCode.OK) {
            HttpListenerResponse response = context.Response;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            response.ContentLength64 = buffer.Length;
            response.ContentType = contentType;
            response.StatusCode = (int) statusCode;

            Stream output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);
            output.Close();
        }

        private async Task HandleResponse(HttpListenerContext context, string message) {
            HttpListenerResponse response = context.Response;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            response.ContentLength64 = buffer.Length;

            Stream output = response.OutputStream;
            await output.WriteAsync(buffer, 0, buffer.Length);
            output.Close(); 
        }

        private Task<byte[]> ReadImageFile(string filePath) {
            return File.ReadAllBytesAsync(filePath);
        }

        private Task<string> ReadHtmlFile(string filePath) {
            return File.ReadAllTextAsync(filePath);
        }
        
        private async Task<string> ReadMessageAsync(HttpListenerRequest request) {
            using (Stream body = request.InputStream) {
                using (StreamReader reader = new StreamReader(body, request.ContentEncoding)) {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}