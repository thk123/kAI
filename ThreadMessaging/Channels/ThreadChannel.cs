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
using System.Collections;

namespace ThreadMessaging
{
	/// <summary>
	/// ThreadChannel is a queued Inter-Thread message channel.
	/// Any objects are allowed to be transfered through this channel.
	/// </summary>
	/// <remarks>All members of this class are thread-safe. </remarks>
	public sealed class ThreadChannel : ThreadReliability, IChannel
	{
		private Queue queue;
		private ThreadSemaphore empty, full;

		/// <summary>
		/// Instanciate a new Inter-Thread message channel.
		/// </summary>
		/// <param name="size">The count of messages the channel can queue before it blocks.</param>
		public ThreadChannel(int size)
		{
			queue = Queue.Synchronized(new Queue(size));
			empty = new ThreadSemaphore(size,size);
			full = new ThreadSemaphore(0,size);
		}

		/// <summary>
		/// Send a message to the channel with unlimited timeout.
		/// </summary>
		/// <param name="item">The object to send. Any objects are allowed to be transfered through this channel.</param>
		/// <remarks>This member is thread-safe.</remarks>
		public void Send(object item)
		{
			try {empty.Acquire();}
			catch(System.Threading.ThreadInterruptedException e)
			{
				DumpItem(item);
				throw e;
			}
			queue.Enqueue(item);
			full.Release();
		}

		/// <summary>
		/// Send a message to the channel with limited timeout.
		/// </summary>
		/// <param name="item">The object to send. Any objects are allowed to be transfered through this channel.</param>
		/// <param name="timeout">The maximum blocking time. A <see cref="SemaphorFailedException"/> is thrown if a timout exceeds.</param>
		/// <remarks>This member is thread-safe.</remarks>
		public void Send(object item, TimeSpan timeout)
		{
			try	{empty.Acquire(timeout);}
			catch(System.Threading.ThreadInterruptedException e)
			{
				DumpItemSynchronized(item);
				throw e;
			}
			queue.Enqueue(item);
			full.Release();
		}

		/// <summary>
		/// Receive a message from the channel with unlimited timeout.
		/// </summary>
		/// <returns>The received object.</returns>
		/// <remarks>This member is thread safe.</remarks>
		public object Receive()
		{
			full.Acquire();
			object item = queue.Dequeue();
			empty.Release();
			return item;
		}

		/// <summary>
		/// Receive a message from the channel with limited timeout.
		/// </summary>
		/// <param name="timeout">The maximum blocking time. A <see cref="SemaphorFailedException"/> is thrown if a timout exceeds.</param>
		/// <returns>The received object.</returns>
		/// <remarks>This member is thread safe.</remarks>
		public object Receive(TimeSpan timeout)
		{
			full.Acquire(timeout);
			object item = queue.Dequeue();
			empty.Release();
			return item;
		}

		protected override void DumpStructure()
		{
			lock(queue.SyncRoot)
			{
				foreach(object item in queue)
					DumpItem(item);
				queue.Clear();
			}
		}
	}
}