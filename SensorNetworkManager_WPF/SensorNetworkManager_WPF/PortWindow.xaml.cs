using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;

namespace SensorNetworkManager_WPF {

	/// <summary>
	/// Interaction logic for PortWindow.xaml
	/// </summary>
	public partial class PortWindow : Window {

		public delegate void OnOkButtonClickHandler(string portNumber);

		public delegate void OnCancelButtonClickHandler();

		public delegate void WindowClosedHandler();

		public event OnOkButtonClickHandler OnOkButtonClickEvent;

		public event OnCancelButtonClickHandler OnCancelButtonClickEvent;

		public event WindowClosedHandler WindowClosedEvent;

		private string _selectedPortNumber;

		public PortWindow() {
			InitializeComponent();
			
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			/* 시리얼 포트 받아와서 comboBox에 추가 */
			var portList = SerialPort.GetPortNames();
			comboBox_port.Items.Clear();
			for (var i = 0; i < portList.Length; i++)
				comboBox_port.Items.Add(portList[i]);
		}

		private void comboBox_port_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			this._selectedPortNumber = comboBox_port.SelectedItem.ToString();
		}

		private void button_OK_Click(object sender, RoutedEventArgs e) {
			if (comboBox_port.SelectedIndex < 0)
				MessageBox.Show("포트를 선택하세요", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
			else
			{
				this.OnOkButtonClickEvent?.Invoke(this._selectedPortNumber);
			}
		}

		private void button_cancel_Click(object sender, RoutedEventArgs e)
		{
			OnCancelButtonClickEvent?.Invoke();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			WindowClosedEvent?.Invoke();
		}
	}
}