using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Controls
{
    public class ChartLineControl : Shape
    {
        static ChartLineControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartLineControl), new FrameworkPropertyMetadata(typeof(ChartLineControl)));
        }

        #region Dependency Property/Event Definitions

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(IList<double>), typeof(ChartLineControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));


        internal static readonly DependencyProperty ParentRadarChartProperty =
            DependencyProperty.Register("ParentRadarChart", typeof(RadarChart), typeof(ChartLineControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender,ParentRadarChart_PropertyChanged));


        #endregion Dependency Property/Event Definitions

        /// <summary>
        /// The represented data values.
        /// </summary>
        public IList<double> Data
        {
            get
            {
                return (IList<double>)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        internal RadarChart ParentRadarChart
        {
            get
            {
                return (RadarChart)GetValue(ParentRadarChartProperty);
            }
            set
            {
                SetValue(ParentRadarChartProperty, value);
            }
        }

        #region Private Methods

        /// <summary>
        /// Return the shape's geometry.
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                //
                // Geometry has not yet been generated.
                // Generate geometry and cache it.
                //
                
                PathGeometry g = new PathGeometry();
                if (ParentRadarChart == null) {
                    return g;
                }
                if (Data == null)
                {
                    return g;
                }
                List<Point> points = Points;
                PathFigure f = new PathFigure();
                f.StartPoint = points[0];
                f.IsClosed = true;
                for (int i = 0; i < points.Count; i++)
                {
                    f.Segments.Add(new LineSegment(points[i], true));
                }
                g.Figures.Add(f);
                return g;
            }
        }

        private List<Point> Points
        {
            get
            {
                var minSide = Math.Min(ActualWidth, ActualHeight);
                Point center = new Point(minSide / 2, minSide / 2);
                double angleIncrementRad = 2 * Math.PI / ParentRadarChart.Axes.Count;

                double axisValue = 0;
                double axisLength = minSide / 2;
                double angleRad = Math.PI / 2; //first axis is vertical
                List<Point> points = new List<Point>();
                Axis axis;
                for (int i = 0; i < ParentRadarChart.Axes.Count; i++)
                {
                    axis = ParentRadarChart.Axes[i];
                    axisValue = axis.Min;
                    if (i < Data.Count)
                    {
                        if (Data[i] > axis.Min && Data[i] < axis.Max)
                        {
                            axisValue = Data[i];
                        }
                    }
                    double pointlength = (axisValue - axis.Min) / (axis.Max - axis.Min) * axisLength;
                    Point p = new Point();
                    p.X = center.X + Math.Cos(angleRad) * pointlength;
                    p.Y = center.Y - Math.Sin(angleRad) * pointlength;
                    points.Add(p);
                    angleRad -= angleIncrementRad;
                }
                return points;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return constraint;
        }

        private static void ParentRadarChart_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartLineControl c = (ChartLineControl)d;
            
        }
        #endregion

    }
}
