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
using System.Collections.Specialized;

namespace ThreadMessaging.Server
{
	public class Sessions
	{
		private Hashtable sessions;
		private readonly TimeSpan timeout;
		private System.Timers.Timer timer;

		private class SessionItem
		{
			public Session Session;
			public DateTime LastAccessed;
			public SessionItem(Session session)
			{
				this.Session = session;
				this.LastAccessed = DateTime.Now;
			}
			public SessionItem(Guid guid)
			{
				this.Session.ID = guid;
				this.LastAccessed = DateTime.Now;
			}
			public void Touch()
			{
				this.LastAccessed = DateTime.Now;
			}
		}

		public Sessions(TimeSpan timeout)
		{
			this.sessions = Hashtable.Synchronized(new Hashtable(32));
			this.timeout = timeout;
			this.timer = new System.Timers.Timer(timeout.TotalMilliseconds/2d);
			this.timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
		}

		public void StartAutoCleanUp()
		{
			this.timer.Start();
		}

		public void StopAutoCleanUp()
		{
			this.timer.Stop();
		}

		public Session Create()
		{
			lock(sessions.SyncRoot)
			{
				Guid guid = Guid.NewGuid();
				while(sessions.ContainsKey(guid))
					guid = Guid.NewGuid();
				return Create(guid);
			}
		}

		private Session Create(Guid guid)
		{
			SessionItem item = new SessionItem(guid);
			sessions.Add(guid,item);
			return item.Session;
		}

		public void Touch(Session session)
		{
			lock(sessions.SyncRoot)
			{
				if(!sessions.ContainsKey(session.ID))
					session = Create(session.ID);
				((SessionItem)sessions[session.ID]).Touch();
			}
		}

		public void Remove(Session session)
		{
			sessions.Remove(session.ID);
		}

		public void CleanUp()
		{
			lock(sessions.SyncRoot)
			{
				DateTime limit = DateTime.Now.Subtract(timeout);
				ArrayList removeList = new ArrayList(sessions.Count);
				foreach(DictionaryEntry entry in sessions)
				{
					SessionItem item = (SessionItem)entry.Value;
					if(limit > item.LastAccessed)
						removeList.Add(item.Session.ID);
				}
				foreach(Guid guid in removeList)
					sessions.Remove(guid);
			}
		}

		private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			CleanUp();
		}
	}
}
