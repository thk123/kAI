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

/*
This Win32 Semaphor Wrapper implementation is based on
a CodeProject Article by Robin Galloway-Lunn (Copyright 2002)
http://www.codeproject.com/csharp/sharetheresource.asp
Thanks for this great work!
*/

using System;

namespace ThreadMessaging
{
	/// <summary>
	/// ProcessSemaphore is an Inter-Process counting semaphore using Win32 Semaphores.
	/// Semaphores are useful for Inter-Process synchronization and communication.
	/// </summary>
	/// <remarks>All members of this class are thread-safe.</remarks>
	public class ProcessSemaphore : ISemaphore, IDisposable
	{
		private uint handle;
		private readonly uint interruptReactionTime;

		/// <summary>
		/// Instanciate a new Inter-Process semaphore.
		/// </summary>
		/// <param name="name">The semaphore's name.</param>
		public ProcessSemaphore(string name) : this(name,0,int.MaxValue,500) {}
		/// <summary>
		/// Instanciate a new Inter-Process semaphore.
		/// </summary>
		/// <param name="name">The semaphore's name.</param>
		/// <param name="initial">The initial count of releases signaled.</param>
		public ProcessSemaphore(string name, int initial) : this(name,initial,int.MaxValue,500) {}
		/// <summary>
		/// Instanciate a new Inter-Process semaphore.
		/// </summary>
		/// <param name="name">The semaphore's name.</param>
		/// <param name="initial">The initial count of releases signaled.</param>
		/// <param name="max">The maximum count of release signaled.</param>
		public ProcessSemaphore(string name, int initial, int max) : this(name,initial,max,500) {}
		/// <summary>
		/// Instanciate a new Inter-Process semaphore.
		/// </summary>
		/// <param name="name">The semaphore's name.</param>
		/// <param name="initial">The initial count of releases signaled.</param>
		/// <param name="max">The maximum count of release signaled.</param>
		/// <param name="interruptReactionTime">The maximum time [ms] needed to react to interrupts.</param>
		public ProcessSemaphore(string name, int initial, int max, int interruptReactionTime)
		{
			this.interruptReactionTime = (uint)interruptReactionTime;
			this.handle = NTKernel.CreateSemaphore(null, initial, max, name);
			if(handle == 0)
				throw new SemaphoreFailedException();
		}

		/// <summary>
		/// Blocks if no release is signaled or removes a release signal. Enter a critical section.
		/// In theory, this method is often called P() [Dijkstra, dutch: passeeren]
		/// </summary>
		/// <remarks>This member is thread-safe.</remarks>
		public void Acquire()
		{
			while(true)
			{ //looped 0.5s timeout to make blocked threads abortable.
				uint res = NTKernel.WaitForSingleObject(handle, interruptReactionTime);
				try	{System.Threading.Thread.Sleep(0);}
				catch(System.Threading.ThreadInterruptedException e)
				{
					if(res == 0)
					{
						int previousCount;
						NTKernel.ReleaseSemaphore(handle,1,out previousCount);
					}
					throw e;
				}
				if(res == 0)
					return;
				if(res != 258)
					throw new SemaphoreFailedException();
			}
		}

		/// <summary>
		/// Blocks if no release is signaled or removes a release signal. Enter a critical section.
		/// In theory, this method is often called P() [Dijkstra, dutch: passeeren]
		/// </summary>
		/// <param name="timeout">The maximum blocking time. Usually an Exceptions is thrown if a timeout exceeds.</param>
		/// <remarks>This member is thread-safe.</remarks>
		public void Acquire(TimeSpan timeout)
		{
			uint milliseconds = (uint)timeout.TotalMilliseconds;
			if(NTKernel.WaitForSingleObject(handle, milliseconds) != 0)
				throw new SemaphoreFailedException();	
		}

		/// <summary>
		/// Signals a release and allows a blocked thread to continue. Leave a critical section.
		/// In theory, this method is often called V() [Dijkstra, dutch: vrijgeven]
		/// </summary>
		/// <remarks>This member is thread-safe.</remarks>
		public void Release()
		{
			int previousCount;
			if(!NTKernel.ReleaseSemaphore(handle, 1, out previousCount))
				throw new SemaphoreFailedException();	
		}

		#region IDisposable Member

		/// <summary>
		/// Don't forget to dispose this object before destruction.
		/// </summary>
		public void Dispose()
		{
			if(handle != 0)
			{
				if(NTKernel.CloseHandle(handle))
					handle = 0;
			}
		}

		#endregion

	}
}
