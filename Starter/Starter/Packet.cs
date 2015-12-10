using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Starter {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Packet {
		public byte type;
		public uint time;
		public ulong longitude;
		public ulong latitude;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
		public string message;

		public static byte[] StructureToByte(object obj) {
			var datasize = Marshal.SizeOf(obj);
			var buff = Marshal.AllocHGlobal(datasize);
			Marshal.StructureToPtr(obj, buff, false);
			var data = new byte[datasize];
			Marshal.Copy(buff, data, 0, datasize);
			Marshal.FreeHGlobal(buff);

			return data;
		}
		/* (바이트배열 -> 구조체)로 변환하는 함수 */
		public static object ByteToStructure(byte[] data, Type type) {
			var buff = Marshal.AllocHGlobal(data.Length);
			Marshal.Copy(data, 0, buff, data.Length);
			var obj = Marshal.PtrToStructure(buff, type);
			Marshal.FreeHGlobal(buff);

			if (Marshal.SizeOf(obj) != data.Length)
				return null;
			return obj;
		}
	}
}