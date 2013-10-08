using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WpfXScope.ViewModels;
using Xceed.Wpf.Toolkit.Chromes;

namespace WpfXScope.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class XScopeControl
    {
        private static readonly string[] VoltageDivisionLabels = {
                                                            "5.12V",
                                                            "2.56V",
                                                            "1.28V",
                                                            "0.64V",
                                                            "0.32V",
                                                            "0.16V",
                                                            "80mV"
                                                        };

        private static readonly double[] VoltageDivisionFactors = {
                                                                    5.12,
                                                                    2.56,
                                                                    1.28,
                                                                    .64,
                                                                    .32,
                                                                    .16,
                                                                    .08
                                                                };

        private static readonly double[] TimeDivisonFactors = {
                                                                .000008,
                                                                .000016,
                                                                .000032,
                                                                .000064,
                                                                .000128,
                                                                .000256,
                                                                .000500,
                                                                .001,
                                                                .002,
                                                                .005,
                                                                .01,
                                                                .05,
                                                                .1,
                                                                .2,
                                                                .5,
                                                                1,
                                                                2,
                                                                5,
                                                                10,
                                                                20,
                                                                50
                                                            };

        private static readonly string[] TimeDivisionLabels = {
                                                         "8us",
                                                         "16us",
                                                         "32us",
                                                         "64us",
                                                         "128us",
                                                         "256us",
                                                         "500us",
                                                         "1ms",
                                                         "2ms",
                                                         "5ms",
                                                         "10ms",
                                                         "50ms",
                                                         "0.1s",
                                                         "0.2s",
                                                         "0.5s",
                                                         "1s",
                                                         "2s",
                                                         "5s",
                                                         "10s",
                                                         "20s",
                                                         "50s"
                                                     };


        public const int VerticalOffset = -126;

        private readonly List<Measurement> _measurements = new List<Measurement>();

        public XScopeControl()
        {
            InitializeComponent();
        }

        public void Redraw()
        {
            ControlCanvas.Children.Clear();

            DrawGrid();
            RenderData();
        }

        #region Routed Events

        public static readonly RoutedEvent ScrollEvent = EventManager.RegisterRoutedEvent(
            "Scroll", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(XScopeControl));

        public event RoutedEventHandler Scroll
        {
            add { AddHandler(ScrollEvent, value); }
            remove { RemoveHandler(ScrollEvent, value); }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty ScrollPositionProperty =
            DependencyProperty.Register(
                "ScrollPosition",
                typeof(int),
                typeof(XScopeControl),
                new FrameworkPropertyMetadata(0)
                );

        public int ScrollPosition
        {
            get { return (int)ScrollBarHPos.Value; }
            set { SetValue(ScrollPositionProperty, value); ScrollBarHPos.Value = value; }
        }


        public static readonly DependencyProperty SamplingRateProperty =
            DependencyProperty.Register(
                "SamplingRate",
                typeof(int),
                typeof(XScopeControl),
                new FrameworkPropertyMetadata(0)
                );

        public int SamplingRate
        {
            get { return (int)GetValue(SamplingRateProperty); }
            set { SetValue(SamplingRateProperty, value); }
        }


        public static readonly DependencyProperty Channel2ColorProperty = DependencyProperty.Register(
            "Channel2Color", typeof(Color), typeof(XScopeControl), new FrameworkPropertyMetadata(Colors.Green,
                                                                                                   FrameworkPropertyMetadataOptions
                                                                                                       .AffectsRender));

        public Color Channel2Color
        {
            get { return (Color)GetValue(Channel2ColorProperty); }
            set { SetValue(Channel2ColorProperty, value); }
        }

        public static readonly DependencyProperty ChannelDTraceProperty = DependencyProperty.Register(
            "ChannelDTrace", typeof(bool), typeof(XScopeControl), new FrameworkPropertyMetadata(true,
                                                                                                  FrameworkPropertyMetadataOptions
                                                                                                      .AffectsRender));

        public bool ChannelDTrace
        {
            get { return (bool)GetValue(ChannelDTraceProperty); }
            set { SetValue(ChannelDTraceProperty, value); }
        }


        public static readonly DependencyProperty Channel2TraceProperty = DependencyProperty.Register(
            "Channel2Trace", typeof(bool), typeof(XScopeControl), new FrameworkPropertyMetadata(true,
                                                                                                  FrameworkPropertyMetadataOptions
                                                                                                      .AffectsRender));

        public bool Channel2Trace
        {
            get { return (bool)GetValue(Channel2TraceProperty); }
            set { SetValue(Channel2TraceProperty, value); }
        }

        public static readonly DependencyProperty Channel2PositionProperty =
            DependencyProperty.Register(
                "Channel2Position",
                typeof(short),
                typeof(XScopeControl),
                new FrameworkPropertyMetadata((short)0)
                );

        public short Channel2Position
        {
            get { return (short)GetValue(Channel2PositionProperty); }
            set { SetValue(Channel2PositionProperty, value); }
        }

        public static readonly DependencyProperty Channel1PositionProperty =
            DependencyProperty.Register(
                "Channel1Position",
                typeof(short),
                typeof(XScopeControl),
                new FrameworkPropertyMetadata((short)0)
                );

        public short Channel2Gain
        {
            get { return (short)GetValue(Channel2GainProperty); }
            set { SetValue(Channel2GainProperty, value); }
        }


        public static readonly DependencyProperty Channel2GainProperty =
            DependencyProperty.Register(
                "Channel2Gain",
                typeof(short),
                typeof(XScopeControl),
                new FrameworkPropertyMetadata((short)0)
                );

        public short Channel1Gain
        {
            get { return (short)GetValue(Channel1GainProperty); }
            set { SetValue(Channel1GainProperty, value); }
        }

        public static readonly DependencyProperty Channel1GainProperty =
            DependencyProperty.Register(
                "Channel1Gain",
                typeof(short),
                typeof(XScopeControl),
                new FrameworkPropertyMetadata((short)0)
                );

        public short Channel1Position
        {
            get { return (short)GetValue(Channel1PositionProperty); }
            set { SetValue(Channel1PositionProperty, value); }
        }

        public static readonly DependencyProperty ScopeDataProperty =
            DependencyProperty.Register(
                "ScopeData",
                typeof(ObservableCollection<byte>),
                typeof(XScopeControl),
                new FrameworkPropertyMetadata(new ObservableCollection<byte>(), FrameworkPropertyMetadataOptions.AffectsRender)
                );

        public ObservableCollection<byte> ScopeData
        {
            get { return (ObservableCollection<byte>)GetValue(ScopeDataProperty); }
            set { SetValue(ScopeDataProperty, value); }
        }


        public static readonly DependencyProperty GridLineColorProperty = DependencyProperty.Register(
            "GridLineColor", typeof(Color), typeof(XScopeControl), new FrameworkPropertyMetadata(Colors.White,
                                                                                                   FrameworkPropertyMetadataOptions
                                                                                                       .AffectsRender));

        public Color GridLineColor
        {
            get { return (Color)GetValue(GridLineColorProperty); }
            set { SetValue(GridLineColorProperty, value); }
        }

        public static readonly DependencyProperty ScopeBackgroundColorProperty = DependencyProperty.Register(
            "ScopeBackgroundColor", typeof(Color), typeof(XScopeControl), new FrameworkPropertyMetadata(Colors.Black,
                                                                                                          FrameworkPropertyMetadataOptions
                                                                                                              .
                                                                                                              AffectsRender));

        public Color ScopeBackgroundColor
        {
            get { return (Color)GetValue(ScopeBackgroundColorProperty); }
            set { SetValue(ScopeBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty Channel1ColorProperty = DependencyProperty.Register(
            "Channel1Color", typeof(Color), typeof(XScopeControl), new FrameworkPropertyMetadata(Colors.Green,
                                                                                                   FrameworkPropertyMetadataOptions
                                                                                                       .AffectsRender));

        public Color Channel1Color
        {
            get { return (Color)GetValue(Channel1ColorProperty); }
            set { SetValue(Channel1ColorProperty, value); }
        }

        public static readonly DependencyProperty Channel1TraceProperty = DependencyProperty.Register(
            "Channel1Trace", typeof(bool), typeof(XScopeControl), new FrameworkPropertyMetadata(true,
                                                                                                  FrameworkPropertyMetadataOptions
                                                                                                      .AffectsRender));

        public bool Channel1Trace
        {
            get { return (bool)GetValue(Channel1TraceProperty); }
            set { SetValue(Channel1TraceProperty, value); }
        }

        public static readonly DependencyProperty RenderLinesProperty = DependencyProperty.Register(
            "RenderLines", typeof(bool), typeof(XScopeControl), new FrameworkPropertyMetadata(true,
                                                                                                FrameworkPropertyMetadataOptions
                                                                                                    .AffectsRender));

        public bool RenderLines
        {
            get { return (bool)GetValue(RenderLinesProperty); }
            set { SetValue(RenderLinesProperty, value); }
        }

        public static readonly DependencyProperty DrawGridLinesProperty = DependencyProperty.Register(
            "DrawGridLines", typeof(bool), typeof(XScopeControl), new FrameworkPropertyMetadata(true,
                                                                                                FrameworkPropertyMetadataOptions
                                                                                                    .AffectsRender));

        public bool DrawGridLines
        {
            get { return (bool)GetValue(DrawGridLinesProperty); }
            set { SetValue(DrawGridLinesProperty, value); }
        }

        public static readonly DependencyProperty DrawTickMarksProperty = DependencyProperty.Register(
            "DrawTickMarks", typeof(bool), typeof(XScopeControl), new FrameworkPropertyMetadata(true,
                                                                                                FrameworkPropertyMetadataOptions
                                                                                                    .AffectsRender));

        public bool DrawTickMarks
        {
            get { return (bool)GetValue(DrawTickMarksProperty); }
            set { SetValue(DrawTickMarksProperty, value); }
        }

        #endregion

        #region Render

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            Redraw();

            FrequencyLabel.Content = "Time: " + TimeDivisionLabels[SamplingRate] + "/div";
            Channel1Label.Content = "Ch1: " + VoltageDivisionLabels[Channel1Gain] + "/div";
            Channel2Label.Content = "Ch2: " + VoltageDivisionLabels[Channel2Gain] + "/div";

            if (_isDragging)
                DrawDragLine();

            AddMeasures();
        }

        private void DrawDragLine()
        {
            var mouseLine = new Line
            {
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 2,
                X1 = _firstMousePoint.X,
                Y1 = _firstMousePoint.Y,
                X2 = _lastMousePoint.X,
                Y2 = _lastMousePoint.Y
            };


            ControlCanvas.Children.Add(mouseLine);
        }

        private void AddMeasures()
        {
            foreach (var measurement in _measurements)
            {
                var line = new Line
                {
                    Stroke = new SolidColorBrush(Colors.Yellow),
                    StrokeThickness = 2,
                    X1 = measurement.StartPoint.X - (4 * ScrollPosition),
                    Y1 = measurement.StartPoint.Y,
                    X2 = measurement.EndPoint.X - (4 * ScrollPosition),
                    Y2 = measurement.EndPoint.Y
                };

                Panel.SetZIndex(line, 2);
                ControlCanvas.Children.Add(line);

                var startPoint = new Point(line.X1, line.Y1);
                var endPoint = new Point(line.X2, line.Y2);
                var v = startPoint - endPoint;
                var midpoint = endPoint + (v / 2);
                var theta = Math.Atan2(v.Y, v.X);
                var offset = Math.PI / 2;
                if (v.Y > 0)
                    offset = -Math.PI / 2;

                var theta2 = theta + offset;
                var x4 = 10 * Math.Cos(theta2) + midpoint.X;
                var y4 = 10 * Math.Sin(theta2) + midpoint.Y;

                var x3 = 30 * Math.Cos(theta2) + midpoint.X;
                var y3 = 30 * Math.Sin(theta2) + midpoint.Y;

                var redline = new Line
                {
                    Stroke = new SolidColorBrush(Colors.Yellow),
                    X1 = x4,
                    Y1 = y4,
                    X2 = x3,
                    Y2 = y3,
                    StrokeThickness = 2
                };

                // anchor points
                AddAnchor(startPoint);
                AddAnchor(endPoint);

                var panel = new StackPanel();
                var outerBorder = new Border
                {
                    Background = new SolidColorBrush(Color.FromArgb(0xa2, 0xaf, 0x2a, 0x2a)),
                    CornerRadius = new CornerRadius(4),
                };

                var innerBorder = new Border
                {
                    Margin = new Thickness(4),
                    Background = new SolidColorBrush(Color.FromArgb(0xa2, 0xf2, 0xda, 0xc7)),
                    CornerRadius = new CornerRadius(4)
                };

                panel.Children.Add(outerBorder);
                outerBorder.Child = innerBorder;

                var grid = new Grid { Margin = new Thickness(5) };
                grid.ColumnDefinitions.Add(new ColumnDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());

                var rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(1, GridUnitType.Auto);

                grid.RowDefinitions.Add(rowDefinition);
                grid.RowDefinitions.Add(new RowDefinition());
                grid.RowDefinitions.Add(new RowDefinition());

                var voltageLabel = new Label { Foreground = new SolidColorBrush(Colors.Brown), Margin = new Thickness(0, 0, 2, 0), Content = "Voltage:", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right, FontSize = 10.0 };
                var voltageValueLabel = new Label { FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.White), Content = measurement.VoltageMeasure, VerticalAlignment = VerticalAlignment.Center, FontSize = 10.0 };
                var timeLabel = new Label { Foreground = new SolidColorBrush(Colors.Brown), Margin = new Thickness(0, 0, 2, 0), Content = "Time:", VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right, FontSize = 10.0 };
                var timeValueLabel = new Label { FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Colors.White), Content = measurement.TimeMeasure, VerticalAlignment = VerticalAlignment.Center, FontSize = 10.0 };

                var button = new ButtonChrome();
                button.Content = "X";
                button.HorizontalAlignment = HorizontalAlignment.Right;
                button.FontSize = 10.0;
                button.Padding = new Thickness(4);
                button.CornerRadius = new CornerRadius(4);

                button.Cursor = Cursors.Hand;

                button.MouseDown += ButtonOnMouseDown;

                Grid.SetColumn(voltageValueLabel, 1);
                Grid.SetColumn(timeValueLabel, 1);

                Grid.SetRow(voltageLabel, 1);
                Grid.SetRow(voltageValueLabel, 1);

                Grid.SetRow(timeLabel, 2);
                Grid.SetRow(timeValueLabel, 2);

                Grid.SetColumn(button, 1);

                var title = new Label();
                title.Content = "Measurement";
                title.FontSize = 10;
                title.HorizontalAlignment = HorizontalAlignment.Right; 
                title.Foreground = new SolidColorBrush(Colors.White);
                title.FontWeight = FontWeights.Bold;

                grid.Children.Add(title);
                grid.Children.Add(voltageLabel);
                grid.Children.Add(voltageValueLabel);
                grid.Children.Add(timeLabel);
                grid.Children.Add(timeValueLabel);
                grid.Children.Add(button);

                innerBorder.Child = grid;

                panel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                var top = y3;
                if ((endPoint.X <= startPoint.X && endPoint.Y <= startPoint.Y) || (endPoint.X >= startPoint.X && endPoint.Y >= startPoint.Y))
                {
                    top -= panel.DesiredSize.Height;
                }

                Panel.SetZIndex(panel, 1);
                Canvas.SetLeft(panel, x3);
                Canvas.SetTop(panel, top);


                var polyLineArrowHead = AddArrowHead(midpoint, theta2);

                ControlCanvas.Children.Add(redline);
                ControlCanvas.Children.Add(polyLineArrowHead);
                ControlCanvas.Children.Add(panel);
            }
        }

        private void ButtonOnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            _measurements.Clear();
        }

        private void AddAnchor(Point anchorPoint)
        {
            var outterEllipse = new Ellipse {Width = 8, Height = 8, Fill = new SolidColorBrush(Colors.White)};
            Canvas.SetLeft(outterEllipse, anchorPoint.X - 4);
            Canvas.SetTop(outterEllipse, anchorPoint.Y - 4);
            Panel.SetZIndex(outterEllipse, 2);
            ControlCanvas.Children.Add(outterEllipse);

            var innerEllipse = new Ellipse {Width = 4, Height = 4, Fill = new SolidColorBrush(Colors.Red)};
            Canvas.SetLeft(innerEllipse, anchorPoint.X - 2);
            Canvas.SetTop(innerEllipse, anchorPoint.Y - 2);
            Panel.SetZIndex(innerEllipse, 2);
            ControlCanvas.Children.Add(innerEllipse);
        }

        private static Polyline AddArrowHead(Point midpoint, double arrowFacingDirection)
        {
            const double arrowSize = 11.5;
            const double arrowHeadAngle = 30 * (Math.PI / 180);

            var arrowX = 2 * Math.Cos(arrowFacingDirection) + midpoint.X;
            var arrowY = 2 * Math.Sin(arrowFacingDirection) + midpoint.Y;

            var arrowPoint = new Point(arrowX, arrowY);
            var p1 = new Point(Math.Cos(arrowFacingDirection + arrowHeadAngle) * arrowSize + arrowPoint.X, Math.Sin(arrowFacingDirection + arrowHeadAngle) * arrowSize + arrowPoint.Y);
            var p2 = new Point(Math.Cos(arrowFacingDirection - arrowHeadAngle) * arrowSize + arrowPoint.X, Math.Sin(arrowFacingDirection - arrowHeadAngle) * arrowSize + arrowPoint.Y);

            var polyLine = new Polyline();
            polyLine.Points.Add(arrowPoint);
            polyLine.Points.Add(p1);
            polyLine.Points.Add(p2);
            polyLine.Points.Add(arrowPoint);
            polyLine.Fill = new SolidColorBrush(Colors.Yellow);
            Panel.SetZIndex(polyLine, 1);
            return polyLine;
        }

        #region Scope Data

        private void RenderData()
        {
            var xmax = Math.Min(512, ScopeData.Count); // display size
            UInt16 i = 0, inc = 2;
            if (SamplingRate < 11)
            {
                i = (UInt16)ScrollBarHPos.Value;
                inc = 4;
            }

            var graph = new Polyline { Stroke = new SolidColorBrush(Channel1Color), StrokeThickness = 1.5 };
            for (var xpos = 0; xpos < xmax; i++, xpos += inc)
            {
                if (!Channel1Trace) continue;
                var yint = ((64 + VerticalOffset - Channel1Position) + ScopeData[i]) * 2;

                if (RenderLines)
                    graph.Points.Add(new Point(xpos, yint));
                else
                {
                    var point = new Ellipse { Width = 2.5, Height = 2.5, Fill = new SolidColorBrush(Channel1Color) };
                    ControlCanvas.Children.Add(point);

                    Canvas.SetLeft(point, xpos);
                    Canvas.SetTop(point, yint);
                }
            }

            if (Channel1Trace && RenderLines)
            {
                ControlCanvas.Children.Add(graph);
            }

            if (SamplingRate < 11)
            {
                i = (UInt16)(256 + ScrollBarHPos.Value);
                inc = 4;
            }

            graph = new Polyline { Stroke = new SolidColorBrush(Channel2Color), StrokeThickness = 1.5 };
            for (var xpos = 0; xpos < xmax; i++, xpos += inc)
            {
                if (!Channel2Trace) continue;
                var yint = ((64 + VerticalOffset - Channel2Position) + ScopeData[i]) * 2;

                if (RenderLines)
                    graph.Points.Add(new Point(xpos, yint));
                else
                {
                    var point = new Ellipse { Width = 2.5, Height = 2.5, Fill = new SolidColorBrush(Channel2Color) };
                    ControlCanvas.Children.Add(point);

                    Canvas.SetLeft(point, xpos);
                    Canvas.SetTop(point, yint);

                }
            }

            if (RenderLines && Channel2Trace)
                ControlCanvas.Children.Add(graph);


            if (SamplingRate < 11)
            {
                i = (UInt16)(512 + +ScrollBarHPos.Value);
                inc = 4;
            }

            graph = new Polyline { Stroke = new SolidColorBrush(Channel2Color), StrokeThickness = 1.5 };
            for (var xpos = 0; xpos < xmax; i++, xpos += inc)
            {
                if (!ChannelDTrace) continue;
                var yint = ((64 + VerticalOffset - Channel2Position) + ScopeData[i]) * 2;

                if (RenderLines)
                    graph.Points.Add(new Point(xpos, yint));
                else
                {
                    var point = new Ellipse { Width = 2.5, Height = 2.5, Fill = new SolidColorBrush(Channel1Color) };
                    ControlCanvas.Children.Add(point);

                    Canvas.SetLeft(point, xpos);
                    Canvas.SetTop(point, yint);

                }
            }

            if (ChannelDTrace && RenderLines)
                ControlCanvas.Children.Add(graph);

        }

        #endregion

        private const int GridStepSize = 64;

        #region Grid
        private void DrawGrid()
        {
            var canvas = ControlCanvas;

            const int gridSize = 512;
            const int tickheight = 4;
            const int tickheightLarge = 8;

            const int halfTickHeight = tickheight / 2;
            const int halfTickHeightLarge = tickheightLarge / 2;


            var rectangle = new Rectangle { Width = gridSize, Height = gridSize, Fill = new SolidColorBrush(ScopeBackgroundColor) };
            canvas.Children.Add(rectangle);

            var lineBrush = new SolidColorBrush(GridLineColor);
            for (var i = 0; i < gridSize; i += GridStepSize)
            {
                var dottedBrush = new SolidColorBrush(Color.FromArgb(0x60, 0xff, 0xff, 0xff));
                if (DrawGridLines)
                {
                    var line = new Line { X1 = 0, Y1 = i, X2 = gridSize, Y2 = i, Stroke = lineBrush };
                    canvas.Children.Add(line);

                    line = new Line { X1 = i, Y1 = 0, X2 = i, Y2 = gridSize, Stroke = lineBrush };
                    canvas.Children.Add(line);
                }


                if (i != 256 || !DrawTickMarks) continue;

                for (var j = 0; j < gridSize; j += 8)
                {
                    if (j % 32 != 0)
                    {
                        canvas.Children.Add(new Line { X1 = j, Y1 = (i - halfTickHeight), X2 = j, Y2 = (i + halfTickHeight), Stroke = lineBrush });
                        canvas.Children.Add(new Line { X1 = (i - halfTickHeight), Y1 = j, X2 = (i + halfTickHeight), Y2 = j, Stroke = lineBrush });
                    }
                    else
                    {
                        canvas.Children.Add(new Line { X1 = j, Y1 = (i - halfTickHeightLarge), X2 = j, Y2 = (i + halfTickHeightLarge), Stroke = lineBrush });
                        canvas.Children.Add(new Line { Y1 = j, X1 = (i - halfTickHeightLarge), Y2 = j, X2 = (i + halfTickHeightLarge), Stroke = lineBrush });
                    }

                    // add dotted line

                }

                if (i == 256)
                {
                    for (var j = 0; j < gridSize; j += 8)
                    {
                        var dashArray = new DoubleCollection { 1, 8 };
                        var line = new Line
                        {
                            X1 = 0,
                            X2 = gridSize,
                            Y1 = j,
                            Y2 = j,
                            StrokeDashArray = dashArray,
                            Stroke = dottedBrush
                        };
                        canvas.Children.Add(line);
                    }
                }
            }
        }
        #endregion

        #endregion

        private Point _firstMousePoint = new Point(0, 0);
        private Point _lastMousePoint = new Point(0, 0);
        private bool _isDragging;

        // This method raises the Tap event 
        void RaiseScrollEvent(int position)
        {
            var newEventArgs = new ScopeScrollEventArgs { Position = position, RoutedEvent = ScrollEvent };
            RaiseEvent(newEventArgs);
            ScrollPosition = position;
        }

        private void ScrollBarHPosScroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            RaiseScrollEvent((int)e.NewValue);
        }

        private void ScopeMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ControlCanvas.ReleaseMouseCapture();
            if (!_isDragging) return;

            _isDragging = false;

            var endPosition = e.GetPosition(this);

            endPosition.X += (4 * ScrollPosition);

            var startPosition = _firstMousePoint;
            startPosition.X += (4 * ScrollPosition);

            AddScopeMeasure(startPosition, endPosition);
            InvalidateVisual();
        }

        private void AddScopeMeasure(Point startPosition, Point endPosition)
        {
            var v = endPosition - startPosition;
            if (v.LengthSquared <= 50) return;

            var line = new Line
                           {
                               Stroke = new SolidColorBrush(Colors.White),
                               StrokeThickness = 2,
                               X1 = startPosition.X,
                               Y1 = startPosition.Y,
                               X2 = endPosition.X,
                               Y2 = endPosition.Y
                           };

            Panel.SetZIndex(line, 2);

            // get x, y values of vector
            var deltaX = Math.Abs(startPosition.X - endPosition.X);
            var deltaY = Math.Abs(startPosition.Y - endPosition.Y);

            var timeMeasureValue = (deltaX/GridStepSize)*TimeDivisonFactors[SamplingRate];
            var voltageMeasureValue = (deltaY/GridStepSize)*VoltageDivisionFactors[Channel1Gain];

            var timeMeasure = GetFormatedString(timeMeasureValue) + "s";
            var voltageMeasure = GetFormatedString(voltageMeasureValue) + "V";
            var measurement = new Measurement
                                  {StartPoint = startPosition, EndPoint = endPosition, TimeMeasure = timeMeasure, VoltageMeasure = voltageMeasure};

            _measurements.Clear();

            _measurements.Add(measurement);
        }

        private void ScopeMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            if (!ValidatePosition(position))
            {
                ControlCanvas.ReleaseMouseCapture();
                _isDragging = false;

                InvalidateVisual();
            }


            if (_isDragging)
            {
                _lastMousePoint = position;
                InvalidateVisual();
            }

            CursorX.Content = "X: " + position.X;
            CursorY.Content = "Y: " + position.Y;
        }

        private bool ValidatePosition(Point position)
        {
            return (position.X >= 0 && position.X <= 511 && position.Y >= 0 && position.Y <= 511);
        }

        private void ScopeMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = true;

            ControlCanvas.CaptureMouse();
            _firstMousePoint = _lastMousePoint = e.GetPosition(this);
        }

        private void ControlCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            _isDragging = false;
            ControlCanvas.ReleaseMouseCapture();
        }

        private static string[] modifier = new[] { "m", "u", "p", "n" };
        private static string GetFormatedString(double value)
        {
            int exponentPart;
            double decimalPart;

            var builder = new StringBuilder();

            GetExponentParts(value, out exponentPart, out decimalPart);
            if (exponentPart < 0)
            {
                exponentPart *= -1;

                var closestOffset = exponentPart / 3;

                if (closestOffset > 3)
                {
                    var raisePower = -((closestOffset - 3) - 1) * 3;
                    closestOffset = 3;
                    decimalPart = decimalPart * Math.Pow(10, raisePower);
                }
                else if (exponentPart % 3 != 0)
                {
                    var raisePower = 3 - exponentPart % 3;
                    decimalPart = decimalPart * Math.Pow(10, raisePower);
                }
                else
                {
                    closestOffset -= 1;
                }

                builder.Append(decimalPart);
                builder.Append(" ");
                builder.Append(modifier[closestOffset]);
            }
            else
            {
                builder.Append(value);
            }

            return builder.ToString();
        }

        private static void GetExponentParts(double value, out int exponentPart, out double decimalPart)
        {

            exponentPart = 0;
            decimalPart = 0;

            string result = value.ToString("0.###E+0");
            var match = rxScientific.Match(result);
            if (match.Success)
            {
                exponentPart = int.Parse(match.Groups["exponent"].Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);

                var decimalString = match.Groups["head"].Value + "." + match.Groups["tail"].Value;
                double.TryParse(decimalString, out decimalPart);
            }
        }

        private static readonly Regex rxScientific = new Regex(@"^(?<sign>-?)(?<head>\d+)(\.(?<tail>\d*?)0*)?E(?<exponent>[+\-]\d+)$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant);

        private void ControlCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            //ControlCanvas.CaptureMouse();
        }
    }

    internal class Measurement
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public string VoltageMeasure { get; set; }
        public string TimeMeasure { get; set; }
    }
}