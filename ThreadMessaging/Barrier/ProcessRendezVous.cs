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
	/// ProcessRendezVous is an Inter-Process rendez-vous implementation.
	/// A rendez-vous is a mechanism to make processes wait on a given point.
	/// </summary>
	/// <remarks>All members of this class are thread-safe. </remarks>
	public sealed class ProcessRendezVous : IRendezVous
	{
		private ProcessSemaphore local, remote;

		/// <summary>
		/// Instanciates an Inter-Process rendez-vous.
		/// </summary>
		/// <param name="localName">A name for the local synchronization. Must match the remote partner's remoteName but must NOT mucht your own remoteName.</param>
		/// <param name="remoteName">A name for the remote synchronization. Must match the temote partner's localName but must NOT mucht your own localName.</param>
		public ProcessRendezVous(string localName, string remoteName)
		{
			local = new ProcessSemaphore("RendezVous_"+localName,0,1);
			remote = new ProcessSemaphore("RendezVous_"+remoteName,0,1);
		}

		/// <summary>
		/// Block until another thread calls the same method.
		/// </summary>
		public void RendezVous()
		{
			remote.Release();
			local.Acquire();
		}
	}
}
