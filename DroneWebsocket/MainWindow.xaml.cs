using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WebSocketSharp;

namespace DroneWebsocket
{

    public partial class MainWindow : Window
    {
        DroneData drone = new DroneData();//All drone data will be put inside this object and displayed later.


        public MainWindow()
        {
            InitializeComponent();

            //Setup and start a 1 second timer to update UI values
            var updateUiTimer = new DispatcherTimer();
            updateUiTimer.Interval = new TimeSpan(0, 0, 1);
            updateUiTimer.Tick += updateUiTimer_tick;
            updateUiTimer.Start();

            //Below sets up event handlers for the websocket using the websocketsharp NuGet package
            //It has been designed to be automatically connected, no input needed. If it disconnects then reconnect with no obvious notification, print to console.
            using (var ws = new WebSocket("ws://thawing-headland-83841.herokuapp.com/"))
            {
                //Connect the websocket here
                ws.Connect();

                ws.OnMessage += (sender, e) =>
                {
                    //messages are handled and parsed here
                    drone.ParseData(e.Data);
                };

                ws.OnError += (sender, e) =>
                {
                    //Errors are printed to the console
                    Console.WriteLine("Websocket error" + Environment.NewLine + e.Exception.ToString());
                };

                ws.OnOpen += (sender, e) =>
                {
                    //Open notification is printed to the console
                    Console.WriteLine("Websocket Opened!");
                };

                ws.OnClose += (sender, e) =>
                {
                    //If the connected is broken automatical reconnect
                    ws.Connect();
                    Console.WriteLine("Websocket closed");
                };
            }
        }

        public void closeApp(object sender, RoutedEventArgs e)
        {
            this.Close(); //This allows custom close button to work
        }

        public void updateUiTimer_tick(object sender, EventArgs e)
        {
            //Update the display every second

            temperatureCurrentText.Text = drone.Temperature;
            temperatureAverageText.Text = drone.GetTemperatureAverage();

            pressureCurrentText.Text = drone.Pressure;
            pressureAverageText.Text = drone.GetPressureAverage();

            humidityCurrentText.Text = drone.Humidity;
            humidityAverageText.Text = drone.GetHumidityAverage();

            percentCloudCurrentText.Text = drone.PercentCloud;
            percentCloudAverageText.Text = drone.GetPercentCloudAverage();

            timeStampText.Text = drone.UTC_time;

            windSpeedText.Text = drone.WindSpeed;
            windDirectionText.Text = drone.WindDirection;

            lattitudeText.Text = drone.Latitude;
            longitudeText.Text = drone.Longitude;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Since there is no frame to the window, use this to be able to drag the window from anywhere.
            if (e.ChangedButton == MouseButton.Left) this.DragMove();
        }
    }
}