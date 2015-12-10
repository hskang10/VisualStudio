using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Centerprogram {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private Socket _socket1;
		private Socket _socket2;
		EndPoint _ip1;
		EndPoint _ip2;
		private readonly BackgroundWorker _backgroundWorker;
		bool _isConnected;
		Packet packet;
		PacketList packetList;

		int count = 1;

		public MainWindow() {
			InitializeComponent();
			Console.WriteLine("Start");

			packetList = this.Resources["MyPacketList"] as PacketList;

			this._backgroundWorker = new BackgroundWorker();
			this._backgroundWorker.DoWork += this.backgroundWorker_DoWork;
		}

		private void button_Click(object sender, RoutedEventArgs e) {
			try {
				this._socket1 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				this._socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

				this._ip1 = new IPEndPoint(IPAddress.Parse(this.textBox_ip1.Text), Convert.ToInt32(this.textBox_port.Text));
				this._ip2 = new IPEndPoint(IPAddress.Any, Convert.ToInt32(this.textBox_port.Text));

				this._socket1.Connect(this._ip1); // 송신
				this._socket2.Bind(this._ip2); // 수신

				this._backgroundWorker.RunWorkerAsync();
				this._isConnected = true;
				this.textBlock.Text = "연결됨";
				this.button1.IsEnabled = true;
				this.button.IsEnabled = false;
				this.textBox_ip1.IsEnabled = false;
				this.textBox_ip2.IsEnabled = false;
				this.textBox_port.IsEnabled = false;
			} catch (SocketException) {
				// Do nothing
			}



		}

		private void backgroundWorker_DoWork(object sender, EventArgs e) {
			while (_isConnected)
			{
				try {
					var buffer = new byte[Marshal.SizeOf(packet)];
					this._socket2.ReceiveFrom(buffer, SocketFlags.None, ref this._ip2);
					Packet bufferToPacket = new Packet();
					bufferToPacket = (Packet) Packet.ByteToStructure(buffer, bufferToPacket.GetType());

					DateTime currTime = DateTime.Now;



					string ipData = ((IPEndPoint) this._ip2).Address.ToString();

					/*
					UpdateLog($"[{currTime.ToString("HH:mm:ss")}] from {ipData}\n" + $"type : {bufferToPacket.type} time : {bufferToPacket.time}\n" +
					          $"longitude : {bufferToPacket.longitude} latitude : {bufferToPacket.latitude}\n" +
					          $"message : {bufferToPacket.message}\n", textBox);
							  */
					Dispatcher.Invoke(new Action(delegate {
						this.packetList.Add(new PacketInfo(this.count++, ipData, bufferToPacket.type, bufferToPacket.time, bufferToPacket.longitude, bufferToPacket.latitude, bufferToPacket.message));
						this.listView.ScrollIntoView(this.listView.Items[this.listView.Items.Count - 1]);
					}));
					
					this._socket1.SendTo(buffer, SocketFlags.None, this._ip1);

				}
				catch (Exception ex) {
				}
			}
		}

		private void UpdateLog(string text, TextBox tb) {
			Dispatcher.Invoke(new Action(delegate {
				tb?.AppendText(text + '\n');
				tb?.ScrollToEnd();
			}));
		}

		private void button1_Click(object sender, RoutedEventArgs e) {
			this._isConnected = false;
			this._socket2.Close();
			this._socket1.Close();
			this.textBlock.Text = "연결되지 않음";

			this.button.IsEnabled = true;
			this.button1.IsEnabled = false;
			this.textBox_ip1.IsEnabled = true;
			this.textBox_ip2.IsEnabled = true;
			this.textBox_port.IsEnabled = true;

		}

		private void ClearButton_Click(object sender, RoutedEventArgs e) {
			this.packetList.Clear();
			this.count = 1;
		}
	}

}
