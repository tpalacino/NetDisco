using NetDisco;
using System;
using System.Diagnostics;

namespace NetDisco.Example
{
	public class MyClient : AutoDiscoverableClient<SampleRequest, SampleResponse>
	{
		private string _Name = string.Empty;

		public override string Name => _Name;

		public MyClient(string name)
		{
			_Name = name;
		}

		public SampleResponse SendFormat(string format, params object[] args)
		{
			return Send(new SampleRequest() { Message = string.Format(format, args) });
		}
	}
}
