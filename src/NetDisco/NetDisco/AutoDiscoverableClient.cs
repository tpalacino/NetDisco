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

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetDisco
{
	/// <summary>Provides derived classes with a client to an auto-discoverable generic server.</summary>
	/// <typeparam name="TRequest">The type of request object.</typeparam>
	/// <typeparam name="TResponse">The type of response object.</typeparam>
	public abstract class AutoDiscoverableClient<TRequest, TResponse> : AutoDiscoverableClient
		where TRequest : IAutoDiscoverableServerRequest
		where TResponse : IAutoDiscoverableServerResponse<TRequest>, new()
	{
		#region Constructors

		/// <summary>Creates a new instance of <see cref="AutoDiscoverableClient{TRequest, TResponse}"/> with the default timeout of 1 minute.</summary>
		public AutoDiscoverableClient() : this(TimeSpan.FromMinutes(1)) { }

		/// <summary>Creates a new instance of <see cref="AutoDiscoverableClient{TRequest, TResponse}"/> with the specified timeout.</summary>
		/// <param name="timeout">The amount of time to wait for server discovery before failing a request.</param>
		public AutoDiscoverableClient(TimeSpan timeout) : base(timeout) { }

		#endregion Constructors

		#region Methods

		#region HandleError
		/// <summary>Called when an error occurrs processing the request.</summary>
		/// <param name="request">The request that caused the error.</param>
		/// <param name="error">The error that occurred.</param>
		/// <returns>A <typeparamref name="TResponse"/> instance based on the request and error.</returns>
		protected virtual TResponse HandleError(TRequest request, Exception error)
		{
			var retVal = new TResponse();
			retVal.Request = request;
			retVal.Error = error;
			return retVal;
		}
		#endregion HandleError

		#region Send
		/// <summary>Sends the specified request to the server.</summary>
		/// <param name="request">The request to send.</param>
		/// <returns>The response from the server, the output from the HandleError method, or null.</returns>
		protected TResponse Send(TRequest request)
		{
			TResponse retVal = default(TResponse);

			if (request != null)
			{
				try
				{
					retVal = Send(request.ToByteArray()).ToObject<TResponse>();
				}
				catch (Exception ex)
				{
					try
					{
						retVal = HandleError(request, ex);
					}
					catch (Exception handleEx)
					{
						handleEx.Data["OriginalError"] = ex;
						Logger.Write(LogLevel.Error, "An error occurred processing a request and HandleError method threw an exception handling it. Details: {0}", handleEx);
					}
				}
			}

			return retVal;
		}
		#endregion Send

		#endregion Methods
	}

	/// <summary>Provides derived classes with a client to an auto-discoverable server.</summary>
	public abstract class AutoDiscoverableClient : BaseDiscoverableComponent
	{
		#region Member Variables

		/// <summary>Indicates if the server has been discovered.</summary>
		private ManualResetEvent mServerDiscovered = new ManualResetEvent(false);

		#endregion Member Variables

		#region Constructors

		/// <summary>Creates a new instance of <see cref="AutoDiscoverableClient"/> with the default timeout of 1 minute.</summary>
		public AutoDiscoverableClient() : this(TimeSpan.FromMinutes(1)) { }

		/// <summary>Creates a new instance of <see cref="AutoDiscoverableClient"/> with the specified timeout.</summary>
		/// <param name="timeout">The amount of time to wait for server discovery before failing a request.</param>
		public AutoDiscoverableClient(TimeSpan timeout)
		{
			Timeout = timeout;
			Start();
		}

		#endregion Constructors

		#region Properties

		#region Name
		/// <summary>The name of the servers to look for.</summary>
		public abstract string Name { get; }
		#endregion Name

		#region Server
		/// <summary>The auto-discovered server information.</summary>
		public ServerInfo Server { get; private set; }
		#endregion Server

		#region Timeout
		/// <summary>The amount of time to wait for server discovery before failing a request.</summary>
		public TimeSpan Timeout { get; set; }
		#endregion Timeout

		#endregion Properties

		#region Event Handlers

		#region OnServerDiscovered
		/// <summary>Called when a server has been discovered.</summary>
		/// <param name="server">The information about the discovered server.</param>
		protected virtual void OnServerDiscovered(ServerInfo server) { }
		#endregion OnServerDiscovered

		#endregion Event Handlers

		#region Methods

		#region Send
		/// <summary>Sends the specified data to the discovered server.</summary>
		/// <param name="data">The data to send to the server.</param>
		/// <returns>A array or bytess with the response from the server.</returns>
		/// <exception cref="TimeoutException">Throw if no server is found within the current timeout.</exception>
		protected byte[] Send(byte[] data)
		{
			byte[] retVal = new byte[0];

			if (mServerDiscovered.WaitOne(Timeout))
			{
				if (data != null && Server != null)
				{
					try
					{
						IPEndPoint server = Server.Endpoint;
						using (UdpClient connection = new UdpClient())
						{
							connection.SafeSend(data, server);
							retVal = connection.SafeReceive(ref server);
						}
					}
					catch (Exception ex)
					{
						Logger.Write(LogLevel.Error, "An error occurred sending {0} bytes of data to the server. Error: {1}", data.Length, ex);
					}
				}
			}
			else
			{
				throw new TimeoutException("The server discover timeout has expired.");
			}

			return retVal;
		}
		#endregion Send

		#region StartUserComponent
		/// <summary>Starts the client's connection to a user server.</summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The arguments of the event.</param>
		protected override void StartUserComponent(object sender, DoWorkEventArgs e)
		{
			mServerDiscovered.WaitOne();
			OnServerDiscovered(Server);
		}
		#endregion StartUserComponent

		#region StartAutoComponent
		/// <summary>Starts the client's search for an auto-discoverable server.</summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The arguments of the event.</param>
		protected override void StartAutoComponent(object sender, DoWorkEventArgs e)
		{
			foreach (var server in ServerInfo.Discover(AutoDiscoverPort, AutoDiscoverData))
			{
				if (server != null && server.Name != null && server.Name.Equals(Name))
				{
					Server = server;
					mServerDiscovered.Set();
					break;
				}
			}
		}
		#endregion StartAutoComponent

		#region ToString
		/// <summary>Gets the string representation of the client.</summary>
		/// <returns>A <see cref="string"/> with the name, and when available the IP address and port of the server.</returns>
		public override string ToString()
		{
			return Server != null ? string.Format("{0} ({1})", Name, Server.Endpoint) : Name;
		}
		#endregion ToString

		#endregion Methods
	}
}