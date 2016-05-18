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
