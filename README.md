# NetDisco
A .Net Standard library for creating auto-discoverable servers and clients.

## Feedback
This project is currently a [MVP](https://en.wikipedia.org/wiki/Minimum_viable_product). Please reach out if you find it useful and esspecially if you have any suggestions.

## Getting Started

### Create a Object Model library
This is where we'll define the object model to share between the server and client projects.

**IMPORTANT:** The requests and responses are send over the network using JSON serialization. 

1. Create a new **Class Library** project
2. Add a class that will be the request object
    ```csharp
    public class Request
    {
        public string Body { get; set; }
    }
    ```
3. Add a class that will be the response objet
    ```csharp
    public class Response
    {
        public string Result { get; set; }
    }
    ```

### Create a Server Application
This is where we'll create the auto-discoverable server using the shared object model
1. Create a new Console App project
2. Install the [NetDisco Nuget package](https://www.nuget.org/packages/NetDisco/)
    * [How To: Include a NuGet package in your project](https://docs.microsoft.com/en-us/visualstudio/mac/nuget-walkthrough)
3. Add a class that will be the server
    * Add the following using
        ```csharp
        using NetDisco;
        ```
    * Add the `AutoDiscoverableServer<Request, Response>` base class
    * Implement the required members **Name**, **Address**, **Port**, **HandleError**, and **ProcessRequest**
        ```csharp
        public class Server : AutoDiscoverableServer<Request, Response>
        {
            public Server(string name, IPAddress address, int port)
            {
                _Name = name;
                _Address = address;
                _Port = port;
            }
            
            private string _Name = string.Empty;
            public override string Name => _Name;
            
            private IPAddress _Address = IPAddress.None;
            public override IPAddress Address => _Address;
            
            private int _Port = 0;
            public override int Port => _Port;
            
            protected override Response HandleError(Request request, Exception error)
            {
                // Log the error message and build a response.
                
                var result = string.Format("Error processing the request. Error: {0}", error);
                
                return new Response { Result = result };
            }
            
            protected override Response ProcessRequest(Request request)
            {
                // Do work with the request and build a response.
                
                var result = string.Format("Server received the request: Body: {0}", request.Body);
                
                return new Response { Result = result };
            }
        }
        ```
4. Update the **Main** method of **Program.cs** to instantiate and start the server
    ```csharp
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server("My Server", IPAddress.Parse("AN_IP_ON_YOUR_MACHINE"), 24516);
            server.Start();
            Console.WriteLine("Server Started: {0}", server);
            Console.ReadLine();
        }
    }
    ```

### Create a Client Application
This is where we'll create the client for the auto-discoverable server using the shared object model
1. Create a new Console App project
2. Install the [NetDisco Nuget package](https://www.nuget.org/packages/NetDisco/)
    * [How To: Include a NuGet package in your project](https://docs.microsoft.com/en-us/visualstudio/mac/nuget-walkthrough)
3. Add a class that will be the client
    * Add the following using
        ```csharp
        using NetDisco;
        ```
    * Add the `AutoDiscoverableClient<Request, Response>` base class
    * Implement the required members **Name** and **HandleError**
    ```csharp
    public class Client : AutoDiscoverableClient<Request, Response>
    {
        public Client(string name)
        {
            _Name = name;
        }
        
        private string _Name = string.Empty;
        public override string Name => _Name;
        
        protected override Response HandleError(Request request, Exception error)
        {
            // Log the error message and build a response.
            
            return new Response { Result = string.Format("Error sending request: {0}", error) };
        }
        
        public string SendMessage(string message)
        {
            return Send(new Request { Body = message })?.Result;
        }
    }
    ```
4. Update the **Main** method of **Program.cs** to instantiate the client, send a message, output the response
    ```csharp
    class Program
    {
        static void Main(string[] args)
        {
            // This name needs to match the name of the server
            var client = new Client("My Server");
            var reply = client.SendMessage("Hello Auto Discovered World!");
            Console.WriteLine(reply);
            Console.ReadLine();
        }
    }
    ```

### Watch it Work
1. Run the server without debugging
2. Run the client and watch it auto-discover the server, send a message and write the response from the server.
3. Close the client and server applications
4. Change the port and or IP address of the server
5. Repeat steps 1 and 2 of this section
