using NetDisco;
using System;
using System.Diagnostics;
using System.Net;

namespace NetDisco.Example
{
	public class MyServer : AutoDiscoverableServer<SampleRequest, SampleResponse>
	{
		private string _Name = string.Empty;

		private IPAddress _Address = IPAddress.None;

		private int _Port = 0;
		
		public override string Name => _Name;

		public override IPAddress Address => _Address;

		public override int Port => _Port;

		public MyServer(string name, IPAddress address, int port)
		{
			_Name = name;
			_Address = address;
			_Port = port;
		}

		protected override SampleResponse ProcessRequest(SampleRequest request)
		{
			var now = string.Format("{0:hh:mm tt} on {0:MM/dd/yyyy}", DateTime.Now);
			Console.WriteLine("You said '{0}' at {1}", request.Message ?? "null", now);
			return new SampleResponse()
			{
				Result = string.Format("Message Received at {0}", now)
			};
		}
	}
}