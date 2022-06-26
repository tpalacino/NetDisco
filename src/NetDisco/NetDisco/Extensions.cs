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
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetDisco
{
	/// <summary>Defines extension methods used by this assembly.</summary>
	internal static class Extensions
	{
		#region Methods

		#region ToObject
		/// <summary>Deserializes the specified data to an object of the specified type.</summary>
		/// <typeparam name="T">The type of object to create.</typeparam>
		/// <param name="data">The binary data with the JSON content to deserialize.</param>
		/// <returns>The deserialized object or the system defined default value for the specified type.</returns>
		internal static T ToObject<T>(this byte[] data)
		{
			return data.ToObject<T>(default(T));
		}
		#endregion ToObject

		#region ToObject
		/// <summary>Converts the specified data to a string and deserializes it to an object of the specified type.</summary>
		/// <typeparam name="T">The type of object to deserialize.</typeparam>
		/// <param name="data">The binary data with the JSON content to deserialize.</param>
		/// <param name="defaultValue">The value to return if there is no data or an error occurs.</param>
		/// <returns>The deserialized object or the specified default value.</returns>
		internal static T ToObject<T>(this byte[] data, T defaultValue)
		{
			T retVal = defaultValue;

			if (data != null && data.Length > 0)
			{
				try
				{
					retVal = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(data));
				}
				catch (Exception ex)
				{
					Logger.Write(LogLevel.Error, "An error occurred converting a byte array to an object. Error: {0}", JsonConvert.SerializeObject(ex));
				}
			}

			return retVal;
		}
		#endregion ToObject

		#region ToByteArray
		/// <summary>Serializes the specified object to JSON and converts it to binary.</summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="obj">The object to serialize and convert to binary.</param>
		/// <returns>The binary representation of the JSON conversion of the specified object or an empty byte array.</returns>
		internal static byte[] ToByteArray<T>(this T obj)
		{
			return obj.ToByteArray(new byte[0]);
		}
		#endregion ToByteArray

		#region ToByteArray
		/// <summary>Serializes the specified object to JSON and converts it to binary.</summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="obj">The object to serialize and convert to binary.</param>
		/// <param name="defaultValue">The value to return if there is no object or an error occurs.</param>
		/// <returns>The binary representation of the JSON conversion of the specified object or the specified default value.</returns>
		internal static byte[] ToByteArray<T>(this T obj, byte[] defaultValue)
		{
			byte[] retVal = defaultValue;

			if (obj != null)
			{
				try
				{
					retVal = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
				}
				catch (Exception ex)
				{
					Logger.Write(LogLevel.Error, "An error occurred converting an object to a byte array. Error: {0}", JsonConvert.SerializeObject(ex));
				}
			}

			return retVal;
		}
		#endregion ToByteArray

		#region SafeSend
		/// <summary>Attempts to use the specified client to send the specified data to the specified endpoint.</summary>
		/// <param name="client">The client to use.</param>
		/// <param name="data">The data to send.</param>
		/// <param name="endpoint">The endpoint to send the data to.</param>
		/// <returns>An <see cref="int"/> with the number of bytes sent.</returns>
		internal static int SafeSend(this UdpClient client, byte[] data, IPEndPoint endpoint)
		{
			int retVal = 0;

			if (client != null && data != null && endpoint != null)
			{
				try
				{
					retVal = client.Send(data, data.Length, endpoint);
				}
				catch (Exception ex)
				{
					Logger.Write(LogLevel.Error, "An error occurred sending {0} bytes to the endpoint {1}:{2}. Error: {3}", data.Length, endpoint.Address, endpoint.Port, JsonConvert.SerializeObject(ex));
				}
			}

			return retVal;
		}
		#endregion SafeSend

		#region SafeReceive
		/// <summary>Attempts to use the specified client to receive data from the specified endpoint.</summary>
		/// <param name="client">The client to use.</param>
		/// <param name="endpoint">The endpoint to receive data from.</param>
		/// <returns>A array of bytes with the received data or an empty array.</returns>
		internal static byte[] SafeReceive(this UdpClient client, ref IPEndPoint endpoint)
		{
			byte[] retVal = null;

			if (client != null && endpoint != null)
			{
				try
				{
					retVal = client.Receive(ref endpoint);
				}
				catch (Exception ex)
				{
					Logger.Write(LogLevel.Error, "An error occurred receiving data from the endpoint {0}:{1}. Error: {2}", endpoint.Address, endpoint.Port, JsonConvert.SerializeObject(ex));
				}
			}

			return retVal;
		}
		#endregion SafeReceive

		#endregion Methods
	}
}