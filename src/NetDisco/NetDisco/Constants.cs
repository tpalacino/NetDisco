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

using System.Text;

namespace NetDisco
{
	/// <summary>Defines constant values used by this assembly.</summary>
	internal static class Constants
	{
		#region Member Variables

		/// <summary>The literal value 18500.</summary>
		internal const int DefaultAutoDiscoverPort = 18500;

		/// <summary>The literal value "ADR".</summary>
		internal const string DefaultAutoDiscoverString = "ADR";

		/// <summary>The UTF encoding of the DefaultAutoDiscoverString.</summary>
		internal static readonly byte[] DefaultAutoDiscoverData = Encoding.UTF8.GetBytes(DefaultAutoDiscoverString);

		#endregion Member Variables
	}
}