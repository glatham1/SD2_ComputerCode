using Arction.Wpf.Charting;
using Arction.Wpf.Charting.Annotations;
using Arction.Wpf.Charting.Axes;
using Arction.Wpf.Charting.SeriesXY;
using Arction.Wpf.Charting.Views.ViewXY;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Data;
using System.ComponentModel;
using MySql.Data.MySqlClient;

namespace NanoPSI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    /*
     * TODO
     * Fix DataGrid to display what is shown on lightning chart
     * Fix messagebox popups
     * Make it so when dataGrid is changed, then changes are saved
     * Take away display on left side of lightning charts
     * Troubleshoot buttons
     * Get read and write sql functions working properly
     */

    // if connection is connected then lettering should be green and everything should be usable, if not connected then lettering should be red and everything in that row should be default and gray out
    // if connected, if is active is checked then everything runs as normal and if not checked then keep updating but gray out to indicate it will not be saved

    public partial class MainWindow : Window
    {
        private LightningChart _chart = null;
        private DispatcherTimer timer;
        private DateTime startTime;
        BluetoothClient client = new BluetoothClient();
        private int minThreshold = 0;
        private int maxThreshold = 400;
        private ObservableCollection<TransducerData> transducerDataCollection = new ObservableCollection<TransducerData>();

        // Change this for local server
        private const string connStr = "server=127.0.0.1;port=3306;database=sys;user=root;password=NinerGraduate-2024;";

        public class TransducerData : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private bool _isActive;
            public bool IsActive
            {
                get => _isActive;
                set
                {
                    if (_isActive != value)
                    {
                        _isActive = value;
                        OnPropertyChanged(nameof(IsActive));
                    }
                }
            }

            private string _transducerName;
            public string TransducerName
            {
                get => _transducerName;
                set
                {
                    if (_transducerName != value)
                    {
                        _transducerName = value;
                        OnPropertyChanged(nameof(TransducerName));
                    }
                }
            }
            public List<double> PressureValues { get; set; }
            public List<double> TimeValues { get; set; }
            public double Offset { get; set; }
            public string ConnectionStatus { get; set; }

            // New properties for the last pressure and time values
            private double _lastPressureValue;
            public double LastPressureValue
            {
                get => _lastPressureValue;
                set
                {
                    if (_lastPressureValue != value)
                    {
                        _lastPressureValue = value;
                        OnPropertyChanged(nameof(LastPressureValue));
                        // No need to notify for PressureValues as it's not directly bound to the UI
                    }
                }
            }

            private double _lastTimeValue;
            public double LastTimeValue
            {
                get => _lastTimeValue;
                set
                {
                    if (_lastTimeValue != value)
                    {
                        _lastTimeValue = value;
                        OnPropertyChanged(nameof(LastTimeValue));
                        // No need to notify for TimeValues as it's not directly bound to the UI
                    }
                }
            }

            // Implement OnPropertyChanged method
            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public TransducerData(int transducerNum)
            {
                TransducerName = "Transducer " + transducerNum;
                PressureValues = new List<double>();
                TimeValues = new List<double>();
                Offset = 0.0;
                ConnectionStatus = "Not Connected";
                IsActive = false;
            }

            public void AddPressureValue(double pressure, double time)
            {
                PressureValues.Add(pressure);
                TimeValues.Add(time);
                // Update the last pressure and time values
                LastPressureValue = pressure;
                LastTimeValue = time;
            }
        }

        public MainWindow()
        {
            // Set Deployment Key for LightningChart components
            string deploymentKey = "lgCAAE3C1L0Ig9kBJABVcGRhdGVhYmxlVGlsbD0yMDI1LTA1LTEyI1JldmlzaW9uPTACgD9VCrEQWTcyiWKli80/h/yvJ3PUo9Iacz9kMGmQRMVUlMQFidiOQwCSaOXmsMNu7XFpqENfqtDrAAoepDhhY1eAnA1aWsJYAdPY01icT/20IDzzsnhuXuyWwRO9w4lCbieMD4do7M6U1Wdj0+xAPRuZGrSY3kaJYnp84YvfZ1wPrvvhXbt/EQPkcKstrpo7kjxhEjSEL3p+rIQfz1zde1WFLR0VmMbTZe4M3c7Z5qS9aPQtPT3GDNeQFgtEBdUhFNjh1xVJguD6y8rjVmqyzkelfnNyj/6zMh/C5rZSaBWpmmSNIgD+nJ3CRyA3DoYXDVTlxmbLP16UV0YvttIC/5SJD15lQkPbd+B/KJYc8Ost0wBYmHGR9/lveXThh/NWz8amQyE3It/nn2S7bvWmJoGOtsbqRCsyeGbZBxW2EnNQnlruvy0tlzQ8FvaMlXCeB4LYQntdi12EKGh8QYIxuEdu5BBquigN3fChsf+r+qjE9+hIXXMIgi5YfhTJU9meik8=";
            Arction.Wpf.Charting.LightningChart.SetDeploymentKey(deploymentKey);

            InitializeComponent();
            CreateChart();
            InitializeTimer();
            InitializeTable();
            DiscoverDevices();
            TestSqlConnection();

            minThresh.Text = minThreshold.ToString();
            maxThresh.Text = maxThreshold.ToString();
        }

        /**************** SQL Connection Functions ************************************************/
        
        // Test Connection to mySQL Server
        private void TestSqlConnection()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open(); // Open the connection just to test
                    sqlConnLabel.Content = "Connected";
                    sqlConnLabel.Foreground = new SolidColorBrush(Colors.Green);
                }
            }
            catch (Exception ex)
            {
                sqlConnLabel.Content = "Not Connected";
                sqlConnLabel.Foreground = new SolidColorBrush(Colors.Red);
                MessageBox.Show($"Failed to connect to database: {ex.Message}");
            }
        }

        // Writes to mySQL Server
        private void WriteToDatabase()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open(); // Open the connection
                    DateTime now = DateTime.Now; // Capture the current time once to use for all rows

                    // Assume `transducerDataCollection` is a collection of your transducer data objects
                    foreach (var transducer in transducerDataCollection)
                    {
                        // Format the pressure and time lists
                        string formattedPressure = string.Join(";", transducer.PressureValues);
                        string formattedTime = string.Join(";", transducer.TimeValues);

                        // Prepare the INSERT statement with the correct table and column names
                        string insertSql = @"INSERT INTO `sys`.`nanopsi`
                                    (`Saved Date/Time`, `Transducer Name`, `Pressure`, `Time`, `Offset`, `Connection Status`, `Is Active`) 
                                    VALUES
                                    (@SavedDateTime, @TransducerName, @Pressure, @Time, @Offset, @ConnectionStatus, @IsActive);";

                        using (MySqlCommand cmd = new MySqlCommand(insertSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@SavedDateTime", now);
                            cmd.Parameters.AddWithValue("@TransducerName", transducer.TransducerName);
                            cmd.Parameters.AddWithValue("@Pressure", formattedPressure);
                            cmd.Parameters.AddWithValue("@Time", formattedTime);
                            cmd.Parameters.AddWithValue("@Offset", transducer.Offset); // Assuming `Offset` is already part of your transducer data object
                            cmd.Parameters.AddWithValue("@ConnectionStatus", transducer.ConnectionStatus);
                            cmd.Parameters.AddWithValue("@IsActive", transducer.IsActive ? "Yes" : "No"); // Assuming `IsActive` is a boolean you're converting to "Yes" or "No"

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred writing to the database: {ex.Message}");
            }
        }

        // Reads from mySQL Server
        public void ReadFromDatabase(string selectedDate)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open(); // Open the connection

                    // Format the input date to match SQL format, assuming selectedDate is already in correct format
                    // If selectedDate is not in 'yyyy-MM-dd' format, you need to convert it first
                    string formattedDate = selectedDate;

                    // Define the SQL query to select all records from nanopsi table for the given date
                    // Assuming the 'Saved Date/Time' column is stored in a DATE or DATETIME format that can be compared with a string
                    string sql = $"SELECT * FROM nanopsi WHERE DATE(`Saved Date/Time`) = '{formattedDate}';";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        transducerDataCollection.Clear(); // Clear existing data before fetching new

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Assuming the reader can directly fetch the offset as double, adjust if necessary
                                double offset = reader["Offset"] != DBNull.Value ? Convert.ToDouble(reader["Offset"]) : 0.0;
                                var transducer = new TransducerData(0) // The 0 placeholder might need to be replaced or managed differently
                                {
                                    TransducerName = reader["Transducer Name"].ToString(),
                                    IsActive = reader["Is Active"].ToString().Equals("Yes"),
                                    ConnectionStatus = reader["Connection Status"].ToString(),
                                    Offset = offset
                                };

                                // Assuming Pressure and Time are stored as semicolon-separated strings
                                transducer.PressureValues = reader["Pressure"].ToString().Split(';').Select(double.Parse).ToList();
                                transducer.TimeValues = reader["Time"].ToString().Split(';').Select(double.Parse).ToList();

                                transducerDataCollection.Add(transducer);
                            }
                        }
                    }
                }

                // Assuming you have a method or logic to refresh the DataGrid with the new transducerDataCollection
                dataGrid.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred reading from the database: {ex.Message}");
            }
        }

        /**************** Bluetooth Connection Functions ******************************************/

        // Find Bluetooth Devices
        private void DiscoverDevices()
        {
            try
            {
                BluetoothDeviceInfo[] devices = client.DiscoverDevicesInRange();
                mcuConnect.Items.Clear(); // Clear existing items
                foreach (BluetoothDeviceInfo device in devices)
                {
                    //mcuConnect.Items.Add(device.DeviceName);
                    mcuConnect.DisplayMemberPath = "DeviceName";
                    mcuConnect.SelectedValuePath = "DeviceAddress";
                    mcuConnect.Items.Add(device);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error discovering devices: {ex.Message}");
            }
        }

        //  User selection for connection
        private void mcuConnect_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (mcuConnect.SelectedItem is BluetoothDeviceInfo selectedDevice)
            {
                ConnectToDevice(selectedDevice);
            }
        }

        // Connects to Bluetooth Device
        private void ConnectToDevice(BluetoothDeviceInfo device)
        {
            try
            {
                client.BeginConnect(device.DeviceAddress, BluetoothService.SerialPort, new AsyncCallback(ConnectCallback), device);
                mcuStatus.Content = "Connected";
                mcuStatus.Foreground = new SolidColorBrush(Colors.Green);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error connecting to device: {ex.Message}");
                mcuStatus.Content = "Not Connected";
                mcuStatus.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        // Ends Connection
        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                client.EndConnect(result);
                

                // After successful connection, you can start reading/writing data
                // Implement your data read/write logic here
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in connection callback: {ex.Message}");
            }
        }

        // Read Microcontroller String
        private void ReadMCU(double time)
        {
            try
            {
                //if (client.Connected)
                //{
                //    NetworkStream stream = client.GetStream();
                //    if (stream.CanRead)
                //    {
                //        byte[] buffer = new byte[1024];
                //        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                //        string inputStr = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                string inputStr = RandomTestString();

                        string[] transducerValues = inputStr.Split(',');
                        for (int i = 0; i < transducerValues.Length; i++)
                        {
                            string[] parts = transducerValues[i].Split(':');
                            int transducerNumber = int.Parse(parts[0]);
                            double analogVoltage = parts[1] == "-1" ? -1 : double.Parse(parts[1]);

                            double pressure = ConvertVoltageToPressure(analogVoltage);

                            // Find or create the corresponding TransducerData object
                            var transducerData = transducerDataCollection.FirstOrDefault(t => t.TransducerName == "Transducer " + transducerNumber);
                            if (transducerData == null)
                            {
                                transducerData = new TransducerData(transducerNumber);
                                transducerDataCollection.Add(transducerData);
                            }

                            // Update the TransducerData object
                            transducerData.IsActive = pressure != -1;
                            transducerData.ConnectionStatus = transducerData.IsActive ? "Connected" : "Not Connected";
                            if (transducerData.IsActive)
                            {
                                pressure += transducerData.Offset; // Apply the offset
                                transducerData.AddPressureValue(pressure, time);
                            }
                        }
                //    }
                //    else
                //    {
                //        System.Windows.MessageBox.Show("Cannot read from device");
                //    }
                //}
                //else
                //{
                //    System.Windows.MessageBox.Show("Not connected to device");
                //}
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error reading from device: {ex.Message}");
            }
        }

        // Writes to Microcontroller
        private void WriteToMCU(string data)
        {
            try
            {
                if (client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    if (stream.CanWrite)
                    {
                        byte[] buffer = Encoding.ASCII.GetBytes(data);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Cannot write to device");
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Not connected to device");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error writing to device: {ex.Message}");
            }
        }

        private string RandomTestString()
        {
            Random random = new Random();
            StringBuilder sb = new StringBuilder();

            for (int i = 1; i <= 15; i++)
            {
                double voltage = (random.NextDouble() * 4.0) + 0.5;
                sb.Append($"{i}:{voltage:F1}");

                if (i < 15)
                {
                    sb.Append(",");
                }
            }

            return sb.ToString();

        }

        /***************** Timer Functions *********************************************************/

        private void InitializeTimer()
        {
            // Create a new timer
            timer = new DispatcherTimer();
            // Set the interval to 1 second
            timer.Interval = TimeSpan.FromSeconds(0.5);
            // Assign the event that's called each tick
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var elapsed = now - startTime;
            lblElapsedTime.Content = $"{elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}";

            try
            {
                ReadMCU(elapsed.TotalSeconds);
                UpdateChartData(elapsed.TotalSeconds);
            } 
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        /***************** Lightning Charts Functions **********************************************/
        private void CreateChart()
        {
            // Create a new chart.
            _chart = new LightningChart
            {
                ChartName = "Cursor tracking chart"
            };

            //Disable rendering, strongly recommended before updating chart properties
            _chart.BeginUpdate();

            //Y-axis are stacked and has common x-axis drawn
            _chart.ViewXY.AxisLayout.YAxesLayout = YAxesLayout.Layered;

            // Clear existing Y-axes and create a single Y-axis with the title "psi"
            _chart.ViewXY.YAxes.Clear();
            var yAxis = new AxisY(_chart.ViewXY);
            yAxis.Title.Text = "Pressure (psi)";
            yAxis.Units.Text = "psi";
            _chart.ViewXY.YAxes.Add(yAxis);

            //Chart background properties
            _chart.ViewXY.GraphBackground.GradientFill = GradientFill.Radial;

            // Enable and configure the legend box
            _chart.ViewXY.LegendBoxes[0].Visible = true;
            _chart.ViewXY.LegendBoxes[0].Layout = LegendBoxLayout.VerticalColumnSpan;
            _chart.ViewXY.LegendBoxes[0].Position = LegendBoxPositionXY.TopRight;
            _chart.ViewXY.LegendBoxes[0].Offset.SetValues(0, 0);
            _chart.ViewXY.LegendBoxes[0].SeriesTitleFont = new WpfFont("Segoe UI", 12, false, false);

            //Chart title text
            _chart.Title.Text = "Pocket Pressures";

            //X-axis title
            _chart.ViewXY.XAxes.Clear();
            var xAxis = new AxisX(_chart.ViewXY);
            xAxis.Title.Text = "Time (s)";
            xAxis.ValueType = AxisValueType.Number;
            xAxis.ScrollMode = XAxisScrollMode.Scrolling;
            _chart.ViewXY.XAxes.Add(xAxis);

            // Define an array of colors for the series
            System.Windows.Media.Color[] colors = new System.Windows.Media.Color[]
            {
                Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Cyan,
                Colors.Magenta, Colors.Orange, Colors.Purple, Colors.Brown, Colors.Gray,
                Colors.DarkRed, Colors.DarkGreen, Colors.DarkBlue, Colors.LightPink, Colors.LightGreen
            };

            // Create a PointLineSeries for each transducer and assign a color from the array
            for (int i = 0; i < 15; i++)
            {
                var series = new PointLineSeries(_chart.ViewXY, _chart.ViewXY.XAxes[0], _chart.ViewXY.YAxes[0])
                {
                    Title = { Text = "Transducer " + (i + 1) },
                    LineStyle = { Color = colors[i] }
                };
                _chart.ViewXY.PointLineSeries.Add(series);
            }

            System.Windows.Media.Color color = Colors.Black;

            /*
            int seriesCount = 15;
            int pointsCount = 500;

            Random rand = new Random();
            const double dXStep = 0.1;
            System.Windows.Media.Color color = Colors.Black;
            
            //Create series
            for (int seriesIndex = 0; seriesIndex < seriesCount; seriesIndex++)
            {
                // Create a point-line series
                PointLineSeries pointLineSeries = new PointLineSeries(_chart.ViewXY, _chart.ViewXY.XAxes[0], yAxis);
                pointLineSeries.LineStyle.Color = DefaultColors.SeriesForBlackBackgroundWpf[seriesIndex];
                pointLineSeries.Title.Text = "Transducer " + (seriesIndex + 1);

                // Set some data into the series
                SeriesPoint[] points = new SeriesPoint[pointsCount];
                double x = 0;
                for (int pointIndex = 0; pointIndex < pointsCount; pointIndex++)
                {
                    points[pointIndex].X = x;
                    points[pointIndex].Y = 300.0 + 800.0 * (0.2 * (rand.NextDouble() - 0.5) + 0.2 * Math.Sin(x * (seriesIndex + 1)));
                    x += dXStep;
                }
                pointLineSeries.Points = points;

                // Add the series to the chart
                _chart.ViewXY.PointLineSeries.Add(pointLineSeries);
            }*/

            //Add an annotation to show the cursor values
            AnnotationXY cursorValueDisplay = new AnnotationXY(_chart.ViewXY, _chart.ViewXY.XAxes[0], _chart.ViewXY.YAxes[0])
            {
                Style = AnnotationStyle.RoundedCallout,
                LocationCoordinateSystem = CoordinateSystem.RelativeCoordinatesToTarget
            };
            cursorValueDisplay.LocationRelativeOffset.X = 130;
            cursorValueDisplay.LocationRelativeOffset.Y = -200;
            cursorValueDisplay.Sizing = AnnotationXYSizing.Automatic;
            cursorValueDisplay.TextStyle.Font = new WpfFont("Lucida Console", 13f, false, false);
            cursorValueDisplay.TextStyle.Color = Colors.Black;
            cursorValueDisplay.Text = "";
            cursorValueDisplay.AllowTargetMove = false;
            cursorValueDisplay.Fill.Color = Colors.White;
            color = Colors.Cyan;
            cursorValueDisplay.Fill.GradientColor = System.Windows.Media.Color.FromArgb(120, color.R, color.G, color.B);
            cursorValueDisplay.BorderVisible = false;
            cursorValueDisplay.Visible = false;
            _chart.ViewXY.Annotations.Add(cursorValueDisplay);

            //Add cursor
            LineSeriesCursor cursor = new LineSeriesCursor(_chart.ViewXY, _chart.ViewXY.XAxes[0]);
            _chart.ViewXY.LineSeriesCursors.Add(cursor);
            cursor.PositionChanged += cursor_PositionChanged;
            cursor.ValueAtXAxis = 10;
            cursor.LineStyle.Color = System.Windows.Media.Color.FromArgb(150, 255, 0, 0);
            cursor.SnapToPoints = checkBoxSnapToNearestDataPoint.IsChecked == true;
            cursor.TrackPoint.Color1 = Colors.White;

            _chart.ViewXY.ZoomToFit();

            _chart.AfterRendering += _chart_AfterRendering;

            //Allow chart rendering
            _chart.EndUpdate();

            _chart.SizeChanged += new SizeChangedEventHandler(_chart_SizeChanged);
            gridChart.Children.Add(_chart);

        }

        private void UpdateChartData(double time)
        {
            _chart.BeginUpdate();

            // Assuming transducerDataCollection and the series are matched by index,
            // and each has a corresponding series in the chart.
            for (int i = 0; i < transducerDataCollection.Count; i++)
            {
                var series = _chart.ViewXY.PointLineSeries[i];
                series.Visible = true;

                var transducerData = transducerDataCollection[i];

                // Prepare a new list of points for the series
                var newPoints = new List<SeriesPoint>();

                // Assuming PressureValues and TimeValues are synchronized by index
                for (int j = 0; j < transducerData.PressureValues.Count; j++)
                {
                    double pressure = transducerData.PressureValues[j];
                    double pointTime = transducerData.TimeValues[j];
                    newPoints.Add(new SeriesPoint(pointTime, pressure));
                }

                // Update the series with the new points
                series.Points = newPoints.ToArray();
            }

            _chart.ViewXY.XAxes[0].SetRange(0, time);
            _chart.ViewXY.YAxes[0].SetRange(minThreshold, maxThreshold);

            _chart.EndUpdate();
        }

        private void cursor_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            // Cancel the on-going rendering because the code below updates 
            // the chart.
            e.CancelRendering = true;

            UpdateCursorResult();
        }

        private void _chart_AfterRendering(object sender, AfterRenderingEventArgs e)
        {
            //Update cursor after the chart has been rendered first time. 
            _chart.AfterRendering -= _chart_AfterRendering;
            UpdateCursorResult();
        }

        private void _chart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCursorResult();
        }

        private void checkBox1_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (_chart != null)
            {
                UpdateCursorResult();
            }
        }

        /// <summary>
        /// Update cursor texts.
        /// </summary>
        private void UpdateCursorResult()
        {
            bool showNextToCursor = checkBoxShowValuesNextToCursor.IsChecked == true;

            //Disable rendering, strongly recommended before updating chart properties
            _chart.BeginUpdate();

            //Get cursor
            LineSeriesCursor cursor = _chart.ViewXY.LineSeriesCursors[0];

            //Get annotation
            AnnotationXY cursorValueDisplay = _chart.ViewXY.Annotations[0];

            //Set annotation target. The location is relative to target. 
            //Use graph bottom as target Y value. 
            float targetYCoord = (float)_chart.ViewXY.GetMarginsRect().Bottom;
            double y;
            _chart.ViewXY.YAxes[0].CoordToValue(targetYCoord, out y);
            cursorValueDisplay.TargetAxisValues.X = cursor.ValueAtXAxis;
            cursorValueDisplay.TargetAxisValues.Y = y;

            double seriesYValue = 0;

            StringBuilder sb = new StringBuilder();
            int seriesNumber = 1;

            string channelStringFormat = "Transducer {0}: {1,12:#####.###} {2}";
            bool labelVisible = false;
            bool accurate = radioButtonAccurate.IsChecked == true;
            string value = "";

            foreach (PointLineSeries series in _chart.ViewXY.PointLineSeries)
            {
                //show series titles and cursor values in them, on the right side of the chart, 
                //if cursor values are not shown next to the cursor in an annotation
                if (series.Visible)
                {
                    series.Title.Visible = !showNextToCursor;
                    bool resolvedOK = false;
                    value = "";

                    if (accurate)
                    {
                        resolvedOK = SolveValueAccurate(series, cursor.ValueAtXAxis, out seriesYValue);
                    }
                    else
                    {
                        resolvedOK = SolveValueCoarse(series, cursor.ValueAtXAxis, out seriesYValue);
                    }

                    AxisY axisY = _chart.ViewXY.YAxes[series.AssignYAxisIndex];
                    axisY.Units.Text = "psi";

                    if (resolvedOK)
                    {
                        labelVisible = true;
                        value = string.Format(channelStringFormat, seriesNumber, seriesYValue.ToString("0.0"), axisY.Units.Text);
                    }
                    else
                    {
                        value = string.Format(channelStringFormat, seriesNumber, "---", axisY.Units.Text);
                    }
                    sb.AppendLine(value);
                    series.Title.Text = value;
                }
                seriesNumber++;
            }

            sb.AppendLine("");
            sb.AppendLine("Time: " + _chart.ViewXY.XAxes[0].TimeString(cursor.ValueAtXAxis, "HH:mm:ss.ffff"));

            //Set text
            cursorValueDisplay.Text = sb.ToString();

            //Show the label only if it selected to be shown
            cursorValueDisplay.Visible = labelVisible && showNextToCursor;

            //Allow chart rendering
            _chart.EndUpdate();
        }


        /// <summary>
        /// Solve value from series data points array. Accurate method, but slower than SolveValueCoarse
        /// </summary>
        /// <param name="series">Series</param>
        /// <param name="xValue">X value</param>
        /// <param name="yValue">Output Y value</param>
        /// <returns>Success status</returns>
        private bool SolveValueAccurate(PointLineSeries series, double xValue, out double yValue)
        {
            AxisY axisY = _chart.ViewXY.YAxes[series.AssignYAxisIndex];
            yValue = 0;

            LineSeriesValueSolveResult result = series.SolveYValueAtXValue(xValue);
            if (result.SolveStatus == LineSeriesSolveStatus.OK)
            {
                //PointLineSeries may have two or more points at same X value. If so, center it between min and max 
                yValue = (result.YMax + result.YMin) / 2.0;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Solve value from screen coordinates. Faster method, but not less accurate than SolveValueAccurate 
        /// </summary>
        /// <param name="series">Series</param>
        /// <param name="xValue">X value</param>
        /// <param name="yValue">Output Y value</param>
        /// <returns>Success status</returns>
        private bool SolveValueCoarse(PointLineSeries series, double xValue, out double yValue)
        {
            AxisY axisY = _chart.ViewXY.YAxes[series.AssignYAxisIndex];
            //As the return value is used later as internal coordinate in SolveYCoordAtXCoord(..) method, don't use DPI conversion.
            float coordX = _chart.ViewXY.XAxes[0].ValueToCoord(xValue, false);
            float coordY;
            yValue = 0;

            LineSeriesCoordinateSolveResult result = series.SolveYCoordAtXCoord(coordX);
            if (result.SolveStatus == LineSeriesSolveStatus.OK)
            {
                coordY = (result.CoordBottom + result.CoordTop) / 2f;
                if (axisY.CoordToValue((int)Math.Round(coordY), out yValue, false) == false)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private void checkBoxSnapToNearestDataPoint_Checked(object sender, RoutedEventArgs e)
        {
            if (_chart != null)
            {
                _chart.ViewXY.LineSeriesCursors[0].SnapToPoints = true;
            }
        }

        private void checkBoxSnapToNearestDataPoint_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_chart != null)
            {
                _chart.ViewXY.LineSeriesCursors[0].SnapToPoints = false;
            }
        }
        
        public void Dispose()
        {
            // Don't forget to clear chart grid child list.
            gridChart.Children.Clear();

            if (_chart != null)
            {
                _chart.Dispose();
                _chart = null;
            }
        }

        /***************** Frontend Control Functions *********************************************/

        private double ConvertVoltageToPressure(double voltage)
        {
            const double minVoltage = 0.5;
            const double maxVoltage = 4.5;
            const double maxPressure = 400.0;

            // Check for error state voltage
            if (voltage == -1)
            {
                return -1;
            }

            // Normalize the voltage to a 0-1 scale
            double normalizedVoltage = (voltage - minVoltage) / (maxVoltage - minVoltage);

            // Convert normalized voltage to pressure
            double pressure = normalizedVoltage * maxPressure;

            return pressure;
        }

        private void InitializeTable()
        {
            for (int i = 1; i <= 15; i++) // Assuming you have 15 transducers
            {
                var transducer = new TransducerData(i);
                transducerDataCollection.Add(transducer);
            }

            dataGrid.ItemsSource = transducerDataCollection;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var column = e.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var bindingPath = (column.Binding as Binding).Path.Path;
                    int rowIndex = e.Row.GetIndex();
                    var editedTextbox = e.EditingElement as TextBox;
                    var editedValue = editedTextbox.Text;

                    var transducerData = transducerDataCollection[rowIndex];

                    switch (bindingPath)
                    {
                        case "TransducerName":
                            transducerData.TransducerName = editedValue;
                            break;
                        // Handle other cases if necessary, like editing the Offset or IsActive
                        case "Offset":
                            if (double.TryParse(editedValue, out double offsetValue))
                            {
                                transducerData.Offset = offsetValue;
                            }
                            else
                            {
                                MessageBox.Show("Invalid offset value.");
                            }
                            break;
                        case "IsActive":
                            var editedCheckBox = e.EditingElement as CheckBox;
                            if (editedCheckBox != null)
                            {
                                transducerData.IsActive = editedCheckBox.IsChecked ?? false;
                            }
                            break;
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Code troubleshoot button for MCU connection
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //TODO: Code troubleshoot button for SQL connection
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Starting the test will replace the current data, do you wish to continue?", "Override Data", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            // Disable and Enable Buttons
            startButton.IsEnabled = false;
            pauseButton.IsEnabled = true;
            stopButton.IsEnabled = true;

            // Disable Threshold Input
            minThresh.IsEnabled = false;
            maxThresh.IsEnabled = false;

            // Enable Read-Only for Table
            dataGrid.IsReadOnly = true;

            // Start Timer
            startTime = DateTime.Now;
            lblStartTime.Content = startTime.ToLongTimeString();
            timer.Start();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("This will save all transducer data for the test, do you wish to continue?", "Save Data", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                WriteToDatabase();
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            FindWindow findWindow = new FindWindow();
            var dialogResult = findWindow.ShowDialog(); // ShowDialog makes findWindow modal and waits for it to close

            if (dialogResult == true)
            {
                // Retrieve the selected date from FindWindow
                string selectedDate = findWindow.SelectedDate;

                // Read and input data from database for specified date
                ReadFromDatabase(selectedDate);
            }
        }

        private void Button_Click_StopTest(object sender, RoutedEventArgs e)
        {
            // Stop Test
            timer.Stop();
            var endTime = DateTime.Now;
            lblEndTime.Content = endTime.ToLongTimeString();

            // Disable and Enable Buttons
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;
            pauseButton.IsEnabled = false;
            pauseButton.Content = "Pause";

            // Enable Threshold Input
            minThresh.IsEnabled = true;
            maxThresh.IsEnabled = true;

            // Disable Read-Only for Table
            dataGrid.IsReadOnly = false;

            var elapsed = endTime - startTime;
            lblElapsedTime.Content = $"{elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}";
        }
        private void MinThresh_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(minThresh.Text, out int value) && value >= 0 && value <= maxThreshold)
            {
                minThreshold = value;
            }
            else
            {
                minThresh.Text = minThreshold.ToString();
            }
        }

        private void MaxThresh_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(maxThresh.Text, out int value) && value >= minThreshold && value <= 400)
            {
                maxThreshold = value;
            }
            else
            {
                maxThresh.Text = maxThreshold.ToString();
            }
        }

        private void Button_Click_PauseTest(object sender, RoutedEventArgs e)
        {
            if (timer.IsEnabled)
            {
                // Timer is running, so pause the test
                timer.Stop();
                pauseButton.Content = "Resume"; // Change button text to indicate it will now resume the test

                // Optionally, update other UI elements or state as needed
                startButton.IsEnabled = false; // Keep start disabled while paused
                stopButton.IsEnabled = true; // Allow stopping the test even when paused
            }
            else
            {
                timer.Start();
                pauseButton.Content = "Pause";

                startButton.IsEnabled = false;
                stopButton.IsEnabled = true;
            }
        }

        private void dataGrid_CellEditEnding_1(object sender, DataGridCellEditEndingEventArgs e)
        {

        }
    }
}
