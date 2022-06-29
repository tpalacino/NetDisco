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

namespace NetDisco
{
	/// <summary>Defines the requirements for being an auto-discoverable server request.</summary>
	public interface IAutoDiscoverableServerRequest { }

	/// <summary>Defines the requirements for being an auto-discoverable server response.</summary>
	/// <typeparam name="TRequest">The type of auto-discoverable server request.</typeparam>
	public interface IAutoDiscoverableServerResponse<TRequest> where TRequest: IAutoDiscoverableServerRequest
	{
		#region Properties

		#region Request
		/// <summary>The auto-discoverable server request.</summary>
		TRequest Request { get; set; }
		#endregion Request

		#region Error
		/// <summary>The error processing the request.</summary>
		Exception Error { get; set; }
		#endregion Error

		#endregion Properties
	}
}