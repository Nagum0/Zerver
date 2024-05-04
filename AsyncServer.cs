using System.Net;

namespace SimpleHTTPServer;

public class AsyncServer {
    private string prefix;
    private int requestCount;
    private HashSet<Route> routes;
    private bool debugMode;

    public AsyncServer(string prefix) {
        this.prefix = prefix;
        requestCount = 0;
        routes = new HashSet<Route>();
        debugMode = false;
    }

    public string Prefix {
        get => prefix;
    }

    public bool DebugMode {
        get => debugMode;
        set => debugMode = value;
    }

    /* 
        Adds a new route.

        @param route
    */
    public void Route(string path, string httpMethod, Func<Request, Response, Task> onRoute) => routes.Add(new Route(path, httpMethod, onRoute));

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
        Creates the response object. Sets the response headers. Then writes the response content to the output stream.

        @param res The HttpListenerResponse object.
        @param content A byte array containing the response body.
        @param contentType A string representing the contentType.
        @param statusCode The response status code.
    */
    internal static async Task HandleResponse(HttpListenerResponse res, byte[] content, string contentType = "text/plain", HttpStatusCode statusCode = HttpStatusCode.OK) {
        res.ContentType = contentType;
        res.StatusCode = (int) statusCode;
        res.ContentLength64 = content.Length;
        await res.OutputStream.WriteAsync(content, 0, content.Length);
        res.OutputStream.Close();
        res.Close();
    }

    /*
        Parses a text file to bytes asynchronously.

        @param filePath The path to the file.
        @return A Task of byte array.
    */
    internal static async Task<byte[]> ParseFileToBytesAsync(string filePath) {
        return await File.ReadAllBytesAsync(filePath);
    }

    /*
        Parses the received string content into a byte array.

        @param content String that holds the contents.
        @return A UTF8 encoded byte array.
    */
    internal static async Task<byte[]> ParseContentToBytes(string content) {
        return await Task.Run(() => System.Text.Encoding.UTF8.GetBytes(content));
    }

    /*
        Upon receiving a request we print out the request information and body. Then we send a response.

        @param context The receieved request.
    */
    private async Task ProcessRequestAsync(HttpListenerContext context) {
        HttpListenerRequest req = context.Request;
        HttpListenerResponse res = context.Response;

        if (debugMode) {
            Console.WriteLine("\n----------------------------------------------------------\n");
            Console.WriteLine($"Request received; Request count: {requestCount}");
            Console.WriteLine($"Request URL: {req.Url}\nURL Absoulte Path: {req.Url?.AbsolutePath}\nHTTP Method: {req.HttpMethod}\nContent-Type: {req.ContentType}\nContent-Length: {req.ContentLength64}\nContent-Encoding{req.ContentEncoding}\nUser-Agent: {req.UserAgent}");
        }

        // Sending response:
        string requestAbsoultePath = req.Url?.AbsolutePath ?? "/";
        string requestHttpMethod = req.HttpMethod;
        bool routeLoaded = false;

        // Searching for a route:
        foreach (Route route in routes) {
            if (route.Path == requestAbsoultePath && route.HttpMethod == requestHttpMethod) {
                await route.OnRoute(new Request(req), new Response(res));
                routeLoaded = true;
                break;
            }
        }
        
        // If no route was loaded, try loading static files:
        if (!routeLoaded) {
            await LoadStaticContent(res, requestAbsoultePath, requestHttpMethod);
        }

        // If there was no response sent yet send a base case response. This should only happen 
        // when the user doesn't call a method that sends a response automatically like: RenderPage
        // or when no static files are sent. 
        if (res.OutputStream.CanWrite) {
            await HandleResponse(res, await ParseContentToBytes("Base case response..."));
        }
    }

    /*
        Loads any requested static content like css, js and image files.

        @param res Response object.
        @param requestAbsoultePath The absolute path of the request.
        @param requestHttpMethod The request's http method.
    */
    private static async Task LoadStaticContent(HttpListenerResponse res, string requestAbsoultePath, string requestHttpMethod) {
        string responseFilePath = "";
        string responseContentType = "text/plain";

        if (requestAbsoultePath.StartsWith("/static/css/") && requestHttpMethod == "GET") {
            responseFilePath = string.Concat("frontend", requestAbsoultePath);
            responseContentType = "text/css";
        }
        else if (requestAbsoultePath.StartsWith("/static/js/") && requestHttpMethod == "GET") {
            responseFilePath = string.Concat("frontend", requestAbsoultePath);
            responseContentType = "text/javascript";
        }
        else if (requestAbsoultePath.StartsWith("/static/imgs/") && requestHttpMethod == "GET") {
            responseFilePath = string.Concat("frontend", requestAbsoultePath);
            responseContentType = "image/jpeg";
        }

        Console.WriteLine($"\nStatic content file path: {responseFilePath}");

        try {
            await HandleResponse(res, await ParseFileToBytesAsync(responseFilePath), responseContentType, HttpStatusCode.OK);
        }
        catch {
            Console.WriteLine("Content not found!");
            await HandleResponse(res, await ParseContentToBytes("404 content not found..."), "text/plain", HttpStatusCode.NotFound);
        }
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