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
using System.Runtime.InteropServices;

namespace ThreadMessaging
{
	public class MemoryMappedFile : IDisposable
	{
		private IntPtr fileMapping;
		private readonly int size;
		private readonly FileAccess access;

		public enum FileAccess : int
		{
			ReadOnly = 2,
			ReadWrite = 4
		}

		private MemoryMappedFile(IntPtr fileMapping, int size, FileAccess access)
		{
			this.fileMapping = fileMapping;
			this.size = size;
			this.access = access;
		}
		
		/// <summary>
		/// Create a virtual memory mapped file located in the system page file.
		/// </summary>
		/// <param name="name">The name of the file. Prefix it with "Global\" or "Local\" to control its scope between NT services and user applications in Terminal Server scenarios.</param>
		/// <param name="access">Whether you need write access to the file.</param>
		/// <param name="size">The preferred size of the file in terms of bytes.</param>
		/// <returns>A MemoryMappedFile instance representing the file.</returns>
		public static MemoryMappedFile CreateFile(string name, FileAccess access, int size)
		{
			if(size < 0)
				throw new ArgumentException("Size must not be negative","size");

			//object descriptor = null;
			//NTAdvanced.InitializeSecurityDescriptor(out descriptor,1);
			//NTAdvanced.SetSecurityDescriptorDacl(ref descriptor,true,null,false);
			//NTKernel.SecurityAttributes sa = new NTKernel.SecurityAttributes(descriptor);

			IntPtr fileMapping = NTKernel.CreateFileMapping(0xFFFFFFFFu,null,(uint)access,0,(uint)size,name);
			if(fileMapping == IntPtr.Zero)
				throw new MemoryMappingFailedException();

			return new MemoryMappedFile(fileMapping,size,access);
		}

		/// <summary>
		/// Create a view of the memory mapped file, allowing to read/write bytes.
		/// </summary>
		/// <param name="offset">An optional offset to the file.</param>
		/// <param name="size">The size of the view in terms of bytes.</param>
		/// <param name="access">Whether you need write access to the view.</param>
		/// <returns>A MemoryMappedFileView instance representing the view.</returns>
		public MemoryMappedFileView CreateView(int offset, int size, MemoryMappedFileView.ViewAccess access)
		{
			if(this.access == FileAccess.ReadOnly && access == MemoryMappedFileView.ViewAccess.ReadWrite)
				throw new ArgumentException("Only read access to views allowed on files without write access","access");
			if(offset < 0)
				throw new ArgumentException("Offset must not be negative","size");
			if(size < 0)
				throw new ArgumentException("Size must not be negative","size");
			IntPtr mappedView = NTKernel.MapViewOfFile(fileMapping,(uint)access,0,(uint)offset,(uint)size);
			return new MemoryMappedFileView(mappedView,size,access);
		}

		#region IDisposable Member

		public void Dispose()
		{
			if(fileMapping != IntPtr.Zero)
			{
				if(NTKernel.CloseHandle((uint)fileMapping))
					fileMapping = IntPtr.Zero;
			}
		}

		#endregion
	}
}
