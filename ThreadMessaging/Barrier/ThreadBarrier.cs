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
	/// ThreadBarrier is an Inter-Thread barrier implementation.
	/// A barrier is a mechanism to make threads wait on a given point.
	/// </summary>
	/// <remarks>All members of this class are thread-safe. </remarks>
	public sealed class ThreadBarrier : IBarrier
	{
		private readonly int count;
		private int index = 0;
		private ManualResetEvent[] events;

		/// <summary>
		/// Instanciates an Inter-Thread rendez-vous.
		/// </summary>
		public ThreadBarrier(int count)
		{
			this.count = count;
			this.events = new ManualResetEvent[count];
			for(int i=0;i<count;i++)
				events[i] = new ManualResetEvent(false);
		}

		/// <summary>
		/// Block until all other threads call the same method.
		/// </summary>
		public void Barrier()
		{
			ManualResetEvent are;
			lock(this)
			{
				if(index == 0)
				{
					for(int i=0;i<count;i++)
						events[i].Reset();
				}
				are = events[index++];
				if(index == count)
					index = 0;
			}
			are.Set();
			WaitHandle.WaitAll(events);
		}
	}
}
