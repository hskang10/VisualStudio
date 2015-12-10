using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace SensorNetworkManager_WPF {

	public partial class MainWindow {

		private void backgroundWorker_Listen_DoWork(object sender, DoWorkEventArgs e) {
			while (_isConnected) {
				Thread.Sleep(10);
				if (serialPort.BytesToRead > 0) {
					var temp = serialPort.ReadByte();
					if (temp == 0x02) {
						while (serialPort.BytesToRead <= 6) ;
						byte[] buffer = new byte[6];
						serialPort.Read(buffer, 0, 6);
						temp = serialPort.ReadByte();

						if (temp == 0x03) {
							//						string output = BitConverter.ToString(buffer);
							var packet = new Packet();
							packet = (Packet)ByteToStructure(buffer, packet.GetType());

//							UpdateLog(packet.ToString(), textBox_log);

							if (packet.receiverID == (byte)Id.Center && packet.type == (int)MessageType.Sensing) {
								UpdateLog("[물체 탐지됨] [ID : 0x" + packet.sourceID.ToString("X2") + "] [level : " + packet.sourceLevel.ToString("D") + "]", this.textBox_log);
								try {
									subWindow?.Blink(packet);
								}
								catch (Exception) {
									// ignored
								}
							}
						}
					}
				}
			}
		}

		private void backgroundWorker_Listen_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			serialPort.Close();
		}

		private void portWindow_OnOKButtonClickEvent(string portNumber) {
			_selectedCOMPort = portNumber;
			label_portSelect.Content = portNumber;
			portWindow?.Close();

			
		}

		private void portWindow_OnCancelButtonClickEvent() {
			portWindow?.Close();
		}

		private void portWindow_WindowClosedEvent() {
			if (portWindow != null) {
				portWindow.OnOkButtonClickEvent -= this.portWindow_OnOKButtonClickEvent;
				portWindow.OnCancelButtonClickEvent -= this.portWindow_OnCancelButtonClickEvent;
				portWindow.WindowClosedEvent -= this.portWindow_WindowClosedEvent;
				portWindow = null;
			}
		}

		private void nodeWindow_OnOKButtonClickEvent(int number) {
			numOfNodes = number;
			label_numNode.Content = number;
			subWindow.grid.Children.Clear();
			subWindow.Set_Node();
			nodeWindow?.Close();
		}

		private void nodeWindow_OnCancelButtonClickEvent() {
			nodeWindow?.Close();
		}

		private void nodeWindow_WindowClosedEvent() {
			if (nodeWindow != null) {
				nodeWindow.OnOKButtonClickEvent -= this.nodeWindow_OnOKButtonClickEvent;
				nodeWindow.OnCancelButtonClickEvent -= this.nodeWindow_OnCancelButtonClickEvent;
				nodeWindow.WindowClosedEvent -= this.nodeWindow_WindowClosedEvent;
				nodeWindow = null;
			}
		}
	}
}