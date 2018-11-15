using NetDisco;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace NetDisco_Example
{
	public class MyClient : AutoDiscoverableClient<SampleRequest, SampleResponse>
	{
		private string _Name = string.Empty;

		public override string Name => _Name;

		public MyClient(string name)
		{
			_Name = name;
		}

		internal Action OnServerFound { get; set; }
		
		protected override void OnServerDiscovered(ServerInfo server)
		{
			if (OnServerFound != null)
			{
				try
				{
					OnServerFound();
				}
				catch (Exception ex)
				{
					Trace.WriteLine(string.Format("An error occurred calling OnServerFound. Details: {0}", JsonConvert.SerializeObject(ex)));
				}
			}
		}

		public SampleResponse SendFormat(string format, params object[] args)
		{
			return Send(new SampleRequest() { Message = string.Format(format, args) });
		}

		protected override SampleResponse HandleError(SampleRequest request, Exception error)
		{
			return new SampleResponse() { Request = request, Error = error };
		}
	}
}
