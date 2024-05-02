using System.Net;

namespace SimpleHTTPServer;

public class Request {
    private HttpListenerRequest req;

    public Request(HttpListenerRequest req) {
        this.req = req;
    }

    /* 
        When called will read the request body and return it as a string.

        @return A Task<string> of the read request body.
    */
    public async Task<string> GetRequestBody() {
        using (Stream body = req.InputStream) {
            using (StreamReader reader = new StreamReader(body, req.ContentEncoding)) {
                return await reader.ReadToEndAsync();
            }
        }
    }
}