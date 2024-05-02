using System.Net;

namespace SimpleHTTPServer;

public class Route {
    private string path;
    private string httpMethod;
    private Func<Request, Response, Task> onRoute;

    public Route(string path, string httpMethod, Func<Request, Response, Task> onRoute) {
        this.path = path;
        this.httpMethod = httpMethod;
        this.onRoute = onRoute;
    }

    public string Path {
        get => path;
    }

    public string HttpMethod {
        get => httpMethod;
    }

    public Func<Request, Response, Task> OnRoute {
        get => onRoute;
    }
}
