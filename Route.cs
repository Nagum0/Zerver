namespace SimpleHTTPServer;

public class Route {
    private string path;
    private string httpMethod;

    public Route(string path, string httpMethod) {
        this.path = path;
        this.httpMethod = httpMethod;
    }

    public string Path {
        get => path;
    }

    public string HttpMethod {
        get => httpMethod;
    }
}
