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
					int temp = serialPort.ReadByte();
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

							if (packet.receiverID == 0xEE && packet.type == 0x21) {
								UpdateLog("[물체 탐지됨] [ID : 0x" + packet.sourceID.ToString("X2") + "] [level : " + packet.sourceLevel.ToString("D") + "]", this.textBox_log);
								try {
									subWindow?.Blink(packet);
								}
								catch (Exception) {
									// ignored
								}
							}
							if (packet.receiverID == 0xEE && packet.type == 0x12) {
								UpdateLog("SINK 노드로부터 초기화 ACK 수신함", this.textBox_systemLog);
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
				portWindow.OnOkButtonClickEvent -= new PortWindow.OnOkButtonClickHandler(portWindow_OnOKButtonClickEvent);
				portWindow.OnCancelButtonClickEvent -= new PortWindow.OnCancelButtonClickHandler(portWindow_OnCancelButtonClickEvent);
				portWindow.WindowClosedEvent -= new PortWindow.WindowClosedHandler(portWindow_WindowClosedEvent);
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
				nodeWindow.OnOKButtonClickEvent -= new NodeWindow.OnOKButtonClickHandler(nodeWindow_OnOKButtonClickEvent);
				nodeWindow.OnCancelButtonClickEvent -= new NodeWindow.OnCancelButtonClickHandler(nodeWindow_OnCancelButtonClickEvent);
				nodeWindow.WindowClosedEvent -= new NodeWindow.WindowClosedHandler(nodeWindow_WindowClosedEvent);
				nodeWindow = null;
			}
		}
	}
}