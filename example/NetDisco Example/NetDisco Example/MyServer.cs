using NetDisco;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net;

namespace NetDisco_Example
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

		protected override SampleResponse HandleError(SampleRequest request, Exception error)
		{
			return new SampleResponse() { Request = request, Error = error };
		}

		public Action<SampleRequest> OnProcessRequest { get; set; }

		protected override SampleResponse ProcessRequest(SampleRequest request)
		{
			if (OnProcessRequest != null)
			{
				try
				{
					OnProcessRequest(request);
				}
				catch (Exception ex)
				{
					Trace.WriteLine(string.Format("An error occurred calling OnProcessRequest. Details: {0}", JsonConvert.SerializeObject(ex)));
				}
			}

			return new SampleResponse()
			{
				Result = string.Format("Received at {0:hh:mm tt} on {0:dd/MM/yyyy}: {1}", DateTime.Now, request.Message ?? "null")
			};
		}
	}
}