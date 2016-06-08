using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Controls
{
 
    public class AxisControl : Control
    {
        static AxisControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AxisControl), new FrameworkPropertyMetadata(typeof(AxisControl)));
        }

        internal static readonly DependencyProperty ParentRadarChartProperty =
            DependencyProperty.Register("ParentRadarChart", typeof(RadarChart), typeof(AxisControl),
                new FrameworkPropertyMetadata());

        internal RadarChart ParentRadarChart
        {
            get {return (RadarChart)GetValue(ParentRadarChartProperty);}
            set {SetValue(ParentRadarChartProperty, value);}
        }

        public static readonly DependencyProperty MaxProperty =
            DependencyProperty.Register("Max", typeof(int), typeof(AxisControl),
                new FrameworkPropertyMetadata(1));

        public int Max
        {
            get { return (int)GetValue(MaxProperty); }
            set {
                SetValue(MaxProperty, value);
                if(ParentRadarChart != null)
                {
                    ParentRadarChart.InvalidateVisual();
                }
            }
        }

        public static readonly DependencyProperty MinProperty =
            DependencyProperty.Register("Min", typeof(int), typeof(AxisControl),
                new FrameworkPropertyMetadata(0));

        public int Min
        {
            get { return (int)GetValue(MinProperty); }
            set {
                SetValue(MinProperty, value);
                if (ParentRadarChart != null)
                {
                    ParentRadarChart.InvalidateVisual();
                }
            }
        }

        public static readonly DependencyProperty MaxBoundaryProperty =
                    DependencyProperty.Register("MaxBoundary", typeof(double), typeof(AxisControl),
                        new FrameworkPropertyMetadata(1D));

        public double MaxBoundary
        {
            get { return (int)GetValue(MaxBoundaryProperty); }
            set { SetValue(MaxBoundaryProperty, value); }
        }

        public static readonly DependencyProperty MinBoundaryProperty =
                    DependencyProperty.Register("MinBoundary", typeof(double), typeof(AxisControl),
                        new FrameworkPropertyMetadata(0D));

        public int MinBoundary
        {
            get { return (int)GetValue(MinBoundaryProperty); }
            set { SetValue(MinBoundaryProperty, value); }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.MouseLeftButtonUp += AxisControl_MouseLeftButtonUp;

        }



        private void AxisControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(ParentRadarChart != null)
            {
                AxisClickedEventArgs args = new AxisClickedEventArgs((Axis)DataContext);
                ParentRadarChart.OnAxisClicked(args);
            }
        }
    }
}
