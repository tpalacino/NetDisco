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
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace NetDisco
{
	/// <summary>Helper to simplify writing log messages.</summary>
	public static class Logger
	{
		#region Events

		/// <summary>An event raised when a log message is written.</summary>
		public static event LogMessageWrittenEventHandler OnLogMessageWritten;

		#endregion Events

		#region Methods

		#region Write
		/// <summary>Writes a log message of the specified level.</summary>
		/// <param name="level">The level of the message to write.</param>
		/// <param name="message">The message to write.</param>
		/// <param name="args">If specified, will be passed as parameters to format the specified message.</param>
		public static void Write(LogLevel level, string message, params object[] args)
		{
			bool hasArgs = args != null && args.Length > 0, appliedFormat = false;
			try
			{
				var callingType = new StackTrace().GetFrame(1).GetMethod().DeclaringType;

				if (hasArgs)
				{
					message = string.Format(message, args);
					appliedFormat = true;
				}

				if (OnLogMessageWritten != null)
				{
					OnLogMessageWritten(level, message);
				}
				else
				{
					Console.WriteLine(message);
				}
			}
			catch (Exception ex)
			{
				var argsList = new List<object>() { message ?? "null" };
				if (hasArgs && !appliedFormat) { argsList.AddRange(args.Select(arg => arg ?? "null")); }
				message = string.Format("An error occurred writing the {0} log. Info: \"{1}\"; Error: {2}", level, string.Join(",", argsList), ex);
				Trace.WriteLine(message);
				Console.WriteLine(message);
			}
		}
		#endregion Write

		#endregion Methods

		/// <summary>A delegate used to raise a log message event.</summary>
		/// <param name="level">The level of the log message.</param>
		/// <param name="message">The message being written.</param>
		public delegate void LogMessageWrittenEventHandler(LogLevel level, string message);
	}
}