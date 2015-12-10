using System;
using System.Runtime.InteropServices;
using System.Windows.Controls;

namespace SensorNetworkManager_WPF {

	public partial class MainWindow {

		/* (구조체 -> 바이트배열)로 변환하는 함수 */
		public static byte[] StructureToByte(object obj) {
			int datasize = Marshal.SizeOf(obj);
			IntPtr buff = Marshal.AllocHGlobal(datasize);
			Marshal.StructureToPtr(obj, buff, false);
			byte[] data = new byte[datasize];
			Marshal.Copy(buff, data, 0, datasize);
			Marshal.FreeHGlobal(buff);

			return data;
		}
		/* (바이트배열 -> 구조체)로 변환하는 함수 */
		public static object ByteToStructure(byte[] data, Type type) {
			IntPtr buff = Marshal.AllocHGlobal(data.Length);
			Marshal.Copy(data, 0, buff, data.Length);
			object obj = Marshal.PtrToStructure(buff, type);
			Marshal.FreeHGlobal(buff);

			if (Marshal.SizeOf(obj) != data.Length)
				return null;
			return obj;
		}


		private void UpdateLog(string text, TextBox textBox) {
			DateTime currTime = DateTime.Now;
			string time = currTime.ToString("HH:mm:ss");

			Dispatcher.Invoke(new Action(delegate () {
				textBox.AppendText("[" + time + "]   " + text + "\n");
				textBox.ScrollToEnd();
			}));
		}
	}
}