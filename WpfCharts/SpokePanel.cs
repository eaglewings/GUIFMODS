using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Controls
{
    public class SpokePanel : Panel
    {
        #region Axis

        /// <summary>
        /// Lines Dependency Property (acts as a kind of ItemsSource, triggering a redraw when the collection is changed)
        /// </summary>
        public static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register("Axis", typeof(Axis), typeof(SpokePanel), new FrameworkPropertyMetadata(null, OnAxisChanged));

        /// <summary>
        /// Gets or sets the Lines property.
        /// </summary>
        public Axis Axis
        {
            get { return (Axis)GetValue(AxisProperty); }
            set { SetValue(AxisProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Lines property.
        /// </summary>
        private static void OnAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (SpokePanel)d;
            var oldAxis = (Axis)e.OldValue;
            var newAxes = target.Axis;

            
        }

        private void AxesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvalidateVisual();
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Lines property.
        /// </summary>
        protected virtual void OnAxisChanged(Axis oldAxis, Axis newAxis)
        {
        }

        #endregion
        protected override Size MeasureOverride(Size availableSize)
        {
            var minSide = Math.Min(availableSize.Width, availableSize.Height);
            foreach (UIElement elem in Children)
            {
                //Give Infinite size as the avaiable size for all the children
                elem.Measure(new Size(minSide/2, double.PositiveInfinity));
            }

            return base.MeasureOverride(availableSize);
        }
        /// <summary>
        ///   Arrange all children based on the geometric equations for the circle.
        /// </summary>
        /// <param name="finalSize"> </param>
        /// <returns> </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;

            double angle = 0;

            //Degrees converted to Radian by multiplying with PI/180
            var incrementalAngularSpace = (360.0 / Children.Count);
            var minSide = Math.Min(finalSize.Width, finalSize.Height);

            foreach (UIElement elem in Children)
            {

                //Offsetting the point to the Avalable rectangular area which is FinalSize.
                var actualChildPoint = new Point(minSide / 2, minSide / 2 - elem.DesiredSize.Height / 2);

                //Call Arrange method on the child element by giving the calculated point as the placementPoint.
                elem.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, minSide / 2, elem.DesiredSize.Height));

                elem.RenderTransformOrigin = new Point(0,0.5);
                elem.RenderTransform = new RotateTransform(angle - 90);

                //Calculate the new _angle for the next element
                angle += incrementalAngularSpace;
            }

            return finalSize;
        }
    }
}
