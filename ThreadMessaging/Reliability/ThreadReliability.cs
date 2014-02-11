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
using System.Collections;

namespace ThreadMessaging
{
	public abstract class ThreadReliability : IReliability
	{
		private DumpContainer container = new DumpContainer();

		protected abstract void DumpStructure();

		protected void DumpItemSynchronized(object item)
		{
			lock(container)
				container.Add(item);
		}

		protected void DumpItem(object item)
		{
			container.Add(item);
		}

		protected object DumpSyncRoot
		{
			get {return container;}
		}

		/// <summary>
		/// True if there's any data in the dump container.
		/// </summary>
		public bool IsDumped
		{
			get {return container.Count > 0; }
		}

		/// <summary>
		/// Copy all data hold by the structure to the local dump container and clears the structure.
		/// </summary>
		/// <remarks>A structure is invalid and no longer usable after dumping. You should notify all the other threads/processes before dumping.</remarks>
		public void Dump()
		{
			DumpStructure();
		}

		/// <summary>
		/// The dump container data structure
		/// </summary>
		public DumpContainer DumpContainer
		{
			get {return container;}
		}
	}
}
