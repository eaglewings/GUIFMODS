using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Controls
{
    /// <summary>
    ///
    ///     <MyNamespace:RadarChart/>
    ///
    /// </summary>
    public class RadarChart : Control
    {
        static RadarChart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RadarChart), new FrameworkPropertyMetadata(typeof(RadarChart)));
        }

        #region Axes

        /// <summary>
        ///   Axes Dependency Property
        /// </summary>
        public static readonly DependencyProperty AxesProperty =
            DependencyProperty.Register("Axes", typeof(IList<Axis>), typeof(RadarChart),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnAxesChanged));

        /// <summary>
        ///   Gets or sets the Axes property. This dependency property 
        ///   indicates ....
        /// </summary>
        public IList<Axis> Axes
        {
            get { return (IList<Axis>)GetValue(AxesProperty); }
            set { SetValue(AxesProperty, value); }
        }

        /// <summary>
        ///   Handles changes to the Axes property.
        /// </summary>
        private static void OnAxesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (RadarChart)d;
            var oldAxes = (IList<Axis>)e.OldValue;
            var newAxes = target.Axes;
            target.OnAxesChanged(oldAxes, newAxes);
        }

        /// <summary>
        ///   Provides derived classes an opportunity to handle changes to the Axes property.
        /// </summary>
        protected virtual void OnAxesChanged(IList<Axis> oldAxes, IList<Axis> newAxes)
        {
        }

        #endregion Axes

        #region Lines

        /// <summary>
        /// Lines Dependency Property
        /// </summary>
        public static readonly DependencyProperty LinesProperty =
            DependencyProperty.Register("Lines", typeof(IEnumerable<ChartLine>), typeof(RadarChart),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, OnLinesChanged));

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
            var target = (RadarChart)d;
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

        #region SelectedLine

        /// <summary>
        /// SelectedLine Dependency Property
        /// </summary>
        public static readonly DependencyProperty SelectedLineProperty =
            DependencyProperty.Register("SelectedLine", typeof(ChartLine), typeof(RadarChart),
                new FrameworkPropertyMetadata(null, OnSelectedLineChanged));

        /// <summary>
        /// Gets or sets the SelectedLine property.
        /// </summary>
        public ChartLine SelectedLine
        {
            get { return (ChartLine)GetValue(SelectedLineProperty); }
            set { SetValue(SelectedLineProperty, value); }
        }

        /// <summary>
        /// Handles changes to the SelectedLine property.
        /// </summary>
        private static void OnSelectedLineChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (RadarChart)d;
            var oldLine = (ChartLine)e.OldValue;
            var newLine = target.SelectedLine;
            target.OnSelectedLineChanged(oldLine, newLine);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the SelectedLine property.
        /// </summary>
        protected virtual void OnSelectedLineChanged(ChartLine oldLine, ChartLine newLine)
        {
        }

        #endregion
        ItemsControl axesPanel;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Control slider;
            
            axesPanel = (ItemsControl) GetTemplateChild("PART_SpokePanel");
           
        }

        private void Slider_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
                
            OnAxisClicked(new AxisClickedEventArgs(null));
        }


        /// <summary>
        /// Event raised whenever an axis is clicked
        /// </summary>
        public static readonly RoutedEvent AxisClickedEvent =
            EventManager.RegisterRoutedEvent("AxisClicked",
            RoutingStrategy.Bubble, typeof(AxisClickedEventHandler), typeof(RadarChart));

        /// <summary>
        /// Event raised whenever an axis is clicked
        /// </summary>
        public event AxisClickedEventHandler AxisClicked
        {
            add { AddHandler(AxisClickedEvent, value); }
            remove { RemoveHandler(AxisClickedEvent, value); }
        }

        //Raises the AxisClicked event
        public void OnAxisClicked(AxisClickedEventArgs e)
        {
            e.RoutedEvent = AxisClickedEvent;
            RaiseEvent(e);
        }

        private void Axis_DoubleClick(object sender, RoutedEventArgs e)
        {
            AxisClickedEventArgs args = new AxisClickedEventArgs(new Axis());
            OnAxisClicked(args);
        }
    }

    /// <summary>
    /// Delegate for the RangeSelectionChanged event
    /// </summary>
    /// <param name="sender">The object raising the event</param>
    /// <param name="e">The event arguments</param>
    public delegate void AxisClickedEventHandler(object sender, AxisClickedEventArgs e);

    /// <summary>
    /// Event arguments for the 
    /// </summary>
    public class AxisClickedEventArgs : RoutedEventArgs
    {
        private Axis axis;

        /// <summary>
        /// The new range start selected in the range slider
        /// </summary>
        public Axis Axis
        {
            get { return axis; }
            set { axis = value; }
        }

        /// <summary>
        /// sets the range start and range stop for the event args
        /// </summary>
        /// <param name="axis">The clicked axis</param>
        internal AxisClickedEventArgs(Axis axis)
        {
            this.axis = axis;
        }

    }
}
