using System.Net;

namespace SimpleHTTPServer;

public class AsyncServer {
    private string prefix;
    private int requestCount;
    public List<Route> routes;
    
    public AsyncServer(string prefix) {
        this.prefix = prefix;
        requestCount = 0;
        routes = new List<Route>();
    }

    public string Prefix {
        get => prefix;
    }

    /*
        Starts the AsyncServer.
    */
    public async Task StartServerAsync() {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add(prefix);
        listener.Start();

        try {
            while (true) {
                HttpListenerContext context = await listener.GetContextAsync();
                requestCount++;
                Task _ = ProcessRequestAsync(context);
            }
        }
        catch (HttpListenerException e) {
            Console.WriteLine(e.Message);
        }
        finally {
            listener.Stop();
        }
    }

    private async Task ProcessRequestAsync(HttpListenerContext context) {
        Console.WriteLine("Request received!");

        HttpListenerRequest req = context.Request;
        string reqMsg = await ReadRequestMessageAsync(req);
        byte[] resContent = ParseContentToBytes("Hello, from Hatsune Miku ;)");
        await HandleResponse(context, resContent);
    }

    /*
        Creates the response object. Sets the response headers. Then writes the response content to the output stream.

        @param context The HttpListenerContext object (the received request).
        @param content A byte array containing the response body.
        @param contentType A string representing the contentType.
        @param statusCode The response status code.
    */
    private async Task HandleResponse(HttpListenerContext context, byte[] content, string contentType = "text/plain", HttpStatusCode statusCode = HttpStatusCode.OK) {
        HttpListenerResponse res = context.Response;
        res.ContentType = contentType;
        res.StatusCode = (int) statusCode;
        res.ContentLength64 = content.Length;
        await res.OutputStream.WriteAsync(content, 0, content.Length);
        res.OutputStream.Close();
    }

    private byte[] ParseContentToBytes(string content) {
        return System.Text.Encoding.UTF8.GetBytes(content);
    }

    /* 
        Reads the body of an incoming request.

        @param req The received request.
        @return The body of the request as a string.
    */
    private async Task<string> ReadRequestMessageAsync(HttpListenerRequest req) {
        using (Stream body = req.InputStream) {
            using (StreamReader reader = new StreamReader(body, req.ContentEncoding)) {
                return await reader.ReadToEndAsync();
            }
        }
    }
}