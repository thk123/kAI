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
	/// ProcessMailBox is an Inter-Process mailbox.
	/// A mailbox is a blocking single item container.
	/// </summary>
	/// <remarks>All members of this class are thread-safe. </remarks>
	public sealed class ProcessMailBox : IMailBox, IDisposable
	{
		private MemoryMappedFile file;
		private MemoryMappedFileView view;
		private ProcessSemaphore empty, full;

		/// <summary>
		/// Instanciate a new Interprocess Mailbox.
		/// </summary>
		/// <param name="name">The name for the Win32 semaphores and the shared memory file.</param>
		/// <param name="size">The size of the shared memory in terms of bytes.</param>
		public ProcessMailBox(string name,int size)
		{
			empty = new ProcessSemaphore(name+".EmptySemaphore.MailBox",1,1);
			full = new ProcessSemaphore(name+".FullSemaphore.MailBox",0,1);
			file = MemoryMappedFile.CreateFile(name+".MemoryMappedFile.MailBox",MemoryMappedFile.FileAccess.ReadWrite,size);
			view = file.CreateView(0,size,MemoryMappedFileView.ViewAccess.ReadWrite);
		}

		/// <summary>
		/// The content accessor. Blocking on getting if empty and on setting if full.
		/// </summary>
		/// <remarks>This member is thread-safe.</remarks>
		public object Content
		{
			get
			{
				full.Acquire();
				object item;
				try {item = view.ReadDeserialize();}
				catch(Exception e)
				{	//Rollback
					full.Release();
					throw e;
				}
				empty.Release();
				return item;
			}

			set 
			{
				empty.Acquire();
				try {view.WriteSerialize(value);}
				catch(Exception e)
				{	//Rollback
					empty.Release();
					throw e;
				}
				full.Release();
			}
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
		}

		#endregion
	}
}
