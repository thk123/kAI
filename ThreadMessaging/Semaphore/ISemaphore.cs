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

namespace ThreadMessaging
{
	/// <summary>
	/// A counting semaphore is a thread or process synchronization primitive.
	/// Semaphores are useful for Inter-Thread or Inter-Process synchronization and communication, depending on the implementation.
	/// Usually all interfaces members of this type are thread-safe.
	/// </summary>
	public interface ISemaphore
	{
		/// <summary>
		/// Blocks if no release is signaled or removes a release signal. Enter a critical section.
		/// In theory, this method is often called P() [Dijkstra, dutch: passeeren]
		/// </summary>
		void Acquire();

		/// <summary>
		/// Blocks if no release is signaled or removes a release signal. Enter a critical section.
		/// In theory, this method is often called P() [Dijkstra, dutch: passeeren]
		/// </summary>
		/// <param name="timeout">The maximum blocking time. Usually an Exceptions is thrown if a timeout exceeds.</param>
		void Acquire(TimeSpan timeout);

		/// <summary>
		/// Signals a release and allows a blocked thread to continue. Leave a critical section.
		/// In theory, this method is often called V() [Dijkstra, dutch: vrijgeven]
		/// </summary>
		void Release();
	}
}