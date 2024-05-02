using System.Net;

namespace SimpleHTTPServer;

public class Response {
    private HttpListenerResponse res;

    public Response(HttpListenerResponse res) {
        this.res = res;
    }

    /* 
        Sends a string as a response.

        @param responseBody Response string to be sent.
    */
    public async Task Send(string responseBody) {
        try {
            await AsyncServer.HandleResponse(res, await AsyncServer.ParseContentToBytes(responseBody));
        }
        catch (FileNotFoundException) {
            Console.WriteLine("An error occured while sending response...");
            await AsyncServer.HandleResponse(res, await AsyncServer.ParseContentToBytes("An error occured while sending response..."));
        }
    }

    /* 
        When called it will render an html page loading all the required assets for it too, such as static css and js files.

        @param pagePath The path to the page to be loaded.
    */
    public async Task RenderPage(string pagePath) {
        try {
            await AsyncServer.HandleResponse(res, await AsyncServer.ParseFileToBytesAsync(string.Concat("frontend/pages/", pagePath)), "text/html");
        }
        catch {
            Console.WriteLine("File for sepcified page not found!");
            await AsyncServer.HandleResponse(res, await AsyncServer.ParseFileToBytesAsync("frontend/pages/404.html"), "text/html", HttpStatusCode.NotFound);
        }
    }
}