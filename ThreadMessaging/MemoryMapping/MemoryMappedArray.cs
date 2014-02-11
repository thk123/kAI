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
	public class MemoryMappedArray
	{
		private MemoryMappedFileView view;
		private readonly int length;
		private readonly int bytesPerEntry;
		private readonly int headerOffset;
		private readonly int firstOffset;

		private enum ArrayStatus : byte
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
		/// <remarks>All members are neither thread-safe nor process-safe! It's up to you to synchronize access.</remarks>
		public MemoryMappedArray(MemoryMappedFileView view, int length, int bytesPerEntry, bool cooperative, int offset)
		{
			this.headerOffset = offset;
			this.firstOffset = 12 + offset;
			this.view = view;

			if(StatusOnline == ArrayStatus.Initialized)
			{	//Already exists. Check compatibility.
				if(LengthOnline != length || BytesPerEntryOnline != bytesPerEntry)
				{
					if(cooperative)
					{	//Accept other parameters.
						this.length = LengthOnline;
						this.bytesPerEntry = BytesPerEntryOnline;
						return;
					}
					else
						throw new MemoryMappedArrayFailedException();
				}
			}
			else
			{	//New Array. Initialize.
				int bytesNeeded = length * bytesPerEntry;
				if(bytesNeeded+firstOffset > view.Size)
					throw new MemoryMappedArrayFailedException();
				LengthOnline = length;
				BytesPerEntryOnline = bytesPerEntry;
				StatusOnline = ArrayStatus.Initialized;
			}

			this.length = length;
			this.bytesPerEntry = bytesPerEntry;
		}

		public int Length
		{
			get {return length;}
		}

		public int BytesPerEntry
		{
			get {return bytesPerEntry;}
		}

		private ArrayStatus StatusOnline
		{
			get	{return (ArrayStatus)view.ReadByte(headerOffset);}
			set	{view.WriteByte((byte)value,headerOffset);}
		}
		private int LengthOnline
		{
			get {return view.ReadInt32(headerOffset+4);}
			set {view.WriteInt32(value,headerOffset+4);}
		}
		private int BytesPerEntryOnline
		{
			get {return view.ReadInt32(headerOffset+8);}
			set {view.WriteInt32(value,headerOffset+8);}
		}

		private int IndexToOffset(int index, int offset)
		{
			if(index > length || index < 0)
				throw new IndexOutOfRangeException();
			return firstOffset + offset + index*bytesPerEntry;
		}

		public byte this[int index, int offset]
		{
			get
			{
				if(offset > bytesPerEntry || offset < 0)
					throw new ArgumentOutOfRangeException("offset",offset,"Must not be negative or greater than the specified bytesPerEntry (here: "+bytesPerEntry.ToString()+")");
				return view.ReadByte(IndexToOffset(index,offset));
			}
			set
			{
				if(offset > bytesPerEntry || offset < 0)
					throw new ArgumentOutOfRangeException("offset",offset,"Must not be negative or greater than the specified bytesPerEntry (here: "+bytesPerEntry.ToString()+")");
				view.WriteByte(value,IndexToOffset(index,offset));
			}
		}

		#region Accessors
		public byte ReadByte(int index, int offset)
		{
			return view.ReadByte(IndexToOffset(index,offset));
		}
		public void WriteByte(byte data, int index, int offset)
		{
			view.WriteByte(data,IndexToOffset(index,offset));
		}

		public void ReadBytes(byte[] data, int index)
		{
			if(data.Length > bytesPerEntry)
				throw new ArgumentOutOfRangeException("data",data.Length,"Length must not be greater than the specified bytesPerEntry (here: "+bytesPerEntry.ToString()+")");
			view.ReadBytes(data,IndexToOffset(index,0));
		}
		public void WriteBytes(byte[] data, int index)
		{
			if(data.Length > bytesPerEntry)
				throw new ArgumentOutOfRangeException("data",data.Length,"Length must not be greater than the specified bytesPerEntry (here: "+bytesPerEntry.ToString()+")");
			view.WriteBytes(data,IndexToOffset(index,0));
		}

		public short ReadInt16(int index, int offset)
		{
			return view.ReadInt16(IndexToOffset(index,offset));
		}
		public void WriteInt16(short data, int index, int offset)
		{
			view.WriteInt16(data,IndexToOffset(index,offset));
		}

		public int ReadInt32(int index, int offset)
		{
			return view.ReadInt32(IndexToOffset(index,offset));
		}
		public void WriteInt32(int data, int index, int offset)
		{
			view.WriteInt32(data,IndexToOffset(index,offset));
		}

		public long ReadInt64(int index, int offset)
		{
			return view.ReadInt64(IndexToOffset(index,offset));
		}
		public void WriteInt64(long data, int index, int offset)
		{
			view.WriteInt64(data,IndexToOffset(index,offset));
		}

		public object ReadDeserialize(int index, int offset)
		{
			return view.ReadDeserialize(IndexToOffset(index,offset),bytesPerEntry);
		}
		public void WriteSerialize(object data, int index, int offset)
		{
			view.WriteSerialize(data,IndexToOffset(index,offset),bytesPerEntry);
		}
		#endregion
	}
}
