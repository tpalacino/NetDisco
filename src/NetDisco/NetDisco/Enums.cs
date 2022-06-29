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

namespace NetDisco
{
	#region LogLevel
	/// <summary>The possible log levels.</summary>
	public enum LogLevel
	{
		/// <summary>The Debug log level.</summary>
		Debug = 0,
		/// <summary>The Info log level.</summary>
		Info = 1,
		/// <summary>The Error log level.</summary>
		Error = 2,
		/// <summary>The Warn log level.</summary>
		Warn = 3,
		/// <summary>The Fatal log level.</summary>
		Fatal = 4
	}
	#endregion LogLevel
}