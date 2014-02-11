#region Copyright 2004 Christoph Daniel R�egg [Modified BSD License]
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

	#region Event Details
	public class MessageReceivedEventArgs : EventArgs
	{
		private object content;
		public MessageReceivedEventArgs(object content)
		{
			this.content = content;
		}
		public object Content
		{
			get {return content;}
		}
	}
	public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);
	#endregion

	/// <remarks>All blocking event handlers should handle <see cref="System.Threading.ThreadInterruptedException"/> if appropriate.</remarks>
	public class ChannelEventGateway : SingleRunnable
	{
		private IChannel source;
		public event MessageReceivedEventHandler MessageReceived;

		public ChannelEventGateway(IChannel source, bool autoStart, bool waitOnStop) : base(true,autoStart,waitOnStop)
		{
			this.source = source;
		}

		protected override void Run()
		{
			while(running)
			{
				object c = source.Receive();
				MessageReceivedEventHandler handler = MessageReceived;
				if(handler != null)
					handler(this,new MessageReceivedEventArgs(c));
			}
		}
	}
}
