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
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace NetDisco
{
	/// <summary>Represents a discovered server.</summary>
	public class ServerInfo
	{
		#region Constructors

		/// <summary>Creates a new instance of <see cref="ServerInfo"/>.</summary>
		private ServerInfo() { }

		#endregion Constructors

		#region Properties

		#region Name
		/// <summary>The name of the server.</summary>
		public string Name { get; private set; }
		#endregion Name

		#region Endpoint
		/// <summary>The IP endpoint of the server.</summary>
		public IPEndPoint Endpoint { get; private set; }
		#endregion Endpoint

		#endregion Properties

		#region Methods

		#region Discover
		/// <summary>Enumerates the auto discoverable servers on the network using the default data on the default port.</summary>
		/// <returns>An <see cref="IEnumerable{ServerInfo}"/> that includes information about the server.</returns>
		public static IEnumerable<ServerInfo> Discover()
		{
			return Discover(Constants.DefaultAutoDiscoverPort, Constants.DefaultAutoDiscoverData);
		}
		#endregion Discover

		#region Discover
		/// <summary>Enumerates the auto discoverable servers on the network by sending the specified data on the default.</summary>
		/// <param name="data">The data the server will use to identify auto discover requests.</param>
		/// <returns>An <see cref="IEnumerable{ServerInfo}"/> that includes information about the server.</returns>
		public static IEnumerable<ServerInfo> Discover(byte[] data)
		{
			return Discover(Constants.DefaultAutoDiscoverPort, data);
		}
		#endregion Discover

		#region Discover
		/// <summary>Enumerates the auto discoverable servers on the network using the default data on the specified port.</summary>
		/// <param name="port">The port the server will use to listen for auto discover requests.</param>
		/// <returns>An <see cref="IEnumerable{ServerInfo}"/> that includes information about the server.</returns>
		public static IEnumerable<ServerInfo> Discover(int port)
		{
			return Discover(port, Constants.DefaultAutoDiscoverData);
		}
		#endregion Discover

		#region Discover
		/// <summary>Enumerates the auto discoverable servers on the network using the specified data on the specified port.</summary>
		/// <param name="port">The port the server will use to listen for auto discover requests.</param>
		/// <param name="data">The data the server will use to identify auto discover requests.</param>
		/// <returns>An <see cref="IEnumerable{ServerInfo}"/> that includes information about the server.</returns>
		public static IEnumerable<ServerInfo> Discover(int port, byte[] data)
		{
			IPEndPoint broadcast = new IPEndPoint(IPAddress.Broadcast, port);
			UdpClient connection = new UdpClient() { EnableBroadcast = true };

			try
			{
				connection.SafeSend(data, broadcast);

				byte[] responseData = connection.SafeReceive(ref broadcast);

				while (responseData != null)
				{
					var info = Create(responseData);
					if (info != null)
					{
						yield return info;
					}

					broadcast = new IPEndPoint(IPAddress.Broadcast, port);
					responseData = connection.SafeReceive(ref broadcast);
				}
			}
			finally
			{
				try { connection.Close(); }
				catch (Exception ex)
				{
					Logger.Write(LogLevel.Error, "An error occurred closing the auto-discovery connection. Error: {0}", ex);
				}
			}
		}
		#endregion Discover

		#region Create
		/// <summary>Reads the server information from the specified data.</summary>
		/// <param name="data">The data with server information.</param>
		/// <returns>A <see cref="ServerInfo"/> object or null.</returns>
		private static ServerInfo Create(byte[] data)
		{
			ServerInfo retVal = null;

			if (data != null && data.Length > 0)
			{
				try
				{
					var info = new { Name = string.Empty, Address = string.Empty, Port = 0 };
					info = data.ToObject(info);

					if (info != null && IPAddress.TryParse(info.Address, out IPAddress ipAddress))
					{
						retVal = new ServerInfo
						{
							Name = info.Name,
							Endpoint = new IPEndPoint(ipAddress, info.Port)
						};
					}
				}
				catch (Exception ex)
				{
					Logger.Write(LogLevel.Error, "An error occurred reading server info from the data. Error: {0}", ex);
				}
			}

			return retVal;
		}
		#endregion Create

		#endregion Methods
	}
}