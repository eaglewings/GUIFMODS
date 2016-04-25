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
    }
}
