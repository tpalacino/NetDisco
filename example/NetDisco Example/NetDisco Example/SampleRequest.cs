using NetDisco;
using System;

namespace NetDisco.Example
{
	public class SampleRequest: IAutoDiscoverableServerRequest
	{
		public string Message { get; set; }
	}
}