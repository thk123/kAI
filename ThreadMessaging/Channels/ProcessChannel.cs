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
	/// ProcessChannel is a queued Inter-Process message channel.
	/// Only objects masked with the <see cref="System.SerializableAttribute"/> are allowed to be transfered through this channel.
	/// </summary>
	/// <remarks>All members of this class are thread-safe.</remarks>
	public sealed class ProcessChannel : ProcessReliability, IChannel, IDisposable
	{
		private MemoryMappedFile file;
		private MemoryMappedFileView view;
		private MemoryMappedQueue queue;
		private ProcessSemaphore empty, full, mutex;

		/// <summary>
		/// Instanciates an Inter-Process message channel.
		/// </summary>
		/// <param name="size">The count of messages the channel can queue before it blocks.</param>
		/// <param name="name">The channel's name. Must be the same for all instances using this channel.</param>
		/// <param name="maxBytesPerEntry">The maximum serialized message size in terms of bytes. Must be the same for all instances using this channel.</param>
		public ProcessChannel( int size, string name, int maxBytesPerEntry)
		{
			int fileSize = 64+size*maxBytesPerEntry;

			empty = new ProcessSemaphore(name+".EmptySemaphore.Channel",size,size);
			full = new ProcessSemaphore(name+".FullSemaphore.Channel",0,size);
			mutex = new ProcessSemaphore(name+".MutexSemaphore.Channel",1,1);
			file = MemoryMappedFile.CreateFile(name+".MemoryMappedFile.Channel",MemoryMappedFile.FileAccess.ReadWrite,fileSize);
			view = file.CreateView(0,fileSize,MemoryMappedFileView.ViewAccess.ReadWrite);
			queue = new MemoryMappedQueue(view,size,maxBytesPerEntry,true,0);
			if(queue.Length < size || queue.BytesPerEntry < maxBytesPerEntry)
				throw new MemoryMappedArrayFailedException();
		}

		/// <summary>
		/// Send a message to the channel with unlimited timeout.
		/// </summary>
		/// <param name="item">The object to send. Only objects masked with the <see cref="System.SerializableAttribute"/> are allowed to be transfered through this channel.</param>
		/// <remarks>This member is thread-safe.</remarks>
		public void Send(object item)
		{
			try {empty.Acquire();}
			catch(System.Threading.ThreadInterruptedException e)
			{
				DumpItemSynchronized(item);
				throw e;
			}
			try {mutex.Acquire();}
			catch(System.Threading.ThreadInterruptedException e)
			{
				DumpItemSynchronized(item);
				empty.Release();
				throw e;
			}
			queue.Enqueue();
			try {queue.WriteSerialize(item,0);}
			catch(Exception e)
			{
				queue.RollbackEnqueue();
				mutex.Release();
				empty.Release();
				throw e;
			}
			mutex.Release();
			full.Release();
		}

		/// <summary>
		/// Send a message to the channel with unlimited timeout.
		/// </summary>
		/// <param name="item">The object to send. Only objects masked with the <see cref="System.SerializableAttribute"/> are allowed to be transfered through this channel.</param>
		/// <param name="timeout">The maximum blocking time. A <see cref="SemaphorFailedException"/> is thrown if a timout exceeds.</param>
		/// <remarks>This member is thread-safe.</remarks>
		public void Send(object item, TimeSpan timeout)
		{
			try {empty.Acquire(timeout);}
			catch(System.Threading.ThreadInterruptedException e)
			{
				DumpItemSynchronized(item);
				throw e;
			}
			try {mutex.Acquire();}
			catch(System.Threading.ThreadInterruptedException e)
			{
				DumpItemSynchronized(item);
				empty.Release();
				throw e;
			}
			queue.Enqueue();
			try {queue.WriteSerialize(item,0);}
			catch(Exception e)
			{
				queue.RollbackEnqueue();
				mutex.Release();
				empty.Release();
				throw e;
			}
			mutex.Release();
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
			mutex.Acquire();
			object item;
			queue.Dequeue();
			try	{item = queue.ReadDeserialize(0);}
			catch(Exception e)
			{
				queue.RollbackDequeue();
				mutex.Release();
				full.Release();
				throw e;
			}
			mutex.Release();
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
			mutex.Acquire();
			object item;
			queue.Dequeue();
			try	{item = queue.ReadDeserialize(0);}
			catch(Exception e)
			{
				queue.RollbackDequeue();
				mutex.Release();
				full.Release();
				throw e;
			}
			mutex.Release();
			empty.Release();
			return item;
		}

		protected override void DumpStructure()
		{
			mutex.Acquire();
			byte[][] dmp = queue.DumpClearAll();
			for(int i=0;i<dmp.Length;i++)
				DumpItemSynchronized(dmp[i]);
			mutex.Release();
		}


		#region IDisposable Member

		/// <summary>
		/// Don't forget to dispose this object before destruction.
		/// </summary>
		public void Dispose()
		{
			view.Dispose();
			file.Dispose();
			empty.Dispose();
			full.Dispose();
			mutex.Dispose();
		}

		#endregion
	}
}