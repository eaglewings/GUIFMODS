using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Controls
{
    ///<summary>
    ///  Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    ///  Step 1a) Using this custom control in a XAML file that exists in the current project.
    ///  Add this XmlNamespace attribute to the root element of the markup file where it is 
    ///  to be used:
    ///
    ///  xmlns:MyNamespace="clr-namespace:WpfCharts"
    ///
    ///
    ///  Step 1b) Using this custom control in a XAML file that exists in a different project.
    ///  Add this XmlNamespace attribute to the root element of the markup file where it is 
    ///  to be used:
    ///
    ///  xmlns:MyNamespace="clr-namespace:WpfCharts;assembly=WpfCharts"
    ///
    ///  You will also need to add a project reference from the project where the XAML file lives
    ///  to this project and Rebuild to avoid compilation errors:
    ///
    ///  Right click on the target project in the Solution Explorer and
    ///  "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    ///  Step 2)
    ///  Go ahead and use your control in the XAML file.
    ///
    ///  <MyNamespace:Controls />
    ///</summary>
    public class SpiderChart : Control
    {
        static SpiderChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (SpiderChart), new FrameworkPropertyMetadata(typeof (SpiderChart)));
        }

        #region Axis

        /// <summary>
        ///   Axis Dependency Property
        /// </summary>
        public static readonly DependencyProperty AxisProperty = DependencyProperty.Register("Axis", typeof (IEnumerable), typeof (SpiderChart), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnAxisChanged));

        /// <summary>
        ///   Gets or sets the Axis property. This dependency property 
        ///   indicates ....
        /// </summary>
        public IEnumerable Axis
        {
            get { return (IEnumerable) GetValue(AxisProperty); }
            set { SetValue(AxisProperty, value); }
        }

        /// <summary>
        ///   Handles changes to the Axis property.
        /// </summary>
        private static void OnAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (SpiderChart) d;
            var oldAxis = (IEnumerable) e.OldValue;
            var newAxis = target.Axis;
            target.OnAxisChanged(oldAxis, newAxis);
        }

        /// <summary>
        ///   Provides derived classes an opportunity to handle changes to the Axis property.
        /// </summary>
        protected virtual void OnAxisChanged(IEnumerable oldAxis, IEnumerable newAxis)
        {
        }

        #endregion

        #region Lines

        /// <summary>
        /// Lines Dependency Property
        /// </summary>
        public static readonly DependencyProperty LinesProperty =
            DependencyProperty.Register("Lines", typeof(IEnumerable<ChartLine>), typeof(SpiderChart),
                new FrameworkPropertyMetadata(null, OnLinesChanged));

        /// <summary>
        /// Gets or sets the Lines property.
        /// </summary>
        public IEnumerable<ChartLine> Lines
        {
            get { return (IEnumerable<ChartLine>)GetValue(LinesProperty); }
            set { SetValue(LinesProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Lines property.
        /// </summary>
        private static void OnLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (SpiderChart)d;
            var oldLines = (IEnumerable<ChartLine>)e.OldValue;
            var newLines = target.Lines;
            target.OnLinesChanged(oldLines, newLines);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Lines property.
        /// </summary>
        protected virtual void OnLinesChanged(IEnumerable<ChartLine> oldLines, IEnumerable<ChartLine> newLines)
        {
        }

        #endregion

        #region Minimum

        /// <summary>
        ///   Minimum Dependency Property
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof (double), 
            typeof (SpiderChart), new FrameworkPropertyMetadata(0.4, OnMinimumChanged, CoerceMinimum));

        /// <summary>
        ///   Gets or sets the Minimum property. This dependency property 
        ///   indicates ....
        /// </summary>
        public double Minimum
        {
            get { return (double) GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        ///   Handles changes to the Minimum property.
        /// </summary>
        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (SpiderChart) d;
            var oldMinimum = (double) e.OldValue;
            var newMinimum = target.Minimum;
            target.OnMinimumChanged(oldMinimum, newMinimum);
        }

        /// <summary>
        ///   Provides derived classes an opportunity to handle changes to the Minimum property.
        /// </summary>
        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
        }

        /// <summary>
        ///   Coerces the Minimum value.
        /// </summary>
        private static object CoerceMinimum(DependencyObject d, object value)
        {
            var target = (SpiderChart) d;
            var desiredMinimum = (double) value;

            return Math.Min(desiredMinimum, target.Maximum);
        }

        #endregion

        #region Maximum

        /// <summary>
        /// Maximum Dependency Property
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(SpiderChart),
                new FrameworkPropertyMetadata((double)1,
                    FrameworkPropertyMetadataOptions.None, OnMaximumChanged));

        /// <summary>
        /// Gets or sets the Maximum property. This dependency property 
        /// indicates ....
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Maximum property.
        /// </summary>
        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (SpiderChart)d;
            var oldMaximum = (double)e.OldValue;
            var newMaximum = target.Maximum;
            target.OnMaximumChanged(oldMaximum, newMaximum);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Maximum property.
        /// </summary>
        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
        }

        #endregion

        #region Ticks

        /// <summary>
        /// Ticks Dependency Property
        /// </summary>
        public static readonly DependencyProperty TicksProperty =
            DependencyProperty.Register("Ticks", typeof(int), typeof(SpiderChart),
                new FrameworkPropertyMetadata(10, OnTicksChanged));

        /// <summary>
        /// Gets or sets the Ticks property. This dependency property 
        /// indicates ....
        /// </summary>
        public int Ticks
        {
            get { return (int)GetValue(TicksProperty); }
            set { SetValue(TicksProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Ticks property.
        /// </summary>
        private static void OnTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (SpiderChart)d;
            var oldTicks = (int)e.OldValue;
            var newTicks = target.Ticks;
            target.OnTicksChanged(oldTicks, newTicks);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Ticks property.
        /// </summary>
        protected virtual void OnTicksChanged(int oldTicks, int newTicks)
        {
        }

        #endregion

        #region Title

        /// <summary>
        /// Title Dependency Property
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SpiderChart),
                new FrameworkPropertyMetadata(string.Empty, OnTitleChanged));

        /// <summary>
        /// Gets or sets the Title property. 
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Title property.
        /// </summary>
        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (SpiderChart)d;
            var oldTitle = (string)e.OldValue;
            var newTitle = target.Title;
            target.OnTitleChanged(oldTitle, newTitle);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Title property.
        /// </summary>
        protected virtual void OnTitleChanged(string oldTitle, string newTitle)
        {
        }

        #endregion
        
    }
}