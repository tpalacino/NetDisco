using NetDisco;
using System;

namespace NetDisco.Example
{
	public class SampleResponse : IAutoDiscoverableServerResponse<SampleRequest>
	{
		public string Result { get; set; }

		public SampleRequest Request { get; set; }

		public Exception Error { get; set; }
	}
}