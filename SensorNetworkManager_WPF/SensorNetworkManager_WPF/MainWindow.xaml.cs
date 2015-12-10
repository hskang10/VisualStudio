using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace SensorNetworkManager_WPF {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	///

	public partial class MainWindow : Window {
		public SubWindow subWindow = null;
		public PortWindow portWindow = null;
		public NodeWindow nodeWindow = null;
		public bool _exit = false;

		public bool _isSubWindowOpened = true;

		private BackgroundWorker backgroundWorker_Listen;
		private SerialPort serialPort;
		private bool _isConnected;
		private string _selectedCOMPort = null;
		public bool _isSetNode = false;
		public int numOfNodes;

		public MainWindow() {
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			backgroundWorker_Listen = new BackgroundWorker();
			backgroundWorker_Listen.DoWork += this.backgroundWorker_Listen_DoWork;
			backgroundWorker_Listen.RunWorkerCompleted += this.backgroundWorker_Listen_RunWorkerCompleted;

			_isConnected = false;

			button_disconnect.IsEnabled = false;

			if (subWindow == null) {
				subWindow = new SubWindow(this);
				subWindow.WindowStartupLocation = WindowStartupLocation.Manual;
				subWindow.Left = this.Left + this.Width;
				subWindow.Top = this.Top;
				subWindow.Show();
			}
		}

		private void Window_Closed(object sender, EventArgs e) {
			if (subWindow == null) return;
			_exit = true;
			subWindow.Close();
		}

		private void Window_LocationChanged(object sender, EventArgs e) {
			if (subWindow == null) return;
			subWindow.Top = this.Top;
			subWindow.Left = this.Left + this.Width;
		}

		private void button_connect_Click(object sender, RoutedEventArgs e) {
			if (_isConnected == false) {
				if (_selectedCOMPort == null) {
					MessageBox.Show("포트를 설정하세요");
				} else {
					try {
						serialPort = new SerialPort(_selectedCOMPort, 9600);
						serialPort.Open();
						_isConnected = true;

						this.backgroundWorker_Listen.RunWorkerAsync();
						this.label_connection.Content = "Connected";
						UpdateLog(this._selectedCOMPort + "포트에 연결됨", textBox_systemLog);
						this.image_connetion.Source = (ImageSource)FindResource("green");

						this.button_connect.IsEnabled = false;
						this.button_disconnect.IsEnabled = true;
						button_portSelect.IsEnabled = false;
					} catch (Exception ex) {
						MessageBox.Show("Connection Fail(" + ex.Message + ")", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
		}

		private void button_disconnect_Click(object sender, RoutedEventArgs e) {
			if (_isConnected) {
				_isConnected = false;
				this.button_connect.IsEnabled = true;
				this.button_disconnect.IsEnabled = false;
				button_portSelect.IsEnabled = true;
				UpdateLog(_selectedCOMPort + "포트에 연결 해제됨", textBox_systemLog);
				this.image_connetion.Source = (ImageSource)FindResource("red");
			}
			this.label_connection.Content = "Disconnected";
		}

		private void button_map_Click(object sender, RoutedEventArgs e) {
			if (_isSubWindowOpened) {
				_isSubWindowOpened = false;
				subWindow.Hide();
				button_map.Content = ">";
			} else {
				_isSubWindowOpened = true;
				subWindow.Show();
				button_map.Content = "<";
			}
		}

		private void button_initialize_Click(object sender, RoutedEventArgs e) {
			if (!_isConnected)
				MessageBox.Show("not connected!", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
			else {
				var packet = new Packet();
				packet.type = (byte)MessageType.Initialization;
				packet.sourceID = (byte)Id.Center;
				packet.sourceLevel = 0;
				packet.senderID = (byte)Id.Center;
				packet.senderLevel = 0;
				packet.receiverID = (byte)Id.Sink;
				byte[] buffer = StructureToByte(packet);
				byte[] stx = {0x02};
				byte[] etx = {0x03};
				serialPort.Write(stx, 0, 1);
				serialPort.Write(buffer, 0, buffer.Length);
				serialPort.Write(etx, 0, 1);
				UpdateLog("SINK 노드로 초기화 요청 보냄", textBox_systemLog);
			}
		}

		private void button_portSelect_Click(object sender, RoutedEventArgs e) {
			if (portWindow == null) {
				portWindow = new PortWindow();
				portWindow.Owner = this;
				portWindow.OnOkButtonClickEvent += this.portWindow_OnOKButtonClickEvent;
				portWindow.OnCancelButtonClickEvent += this.portWindow_OnCancelButtonClickEvent;
				portWindow.WindowClosedEvent += this.portWindow_WindowClosedEvent;
				portWindow.ShowDialog();
			}
		}

		private void logo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			Process.Start("http://mimocom.knu.ac.kr");
		}

		private void button_exit_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void button_nodeSetting_Click(object sender, RoutedEventArgs e) {
			if (_isSetNode) {
				var dr = MessageBox.Show("이미 설정되어 있습니다. 다시 설정하시겠습니까?", "알림", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (dr == MessageBoxResult.No)
					return;
			}

			_isSetNode = false;

			if (nodeWindow != null) return;

			nodeWindow = new NodeWindow();
			nodeWindow.Owner = this;

			nodeWindow.OnOKButtonClickEvent += this.nodeWindow_OnOKButtonClickEvent;
			nodeWindow.OnCancelButtonClickEvent += this.nodeWindow_OnCancelButtonClickEvent;
			nodeWindow.WindowClosedEvent += this.nodeWindow_WindowClosedEvent;
			nodeWindow.ShowDialog();
		}
	}
}