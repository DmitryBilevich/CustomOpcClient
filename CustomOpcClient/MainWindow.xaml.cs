using System.Windows;
using System;
using System.Windows.Threading;
using System.Windows.Media;
using CustomOpcClient.Domain.Model;
using CustomOpcClient.Domain;

namespace CustomOpcClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _motorActive;
        private int _motorSpeed;
        private bool _autManSwitch;

        private readonly IMotorService _motorService;

        public MainWindow(IMotorService motorService)
        {
            _motorService = motorService;
            Initialize();
        }

        public void Initialize()
        {
            InitializeComponent();
            InitWindowRefresh();
            _motorService.Connect();
            _motorService.ReadMotorData(MotorChangedCallBack);
        }

        private void InitWindowRefresh()
        {
            DispatcherTimer tmr = new DispatcherTimer();
            tmr.Interval = TimeSpan.FromMilliseconds(100);
            tmr.Tick += new EventHandler(WindowRefresh);
            tmr.Start();
        }

        private void WindowRefresh(object sender, EventArgs e)
        {
            if (_motorActive == true)
                ledMotor.Fill = new SolidColorBrush(Colors.Green);
            else
                ledMotor.Fill = new SolidColorBrush(Colors.Gray);

            lblSpeed.Content = _motorSpeed;
            btnAutMan.IsChecked = _autManSwitch;
        }

        private void MotorChangedCallBack(Motor motor)
        {
            if (motor == null)
                return;

            if (motor.Speed.HasValue)
                _motorSpeed = motor.Speed.Value;
            if (motor.IsActive.HasValue)
                _motorActive = motor.IsActive.Value;
            if (motor.IsAutoMode.HasValue)
                _autManSwitch = motor.IsAutoMode.Value;
        }

        private void btnAutMan_Click(object sender, RoutedEventArgs e)
        {
            _motorService.AutomationManualSwitch(Convert.ToInt32(!btnAutMan.IsChecked));
            e.Handled = true;
        }

        private void btnStartMotor_Click(object sender, RoutedEventArgs e)
        {
            _motorService.StartMotor();
        }

        private void btnStoptMotor_Click(object sender, RoutedEventArgs e)
        {
            _motorService.StopMotor();
        }

        private void btnJogMotor_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _motorService.JogMotor(1);
        }

        private void btnJogMotor_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _motorService.JogMotor(0);
        }

    }
}
