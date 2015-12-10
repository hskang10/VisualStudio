using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Centerprogram {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Packet {
		public byte type { get; set; }
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

		internal static Packet ByteToStructure(Type type) {
			throw new NotImplementedException();
		}
	}

	public class PacketInfo {
		public byte Type { get; set; }
		public uint Time { get; set; }
		public ulong Longitude { get; set; }
		public ulong Latitude { get; set; }
		public string Message { get; set; }
		public string FromIp { get; set; }
		public int Count { get; set; }


		public PacketInfo(int count, string fromIp, byte type, uint time, ulong longitude, ulong latitude, string message) {
			this.Count = count;
			this.FromIp = fromIp;
			this.Type = type;
			this.Time = time;
			this.Longitude = longitude;
			this.Latitude = latitude;
			this.Message = message;
		}
	}
	public class PacketList : ObservableCollection<PacketInfo> {}
}