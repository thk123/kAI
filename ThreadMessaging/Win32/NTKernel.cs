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
	internal class NTKernel
	{
		#region Data Structures

		[StructLayout(LayoutKind.Sequential)]
		internal class SecurityAttributes
		{
			public SecurityAttributes(object securityDescriptor)
			{
				this.lpSecurityDescriptor = securityDescriptor;
			}

			uint nLegnth = 12;
			object lpSecurityDescriptor;
			[MarshalAs(UnmanagedType.VariantBool)]
			bool bInheritHandle = true;
		}

		#endregion

		#region General

		[DllImport("kernel32",EntryPoint="CloseHandle",SetLastError=true,CharSet=CharSet.Unicode)]
		[return : MarshalAs( UnmanagedType.VariantBool )]
		internal static extern bool CloseHandle(uint hHandle);

		[DllImport("kernel32",EntryPoint="GetLastError",SetLastError=true,CharSet=CharSet.Unicode)]
		internal static extern uint GetLastError();

		#endregion

		#region Semaphore

		[DllImport("kernel32",EntryPoint="CreateSemaphore",SetLastError=true,CharSet=CharSet.Unicode)]
		internal static extern uint CreateSemaphore(SecurityAttributes auth, int initialCount, int maximumCount, string name);

		[DllImport("kernel32",EntryPoint="WaitForSingleObject",SetLastError=true,CharSet=CharSet.Unicode)]
		internal static extern uint WaitForSingleObject(uint hHandle, uint dwMilliseconds);

		[DllImport("kernel32",EntryPoint="ReleaseSemaphore",SetLastError=true,CharSet=CharSet.Unicode)]
		[return : MarshalAs( UnmanagedType.VariantBool )]
		internal static extern bool ReleaseSemaphore(uint hHandle, int lReleaseCount, out int lpPreviousCount);

		#endregion

		#region Memory Mapped Files

		[DllImport("Kernel32.dll",EntryPoint="CreateFileMapping",SetLastError=true,CharSet=CharSet.Unicode)]
		internal static extern IntPtr CreateFileMapping(uint hFile, SecurityAttributes lpAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);
		
		[DllImport("Kernel32.dll",EntryPoint="OpenFileMapping",SetLastError=true,CharSet=CharSet.Unicode)]
		internal static extern IntPtr OpenFileMapping(uint dwDesiredAccess, bool bInheritHandle, string lpName);
		
		[DllImport("Kernel32.dll",EntryPoint="MapViewOfFile",SetLastError=true,CharSet=CharSet.Unicode)]
		internal static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);
		
		[DllImport("Kernel32.dll",EntryPoint="UnmapViewOfFile",SetLastError=true,CharSet=CharSet.Unicode)]
		[return : MarshalAs( UnmanagedType.VariantBool )]
		internal static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
		
		#endregion
	}
}
