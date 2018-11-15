using System;
using System.Collections.Generic;
using System.Text;

namespace NetDisco_Example
{
	public class SampleResponse
	{
		public string Result { get; set; }

		public SampleRequest Request { get; set; }

		public Exception Error { get; set; }
	}
}
