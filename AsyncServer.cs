using System.Net;

namespace SimpleHTTPServer;

public class AsyncServer {
    private string prefix;
    private int requestCount;
    private HashSet<Route> routes;
    
    public AsyncServer(string prefix) {
        this.prefix = prefix;
        requestCount = 0;
        routes = new HashSet<Route>();
    }

    public string Prefix {
        get => prefix;
    }

    /* 
        Adds a new route.

        @param route
    */
    public void Route(string path, string httpMethod) => routes.Add(new Route(path, httpMethod));

    /*
        Starts the server.
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

    /*
        Upon receiving a request we print out the request information and body. Then we send a response.

        @param context The receieved request.
    */
    private async Task ProcessRequestAsync(HttpListenerContext context) {
        HttpListenerRequest req = context.Request;
        string reqBody = await ReadRequestBodyAsync(req);

        // Printing request information:
        Console.WriteLine($"Request received; Request count: {requestCount}");
        Console.WriteLine($"Request URL: {req.Url}\nRequest body: {reqBody}\nHTTP Method: {req.HttpMethod}\nContent-Type: {req.ContentType}\nContent-Length: {req.ContentLength64}\nContent-Encoding{req.ContentEncoding}\nUser-Agent: {req.UserAgent}\n");
        
        // Sending response:
        string htmlPath = "";
        HttpStatusCode statusCode = HttpStatusCode.OK;

        foreach (Route route in routes) {
            if (route.Path == req.Url?.AbsolutePath && route.HttpMethod == req.HttpMethod) {
                htmlPath = string.Concat(route.Path, ".html");
                break;
            }
        }

        if (htmlPath == "") {
            htmlPath = "/404.html";
            statusCode = HttpStatusCode.NotFound;
        }

        Console.WriteLine($"Sent page: frontend/pages{htmlPath}");
        byte[] resContent = await ParseTextFileToBytesAsync($"frontend/pages{htmlPath}");

        Console.WriteLine("\n----------------------------------------------------------\n");

        await HandleResponse(context, resContent, "text/html", statusCode);
    }

    /*
        Creates the response object. Sets the response headers. Then writes the response content to the output stream.

        @param context The HttpListenerContext object (the received request).
        @param content A byte array containing the response body.
        @param contentType A string representing the contentType.
        @param statusCode The response status code.
    */
    private static async Task HandleResponse(HttpListenerContext context, byte[] content, string contentType = "text/plain", HttpStatusCode statusCode = HttpStatusCode.OK) {
        HttpListenerResponse res = context.Response;
        res.ContentType = contentType;
        res.StatusCode = (int) statusCode;
        res.ContentLength64 = content.Length;
        await res.OutputStream.WriteAsync(content, 0, content.Length);
        res.OutputStream.Close();
    }

    /*
        Parses a text file to bytes asynchronously.

        @param filePath The path to the file.
        @return A Task of byte array.
    */
    private static async Task<byte[]> ParseTextFileToBytesAsync(string filePath) {
        return await File.ReadAllBytesAsync(filePath);
    }

    /*
        Parses the received string content into a byte array.

        @param content String that holds the contents.
        @return A UTF8 encoded byte array.
    */
    private static byte[] ParseContentToBytes(string content) {
        return System.Text.Encoding.UTF8.GetBytes(content);
    }

    /* 
        Reads the body of an incoming request.

        @param req The received request.
        @return The body of the request as a string.
    */
    private static async Task<string> ReadRequestBodyAsync(HttpListenerRequest req) {
        using (Stream body = req.InputStream) {
            using (StreamReader reader = new StreamReader(body, req.ContentEncoding)) {
                return await reader.ReadToEndAsync();
            }
        }
    }
}