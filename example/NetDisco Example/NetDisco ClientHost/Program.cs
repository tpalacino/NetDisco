using NetDisco.Example;
using NetDisco_Example;
using System;

namespace NetDisco_ClientHost
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var client = new MyClient(Constants.ServerName);
			
			Console.WriteLine("Type a message to send to the server or type 'quit' to terminate the application.");
			var message = Console.ReadLine();
			while (!"quit".Equals(message))
			{
				var response = client.SendFormat("SENT: {0}", message);
				if (response != null)
				{
					if (!string.IsNullOrWhiteSpace(response.Result))
					{
						Console.WriteLine("Server responded: {0}", response.Result);
					}
					else if (response.Error != null)
					{
						Console.WriteLine("ERROR: {0}", response.Error);
					}
				}
				message = Console.ReadLine();
			}
			Console.ReadLine();
		}
	}
}
