using Arction.Wpf.Charting;
using Arction.Wpf.Charting.Annotations;
using Arction.Wpf.Charting.Axes;
using Arction.Wpf.Charting.SeriesXY;
using Arction.Wpf.Charting.Views.ViewXY;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Bluetooth.Factory;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace NanoPSI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LightningChart _chart = null;
        private DispatcherTimer timer;
        private DateTime startTime;
        BluetoothClient client = new BluetoothClient();
        private Random random = new Random(); // TEMP FOR TESTING

        public MainWindow()
        {
            // Set Deployment Key for LightningChart components
            string deploymentKey = "lgCAAE3C1L0Ig9kBJABVcGRhdGVhYmxlVGlsbD0yMDI1LTA1LTEyI1JldmlzaW9uPTACgD9VCrEQWTcyiWKli80/h/yvJ3PUo9Iacz9kMGmQRMVUlMQFidiOQwCSaOXmsMNu7XFpqENfqtDrAAoepDhhY1eAnA1aWsJYAdPY01icT/20IDzzsnhuXuyWwRO9w4lCbieMD4do7M6U1Wdj0+xAPRuZGrSY3kaJYnp84YvfZ1wPrvvhXbt/EQPkcKstrpo7kjxhEjSEL3p+rIQfz1zde1WFLR0VmMbTZe4M3c7Z5qS9aPQtPT3GDNeQFgtEBdUhFNjh1xVJguD6y8rjVmqyzkelfnNyj/6zMh/C5rZSaBWpmmSNIgD+nJ3CRyA3DoYXDVTlxmbLP16UV0YvttIC/5SJD15lQkPbd+B/KJYc8Ost0wBYmHGR9/lveXThh/NWz8amQyE3It/nn2S7bvWmJoGOtsbqRCsyeGbZBxW2EnNQnlruvy0tlzQ8FvaMlXCeB4LYQntdi12EKGh8QYIxuEdu5BBquigN3fChsf+r+qjE9+hIXXMIgi5YfhTJU9meik8=";
            Arction.Wpf.Charting.LightningChart.SetDeploymentKey(deploymentKey);

            InitializeComponent();
            CreateChart();
            InitializeTimer();
            DiscoverDevices();
        }

        /**************** SQL Connection Functions ************************************************/

        /*
         *  1:3.4,2:2.5,3:-1
        */

        /**************** Bluetooth Connection Functions ******************************************/
        
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

        private void mcuConnect_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (mcuConnect.SelectedItem is BluetoothDeviceInfo selectedDevice)
            {
                ConnectToDevice(selectedDevice);
            }
        }

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
        /*
        private string ReadMCU()
        {
            try
            {
                if (client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    if (stream.CanRead)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        return Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    }
                    else
                    {
                        return "Cannot read from device";
                    }
                }
                else
                {
                    return "Not connected to device";
                }
            }
            catch (Exception ex)
            {
                return $"Error reading from device: {ex.Message}";
            }
        }

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
        */

        private double ReadMCU()
        {
            // TODO: Need to read ASCII values from microcontroller here and return them as a double
            
            return random.NextDouble() * 100;
        }

        /***************** Timer Functions *********************************************************/

        private void InitializeTimer()
        {
            // Create a new timer
            timer = new DispatcherTimer();
            // Set the interval to 1 second
            timer.Interval = TimeSpan.FromSeconds(0.25);
            // Assign the event that's called each tick
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var elapsed = now - startTime;
            lblElapsedTime.Content = $"{elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}";

            // Update the chart with new data
            UpdateChartData(elapsed.TotalSeconds, ReadMCU());
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
            _chart.ViewXY.YAxes.Add(yAxis);

            //Chart background properties
            _chart.ViewXY.GraphBackground.GradientFill = GradientFill.Radial;

            //Don't show legend box
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

            // Create and add a PointLineSeries to the chart
            var series = new PointLineSeries(_chart.ViewXY, xAxis, yAxis);
            series.Title.Text = "Pressure Data";
            _chart.ViewXY.PointLineSeries.Add(series); // Will need to add more PointLineSeries per transducer
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

        private void UpdateChartData(double time, double pressure)
        {
            // TODO: Adjust to update all 15 point line series with time and pressure
            
            _chart.BeginUpdate();

            var series = _chart.ViewXY.PointLineSeries[0];
            var points = series.Points.ToList(); // Convert the array to a list
            points.Add(new SeriesPoint(time, pressure)); // Add the new point
            series.Points = points.ToArray(); // Convert the list back to an array

            // Adjust X-axis to show the latest data
            _chart.ViewXY.XAxes[0].SetRange(0, time);

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
            // Start Timer
            startTime = DateTime.Now;
            lblStartTime.Content = startTime.ToLongTimeString();
            timer.Start();

            //Disable and Enable Buttons
            startButton.IsEnabled = false;
            stopButton.IsEnabled = true;

            //Begin Test
            // TODO: Add functionality to Real Time Pressure Data table
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            // TODO: Save data button to save to server
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            // TODO: Find data button to pull from server
        }

        private void Button_Click_StopTest(object sender, RoutedEventArgs e)
        {
            // Stop Test
            timer.Stop();
            var endTime = DateTime.Now;
            lblEndTime.Content = endTime.ToLongTimeString();

            //Disable and Enable Buttons
            startButton.IsEnabled = true;
            stopButton.IsEnabled = false;

            var elapsed = endTime - startTime;
            lblElapsedTime.Content = $"{elapsed.Hours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}";
        }
    }
}
