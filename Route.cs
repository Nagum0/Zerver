namespace SimpleHTTPServer;

public class Route {
    private string path;
    private string httpMethod;
    private Action onRoute;

    public Route(string path, string httpMethod, Action onRoute) {
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

    public Action OnRoute {
        get => onRoute;
    }
}
