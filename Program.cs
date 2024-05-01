﻿using System.Net;

namespace SimpleHTTPServer {
    public class Program {
        static async Task Main(string[] args) {
            string prefix = "http://localhost:5000/";
            AsyncServer server = new AsyncServer(prefix);
            
            server.routes.Add(new Route("/rei", "GET"));
            
            /*
                server.Route("/reviews", Method.GET, "");
            */

            await server.StartServerAsync();
        }
    }
}
