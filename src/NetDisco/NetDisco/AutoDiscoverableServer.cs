/*
	This file is part of NetDisco.

    NetDisco is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    NetDisco is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with NetDisco.  If not, see <https://www.gnu.org/licenses/>.
 */

using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetDisco
{
	/// <summary>Provides derived classes with an auto-discoverable generic server.</summary>
	/// <typeparam name="TRequest">The type of request object.</typeparam>
	/// <typeparam name="TResponse">The type of response object.</typeparam>
	public abstract class AutoDiscoverableServer<TRequest, TResponse> : AutoDiscoverableServer
	{
		#region Methods

		#region HandleError
		/// <summary>Called when an error occurrs processing the request.</summary>
		/// <param name="request">The request that caused the error.</param>
		/// <param name="error">The error that occurred.</param>
		/// <returns>A <typeparamref name="TResponse"/> instance based on the request and error.</returns>
		protected abstract TResponse HandleError(TRequest request, Exception error);
		#endregion HandleError

		#region ProcessRequest
		/// <summary>Called to process a request.</summary>
		/// <param name="request">The request to process.</param>
		/// <returns>A <typeparamref name="TResponse"/> instance.</returns>
		protected abstract TResponse ProcessRequest(TRequest request);
		#endregion ProcessRequest

		#region ProcessRequest
		/// <summary>Processes the request.</summary>
		/// <param name="data">The raw data of the request.</param>
		/// <returns>A array of bytes with the raw data of the response.</returns>
		protected override byte[] ProcessRequest(byte[] data)
		{
			byte[] retVal = null;

			if (data != null && data.Length > 0)
			{
				TResponse response = default(TResponse);
				var request = data.ToObject<TRequest>();
				try
				{
					response = ProcessRequest(request);
				}
				catch (Exception ex)
				{
					try
					{
						response = HandleError(request, ex);
					}
					catch (Exception handleEx)
					{
						handleEx.Data["OriginalError"] = ex;
						Logger.Write(LogLevel.Error, "An error occurred processing a request and HandleError method threw an exception handling it. Details: {0}", data.Length, JsonConvert.SerializeObject(handleEx));
					}
				}
				retVal = response.ToByteArray();
			}

			return retVal;
		}
		#endregion ProcessRequest

		#endregion Methods
	}

	/// <summary>Provides derived classes with an auto-discoverable server.</summary>
	public abstract class AutoDiscoverableServer : BaseDiscoverableComponent
	{
		#region Properties

		#region Name
		/// <summary>The name of the server.</summary>
		public abstract string Name { get; }
		#endregion Name

		#region Address
		/// <summary>The IP address that the server will use to listen for requests.</summary>
		public abstract IPAddress Address { get; }
		#endregion Address

		#region Port
		/// <summary>The port that the server will use to listen for requests.</summary>
		public abstract int Port { get; }
		#endregion Port

		#endregion Properties

		#region Methods

		#region HandleError
		/// <summary>Called when an error occurs processing a request that is not an auto discover request.</summary>
		/// <param name="error">The error.</param>
		/// <returns>The data to send back to the remote endpoint.</returns>
		protected virtual byte[] HandleError(Exception error)
		{
			byte[] retVal = new byte[0];

			if (error != null)
			{
				retVal = Encoding.UTF8.GetBytes(error.ToString());
			}

			return retVal;
		}
		#endregion HandleError

		#region ProcessRequest
		/// <summary>Called when a request is received that is not an auto discover request.</summary>
		/// <param name="data">The data received from the remote endpoint.</param>
		/// <returns>The data to send back to the remote endpoint.</returns>
		protected abstract byte[] ProcessRequest(byte[] data);
		#endregion ProcessRequest

		#region StartAutoComponent
		/// <summary>Starts the auto-discoverable server.</summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The arguments of the event.</param>
		protected override void StartAutoComponent(object sender, DoWorkEventArgs e)
		{
			try
			{
				IPEndPoint local = new IPEndPoint(IPAddress.Any, AutoDiscoverPort);
				IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);

				using (UdpClient connection = new UdpClient(local))
				{
					byte[] data = connection.SafeReceive(ref remote);

					while (!mIsStopping)
					{
						if (AutoDiscoverData.SequenceEqual(data))
						{
							var ip = Address.ToString();
							byte[] responseData = (new { Name, Address = ip, Port }).ToByteArray();
							connection.SafeSend(responseData, remote);
						}
						remote = new IPEndPoint(IPAddress.Any, 0);
						data = connection.SafeReceive(ref remote);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Write(LogLevel.Error, "An error occurred initializing the auto-discover component of the server. Error: {0}", JsonConvert.SerializeObject(ex));
			}
		}
		#endregion StartAutoComponent

		#region StartUserComponent
		/// <summary>Starts the user defined server.</summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The arguments of the event.</param>
		protected override void StartUserComponent(object sender, DoWorkEventArgs e)
		{
			try
			{
				IPEndPoint local = new IPEndPoint(Address, Port);
				IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);

				using (UdpClient connection = new UdpClient(local))
				{
					byte[] data = connection.SafeReceive(ref remote);

					while (!mIsStopping)
					{
						byte[] response;
						try
						{
							response = ProcessRequest(data);
						}
						catch (Exception ex)
						{
							response = HandleError(ex);
						}

						connection.SafeSend(response, remote);
						remote = new IPEndPoint(IPAddress.Any, 0);
						data = connection.SafeReceive(ref remote);
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Write(LogLevel.Error, "An error occurred converting an object to a byte array. Error: {0}", JsonConvert.SerializeObject(ex));
			}
		}
		#endregion StartUserComponent

		#region ToString
		/// <summary>Gets the string representation of the server.</summary>
		/// <returns>A <see cref="string"/> with the name, IP address, and port of the server.</returns>
		public override string ToString()
		{
			return string.Format("{0} ({1}:{2})", Name, Address, Port);
		}
		#endregion ToString

		#endregion Methods
	}
}