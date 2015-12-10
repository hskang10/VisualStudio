using System;
using System.Windows;
using System.Windows.Controls;

namespace SensorNetworkManager_WPF {

	/// <summary>
	/// Interaction logic for NodeWindow.xaml
	/// </summary>
	public partial class NodeWindow : Window {

		public delegate void OnOKButtonClickHandler(int Number);
		public delegate void OnCancelButtonClickHandler();
		public delegate void WindowClosedHandler();
		public event OnOKButtonClickHandler OnOKButtonClickEvent;
		public event OnCancelButtonClickHandler OnCancelButtonClickEvent;
		public event WindowClosedHandler WindowClosedEvent;

		private int selectedNumber;

		public NodeWindow() {
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			for (int i = 0; i < 10; i++) {
				comboBox.Items.Add(i + 1);
			}
		}

		private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			selectedNumber = (int)comboBox.SelectedItem;
		}

		private void button_OK_Click(object sender, RoutedEventArgs e) {
			if (comboBox.SelectedIndex < 0)
				MessageBox.Show("개수를 선택하세요", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
			else {
				OnOKButtonClickEvent?.Invoke(selectedNumber);
			}
		}

		private void button_Cancel_Click(object sender, RoutedEventArgs e) {
			OnCancelButtonClickEvent?.Invoke();
		}

		private void Window_Closed(object sender, EventArgs e) {
			WindowClosedEvent?.Invoke();
		}
	}
}