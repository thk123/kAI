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
	/// <remarks>All members are neither thread-safe nor process-safe! It's up to you to synchronize access.</remarks>
	public class MemoryMappedQueue
	{
		private MemoryMappedArray array;
		private MemoryMappedFileView view;
		private readonly int length;
		private readonly int headerOffset;
		private int currentIn, currentOut;

		private enum QueueStatus : byte
		{
			Uninitialized = 0,
			Initialized = 1
		}

		/// <summary>
		/// Instanciate a new memory mapped array for inter-process access.
		/// </summary>
		/// <param name="view">The memory mapped file view.</param>
		/// <param name="length">The count of entries of the array.</param>
		/// <param name="bytesPerEntry">The (maximal) count of bytes per entry.</param>
		/// <param name="cooperative">
		/// If true, it accepts other length and bytesPerEntry if the array already exists.
		/// If falls it throws an exception if the array already exists but with other length or bytesPerEntry.
		/// </param>
		/// <param name="offset">An optional initial positive offset before the array starts.</param>
		public MemoryMappedQueue(MemoryMappedFileView view, int length, int bytesPerEntry, bool cooperative, int offset)
		{
			this.headerOffset = offset;
			this.view = view;
			this.array = new MemoryMappedArray(view,length,bytesPerEntry,cooperative,offset+16);
			this.length = array.Length;

			if(StatusOnline != QueueStatus.Initialized)
			{
				InOnline = 0;
				OutOnline = 0;
				CountOnline = 0;
				StatusOnline = QueueStatus.Initialized;
			}
		}

		public int Length
		{
			get {return length;}
		}
		public int BytesPerEntry
		{
			get {return array.BytesPerEntry;}
		}

		private QueueStatus StatusOnline
		{
			get	{return (QueueStatus)view.ReadByte(headerOffset);}
			set	{view.WriteByte((byte)value,headerOffset);}
		}
		private int InOnline
		{
			get {return view.ReadInt32(headerOffset+4);}
			set {view.WriteInt32(value,headerOffset+4);}
		}
		private int OutOnline
		{
			get {return view.ReadInt32(headerOffset+8);}
			set {view.WriteInt32(value,headerOffset+8);}
		}
		private int CountOnline
		{
			get {return view.ReadInt32(headerOffset+12);}
			set {view.WriteInt32(value,headerOffset+12);}
		}

		public void Enqueue()
		{
			int count = CountOnline;
			if(count == length)
				throw new InvalidOperationException();
			CountOnline = count+1;
			currentIn = InOnline;
			InOnline = (currentIn+1) % length;
		}
		public void RollbackEnqueue()
		{
			CountOnline = CountOnline-1;
			currentIn = InOnline-2;
			if(currentIn < 0)
				currentIn += length;
			InOnline = (currentIn+1) % length;
		}

		public void Dequeue()
		{
			int count = CountOnline;
			if(count == 0)
				throw new InvalidOperationException();
			CountOnline = count-1;
			currentOut = OutOnline;
			OutOnline = (currentOut+1) % length;
		}
		public void RollbackDequeue()
		{
			CountOnline = CountOnline+1;
			currentOut = OutOnline-2;
			if(currentOut < 0)
				currentOut += length;
			OutOnline = (currentOut+1) % length;
		}

		public byte[][] DumpClearAll()
		{
			int cnt = CountOnline;
			byte[][] ret = new byte[cnt][];
			for(int i=0;i<cnt;i++)
			{
				ret[i] = new byte[BytesPerEntry];
				Dequeue();
				ReadBytes(ret[i]);
			}
			return ret;
		}

		public byte this[int offset]
		{
			get {return array[currentOut,offset];}
			set {array[currentIn,offset] = value;}
		}

		#region Accessors
		public byte ReadByte(int offset)
		{
			return array.ReadByte(currentOut,offset);
		}
		public void WriteByte(byte data, int offset)
		{
			array.WriteByte(data,currentIn,offset);
		}

		public void ReadBytes(byte[] data)
		{
			array.ReadBytes(data,currentOut);
		}
		public void WriteBytes(byte[] data)
		{
			array.WriteBytes(data,currentIn);
		}

		public short ReadInt16(int offset)
		{
			return array.ReadInt16(currentOut,offset);
		}
		public void WriteInt16(short data, int offset)
		{
			array.WriteInt16(data,currentIn,offset);
		}

		public int ReadInt32(int offset)
		{
			return array.ReadInt32(currentOut,offset);
		}
		public void WriteInt32(int data, int offset)
		{
			array.WriteInt32(data,currentIn,offset);
		}

		public long ReadInt64(int offset)
		{
			return array.ReadInt64(currentOut,offset);
		}
		public void WriteInt64(long data, int offset)
		{
			array.WriteInt64(data,currentIn,offset);
		}

		public object ReadDeserialize(int offset)
		{
			return array.ReadDeserialize(currentOut,offset);
		}
		public void WriteSerialize(object data, int offset)
		{
			array.WriteSerialize(data,currentIn,offset);
		}
		#endregion
	}
}
