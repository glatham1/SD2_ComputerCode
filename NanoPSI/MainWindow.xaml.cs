using Arction.Wpf.Charting;
using Arction.Wpf.Charting.Annotations;
using Arction.Wpf.Charting.Axes;
using Arction.Wpf.Charting.SeriesXY;
using Arction.Wpf.Charting.Views.ViewXY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NanoPSI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LightningChart _chart = null;

        public MainWindow()
        {
            // Set Deployment Key for LightningChart components
            string deploymentKey = "lgCAAE3C1L0Ig9kBJABVcGRhdGVhYmxlVGlsbD0yMDI1LTA1LTEyI1JldmlzaW9uPTACgD9VCrEQWTcyiWKli80/h/yvJ3PUo9Iacz9kMGmQRMVUlMQFidiOQwCSaOXmsMNu7XFpqENfqtDrAAoepDhhY1eAnA1aWsJYAdPY01icT/20IDzzsnhuXuyWwRO9w4lCbieMD4do7M6U1Wdj0+xAPRuZGrSY3kaJYnp84YvfZ1wPrvvhXbt/EQPkcKstrpo7kjxhEjSEL3p+rIQfz1zde1WFLR0VmMbTZe4M3c7Z5qS9aPQtPT3GDNeQFgtEBdUhFNjh1xVJguD6y8rjVmqyzkelfnNyj/6zMh/C5rZSaBWpmmSNIgD+nJ3CRyA3DoYXDVTlxmbLP16UV0YvttIC/5SJD15lQkPbd+B/KJYc8Ost0wBYmHGR9/lveXThh/NWz8amQyE3It/nn2S7bvWmJoGOtsbqRCsyeGbZBxW2EnNQnlruvy0tlzQ8FvaMlXCeB4LYQntdi12EKGh8QYIxuEdu5BBquigN3fChsf+r+qjE9+hIXXMIgi5YfhTJU9meik8=";

            // Setting Deployment Key for non-bindable chart
            Arction.Wpf.Charting.LightningChart.SetDeploymentKey(deploymentKey);

            InitializeComponent();
            CreateChart();
        }

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
            _chart.ViewXY.AxisLayout.YAxesLayout = YAxesLayout.Stacked;

            _chart.ViewXY.YAxes.Clear(); // Remove existing y-axes.

            //Chart background properties
            _chart.ViewXY.GraphBackground.GradientFill = GradientFill.Radial;

            //Don't show legend box
            _chart.ViewXY.LegendBoxes[0].Visible = false;

            //Chart title text
            _chart.Title.Text = "Pocket Pressures";

            //X-axis title
            _chart.ViewXY.XAxes[0].Title.Text = "Time";
            _chart.ViewXY.XAxes[0].ScrollMode = XAxisScrollMode.None;

            int seriesCount = 15;
            int pointsCount = 500;

            Random rand = new Random();
            const double dXStep = 0.1;
            System.Windows.Media.Color color = Colors.Black;

            //Create series
            for (int seriesIndex = 0; seriesIndex < seriesCount; seriesIndex++)
            {
                //Create new Y axis for each series 
                AxisY axisY = new AxisY(_chart.ViewXY);
                axisY.Units.Text = "psi";
                axisY.Title.Text = "Transducer " + (seriesIndex + 1).ToString();
                axisY.Title.Angle = 0;
                axisY.Title.Font = new WpfFont("Segoe UI", 13, false, false);
                axisY.Title.Color = Colors.White;
                axisY.AutoFormatLabels = false;
                axisY.AutoDivSeparationPercent = 5;
                axisY.LabelsNumberFormat = "0";

                _chart.ViewXY.YAxes.Add(axisY);

                //Create a point-line series
                PointLineSeries pointLineSeries = new PointLineSeries(_chart.ViewXY, _chart.ViewXY.XAxes[0], axisY);
                pointLineSeries.LineStyle.Color = DefaultColors.SeriesForBlackBackgroundWpf[seriesIndex];

                //Set some data into series 
                SeriesPoint[] points = new SeriesPoint[pointsCount];
                double x = 0;
                for (int pointIndex = 0; pointIndex < pointsCount; pointIndex++)
                {
                    points[pointIndex].X = x;
                    points[pointIndex].Y = 300.0 + 800.0 * (0.2 * (rand.NextDouble() - 0.5) + 0.2 * Math.Sin(x * (seriesIndex + 1)));
                    x += dXStep;
                }

                //Assign the points array to series
                pointLineSeries.Points = points;

                color = pointLineSeries.LineStyle.Color;

                //Set title style, for "right-edge" cursor values mode 
                pointLineSeries.Title.HorizontalAlign = AlignmentHorizontal.Right;
                pointLineSeries.Title.Fill.Style = RectFillStyle.ColorOnly;
                pointLineSeries.Title.Fill.Color = ChartTools.CalcGradient(System.Windows.Media.Color.FromArgb(200, 255, 255, 255), System.Windows.Media.Color.FromArgb(200, color.R, color.G, color.B), 50);
                pointLineSeries.Title.Fill.GradientColor = ChartTools.CalcGradient(System.Windows.Media.Color.FromArgb(200, 0, 0, 0), System.Windows.Media.Color.FromArgb(200, color.R, color.G, color.B), 50);
                pointLineSeries.Title.Color = Colors.White;

                //Add point-line series to PointLineSeries list
                _chart.ViewXY.PointLineSeries.Add(pointLineSeries);
            }



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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MCU Troubleshoot code
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //SQL Server Link Troubleshoot code
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //Start Timer
            //Begin Test
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //Save data button to save to server
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            //Find data button to pull from server
        }
    }
}
