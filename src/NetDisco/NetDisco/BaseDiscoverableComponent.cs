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

namespace NetDisco
{
	/// <summary>Provides derived classes with common auto-discoverable functionality.</summary>
	public abstract class BaseDiscoverableComponent : IDisposable
	{
		#region Member Variables

		/// <summary>The background worker for the user component.</summary>
		protected BackgroundWorker mUserWorker = null;

		/// <summary>The background worker for the auto-discoverable component.</summary>
		protected BackgroundWorker mAutoWorker = null;

		/// <summary>Indicates if the component is stopping.</summary>
		protected bool mIsStopping = false;

		#endregion Member Variables

		#region Constructors

		/// <summary>Creates a new instance of <see cref="BaseDiscoverableComponent"/>.</summary>
		public BaseDiscoverableComponent()
		{
			mUserWorker = new BackgroundWorker() { WorkerSupportsCancellation = true };
			mUserWorker.DoWork += StartUserComponent;

			mAutoWorker = new BackgroundWorker() { WorkerSupportsCancellation = true };
			mAutoWorker.DoWork += StartAutoComponent;
		}

		#endregion Constructors

		#region Properties

		#region AutoDiscoverPort
		/// <summary>The port where the server will listen for auto discovery requests. Default value is 18500.</summary>
		protected virtual int AutoDiscoverPort { get { return Constants.DefaultAutoDiscoverPort; } }
		#endregion AutoDiscoverPort

		#region AutoDiscoverData
		/// <summary>The data that will match the data sent in auto discovery requests.</summary>
		protected virtual byte[] AutoDiscoverData { get { return Constants.DefaultAutoDiscoverData; } }
		#endregion AutoDiscoverData

		#endregion Properties

		#region Methods

		#region Dispose
		/// <summary>Disposes the component.</summary>
		public void Dispose()
		{
			Stop();
		}
		#endregion Dispose

		#region Start
		/// <summary>Starts the component.</summary>
		public void Start()
		{
			if (mUserWorker != null && !mUserWorker.IsBusy)
			{
				mUserWorker.RunWorkerAsync();
			}
			if (mAutoWorker != null && !mAutoWorker.IsBusy)
			{
				mAutoWorker.RunWorkerAsync();
			}
		}
		#endregion Start

		#region StartUserComponent
		/// <summary>Starts the user component.</summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The arguments of the event.</param>
		protected abstract void StartUserComponent(object sender, DoWorkEventArgs e);
		#endregion StartUserComponent

		#region StartAutoComponent
		/// <summary>Starts the auto-discoverable component.</summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The arguments of the event.</param>
		protected abstract void StartAutoComponent(object sender, DoWorkEventArgs e);
		#endregion StartAutoComponent

		#region Stop
		/// <summary>Stops the server.</summary>
		public void Stop()
		{
			if (!mIsStopping)
			{
				mIsStopping = true;
				if (mUserWorker != null)
				{
					try
					{
						mUserWorker.CancelAsync();
						mUserWorker.Dispose();
					}
					finally
					{
						mUserWorker = null;
					}
				}
				if (mAutoWorker != null)
				{
					try
					{
						mAutoWorker.CancelAsync();
						mAutoWorker.Dispose();
					}
					finally
					{
						mAutoWorker = null;
					}
				}
			}
		}
		#endregion Stop

		#endregion Methods
	}
}