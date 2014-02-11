#region Copyright 2004 Christoph Daniel Rüegg [Modified BSD License]
/*
ThreadMessaging.NET, InterThread/-Process Communication Framework.
Copyright (c) 2004, Christoph Daniel Rueegg, http://cdrnet.net/.
All rights reserved.

[Modified BSD License]

Redistribution and use in source and binary forms, with or without modification,
are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice,
this list of conditions and the following disclaimer. 

2. Redistributions in binary form must reproduce the above copyright notice,
this list of conditions and the following disclaimer in the documentation
and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

using System;
using System.Threading;

namespace ThreadMessaging
{
	/// <summary>
	/// SingleRunnable provides simple management of a threaded blocking loop implementations.
	/// </summary>
	public abstract class SingleRunnable : IRunnable
	{
		private Thread thread;
		/// <summary>Use this variable in your infinite while loop condition.</summary>
		protected bool running = false;
		private bool aborted = false;
		private readonly bool interruptOnStop;
		private ThreadRendezVous sync;

		/// <summary>Initialize the runnable base class.</summary>
		//protected SingleRunnable() : this(true,false,false) {}
		/// <summary>Initialize the runnable base class.</summary>
		/// <param name="interruptOnStop">If true, an interrupt is fired on <see cref="IRunnable.Stop"/></param>
		/// <param name="autoStart">If true, the runnably is started automatically.</param>
		//protected SingleRunnable(bool interruptOnStop, bool autoStart) : this(interruptOnStop,autoStart,false) {}
		/// <summary>Initialize the runnable base class.</summary>
		/// <param name="interruptOnStop">If true, an interrupt is fired on <see cref="IRunnable.Stop"/></param>
		/// <param name="autoStart">If true, the runnably is started automatically.</param>
		/// <param name="waitOnStop">If true, <see cref="IRunnable.Stop"/> waits until the thread really finished.</param>
		protected SingleRunnable(bool interruptOnStop, bool autoStart, bool waitOnStop)
		{
			if(waitOnStop)
				this.sync = new ThreadRendezVous();
			this.sync = sync;
			this.interruptOnStop = interruptOnStop;
			if(autoStart)
				Start();
		}

		/// <summary>
		/// Override this method to implement your loop. Check for thread interrupts is recommended.
		/// </summary>
		protected abstract void Run();

		private void ThreadRun()
		{
			try {Run();}
			catch(System.Threading.ThreadInterruptedException) {}
		}

		/// <summary>
		/// Start the runnable.
		/// </summary>
		public virtual void Start()
		{
			if(aborted)
				throw new InvalidOperationException();
			if(thread == null)
			{
				running = true;
				thread = new Thread(new ThreadStart(this.ThreadRun));
				thread.Start();
			}
		}

		/// <summary>
		/// Stop the runnable. This method tells the thread to finish it's work and waits until the thread really finishes if sync is enabled.
		/// </summary>
		public virtual void Stop()
		{
			if(aborted)
				throw new InvalidOperationException();
			if(thread != null)
			{
				running = false;
				if(interruptOnStop)
					thread.Interrupt();
				thread.Join();
				//if(sync != null) //wait for the thread.
				//	sync.RendezVous();
			}
		}

		/// <summary>
		/// Stop the runnable. This method forces the threads to abort and propably leaves data in an invalid state. It won't wait until the thread really aborts.
		/// </summary>
		public virtual void ForceAbort()
		{
			if(thread != null)
			{
				aborted = true;
				running = false;
				thread.Abort();
				thread = null;
			}
		}
	}
}
