using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Starter {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		public MainWindow() {
			InitializeComponent();

			this.comboBoxType.Items.Add("사고발생");
			this.comboBoxType.Items.Add("도로결빙");
			this.comboBoxType.Items.Add("공사중");

			this.textBoxMessage.MaxLength = 19;
		}

		private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			
		}

		private void buttonSend_Click(object sender, RoutedEventArgs e) {

			Socket socket;
			EndPoint endPoint;
			Packet packet;

			string ipAddress = this.textBoxIP.Text;
			string portNumber = this.textBoxPort.Text;

			try {
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), Int32.Parse(portNumber));
			}
			catch (Exception) {
				MessageBox.Show("잘못된 IP주소나 Port번호를 입력하였습니다.\n" +
				                "IP주소 : xxx.xxx.xxx.xxx\n" +
				                "Port번호 : 0~65535");
				return;
			}

						
			if (this.comboBoxType.SelectedIndex < 0) {
					MessageBox.Show("타입을 선택하세요");
					return;
			}
			packet = new Packet();
			packet.type = (byte) this.comboBoxType.SelectedIndex;
			packet.time = 5;
			packet.longitude = 0;
			packet.latitude = 0;
			packet.message = this.textBoxMessage.Text;

			var buffer = Packet.StructureToByte(packet);
			socket.SendTo(buffer, endPoint);

			DateTime currTime = DateTime.Now;
			UpdateLog($"[{currTime.ToString("HH:mm:ss")}] 메시지 전송 to {ipAddress}:{portNumber}", textBoxLog);

		}

		private void UpdateLog(string text, TextBox tb) {
			Dispatcher.Invoke(new Action(delegate {
				tb.AppendText(text + "\n");
				tb.ScrollToEnd();
			}));
		}

		private void textBoxPort_PreviewKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.ImeProcessed || e.Key == Key.Space)
				e.Handled = true;
		}

		private void textBoxPort_PreviewTextInput(object sender, TextCompositionEventArgs e) {
			int checkVal;

			if (!int.TryParse(e.Text, out checkVal)) {
				e.Handled = true;
			}
		}

		private void textBoxPort_TextChanged(object sender, TextChangedEventArgs e) {
			if (this.textBoxPort.Text.Length > 0 && Int32.Parse(this.textBoxPort.Text) > 65535) {
				MessageBox.Show("포트 번호는 65535를 초과할 수 없습니다");
				e.Handled = true;
			}
		}

		private void textBoxLog_PreviewKeyDown(object sender, KeyEventArgs e) {
			e.Handled = true;
		}
	}
}

