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
	/// ThreadMailBox is an Inter-Thread mailbox.
	/// A mailbox is a blocking single item container.
	/// </summary>
	/// <remarks>All members of this class are thread-safe. </remarks>
	public sealed class ThreadMailBox : IMailBox
	{
		private object content;
		private ThreadSemaphore empty, full;

		/// <summary>
		/// Instanciates a new Inter-Thread mailbox.
		/// </summary>
		public ThreadMailBox()
		{
			empty = new ThreadSemaphore(1,1);
			full = new ThreadSemaphore(0,1);
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
				object item = content;
				empty.Release();
				return item;
			}
			set 
			{
				empty.Acquire();
				content = value;
				full.Release();
			}
		}
	}
}
