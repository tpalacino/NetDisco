using NetDisco.Example;
using NetDisco_Example;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace NetDisco_ServerHost
{
	class Program
	{
		static void Main(string[] args)
		{
			var server = new MyServer(Constants.ServerName, GetIpAddress(), new Random(DateTime.Now.Millisecond).Next(24000, 25000))
			{
				OnProcessRequest = (request) =>
				{
					Console.WriteLine("RECEIVED: {0}", request.Message);
				}
			};
			server.Start();
			Console.WriteLine("Server started: {0}", server);
			Console.WriteLine("Press enter to terminate.");
			Console.ReadLine();
		}

		private static IPAddress GetIpAddress()
		{
			IPAddress retVal = null;

			var host = Dns.GetHostEntry(Dns.GetHostName());
			if (host != null && host.AddressList != null)
			{
				foreach (var address in host.AddressList)
				{
					if (AddressFamily.InterNetwork.Equals(address.AddressFamily))
					{
						retVal = address;
						break;
					}
				}
			}

			return retVal;
		}
	}
}
