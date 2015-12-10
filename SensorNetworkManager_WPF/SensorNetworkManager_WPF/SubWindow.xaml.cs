using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SensorNetworkManager_WPF {

	/// <summary>
	/// Interaction logic for SubWindow.xaml
	/// </summary>
	public partial class SubWindow : Window {

		private struct Node {
			public byte ID;
			public byte level;
		}

		private MainWindow mainWindow;

		private int _count = 0;

		private const int ID_OFFSET = 0x01;

		private Ellipse[] ellipse;
		private RadioButton[] button;
		private BackgroundWorker[] backgroundWorker;
		private Node[] node;

		public SubWindow(MainWindow mainW) {
			mainWindow = mainW;
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
		}

		private void Window_Closed(object sender, EventArgs e) {
			mainWindow.subWindow = null;
		}

		/// <summary>
		/// 탐지된 노드 표시
		/// </summary>
		/// <param name="packet">The packet.</param>
		public void Blink(Packet packet) {
			byte srcID = packet.sourceID;
			int idx = packet.sourceID - ID_OFFSET;
			

			Dispatcher.Invoke(new Action(delegate {
				button[idx].FontWeight = FontWeights.Bold;
				button[idx].Content = packet.sourceLevel;
			}));

			node[idx].level = packet.sourceLevel;

			if (!backgroundWorker[idx].IsBusy)
				backgroundWorker[idx].RunWorkerAsync(idx);
		}

		public void Set_Node() {
			_count = 0;
			int num = mainWindow.numOfNodes;
			ellipse = new Ellipse[num];
			button = new RadioButton[num];
			backgroundWorker = new BackgroundWorker[num];
			node = new Node[num];
			this.grid.MouseLeftButtonDown += Grid_MouseLeftButtonDown;
			label.Content = "0x" + (_count + 1).ToString("X2") + " 위치를 지정하세요.";
		}

		private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if (_count < mainWindow.numOfNodes) {
				Point p = e.GetPosition(grid);

				ellipse[_count] = CreateEllipse(p);
				this.grid.Children.Add(ellipse[_count]);

				button[_count] = CreateButton(p);
				this.grid.Children.Add(button[_count]);

				backgroundWorker[_count] = new BackgroundWorker();
				backgroundWorker[_count].DoWork += new DoWorkEventHandler(worker_DoWork);
				_count++;

				label.Content = "0x" + (_count + 1).ToString("X2") + " 위치를 지정하세요.";

				if (_count == mainWindow.numOfNodes) {
					label.Content = "";
					mainWindow._isSetNode = true;
					this.grid.MouseLeftButtonDown -= Grid_MouseLeftButtonDown;
					MessageBox.Show("설정 완료");
				}
			}
		}

		private void SubWindow_Click(object sender, RoutedEventArgs e) {
			for (int i = 0; i < mainWindow.numOfNodes; i++) {
				if (button[i].GetHashCode() == sender.GetHashCode()) {
					this.label_ID.Content = "0x" + (i + ID_OFFSET).ToString("X2");

					if (node[i].level < 1)
						this.label_level.Content = "알 수 없음";
					else
						this.label_level.Content = node[i].level;
				}
			}
		}

		private void worker_DoWork(object sender, DoWorkEventArgs e) {
			var index = (int)e.Argument;

			for (int j = 0; j < 2; j++) {
				for (int i = 0; i < 100; i++) {
					Dispatcher.Invoke(new Action(delegate () {
						ellipse[index].Opacity = (double)i / 100;
					}));
					Thread.Sleep(1);
				}
				for (int i = 100; i > 0; i--) {
					Dispatcher.Invoke(new Action(delegate () {
						ellipse[index].Opacity = (double)i / 100;
					}));
					Thread.Sleep(1);
				}
			}
		}

		private void Window_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.System && e.SystemKey == Key.F4) {
				e.Handled = true;
			}
		}

		private void Window_Closing(object sender, CancelEventArgs e) {
			if (mainWindow._exit) {
				e.Cancel = false;
			} else {
				e.Cancel = true;
				mainWindow._isSubWindowOpened = false;
				Dispatcher.Invoke(new Action(delegate {
					mainWindow.button_map.Content = ">";
				}));
				this.Hide();
			}
		}

		private RadioButton CreateButton(Point p) {
			var radioButton = new RadioButton();
			radioButton.HorizontalAlignment = HorizontalAlignment.Left;
			radioButton.VerticalAlignment = VerticalAlignment.Top;
			radioButton.Margin = new Thickness(p.X - 7, p.Y - 7, 0, 0);
			radioButton.ToolTip = "0x" + (_count + 1).ToString("X2");
			radioButton.Click += SubWindow_Click;

			return radioButton;
		}

		private Ellipse CreateEllipse(Point p) {
			var ellipse = new Ellipse();

			var brush = new LinearGradientBrush();
			brush.StartPoint = new Point(0, 0);
			brush.EndPoint = new Point(1, 1);

			var gs1 = new GradientStop();
			gs1.Color = Colors.White;
			gs1.Offset = 0.0;
			var gs2 = new GradientStop();
			gs2.Color = Colors.Red;
			gs2.Offset = 1.0;
			brush.GradientStops.Add(gs1);
			brush.GradientStops.Add(gs2);

			ellipse.Fill = brush;
			ellipse.HorizontalAlignment = HorizontalAlignment.Left;
			ellipse.VerticalAlignment = VerticalAlignment.Top;
			ellipse.Width = 40;
			ellipse.Height = 40;
			ellipse.Margin = new Thickness(p.X - ellipse.Width / 2, p.Y - ellipse.Height / 2, 0, 0);
			ellipse.Opacity = 0;

			return ellipse;
		}
	}
}