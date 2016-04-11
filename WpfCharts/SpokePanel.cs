using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

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
            var incrementalAngularSpace = (360.0 / Children.Count) * (Math.PI / 180);
            //An approximate radii based on the avialable size , obviusly a better approach is needed here.
            const double d = 2.15;
            var minSide = Math.Min(RenderSize.Width, RenderSize.Height);
            var radiusX = minSide / d;
            var radiusY = minSide / d;

            foreach (UIElement elem in Children)
            {
                //Calculate the point on the circle for the element
                var childPoint = new Point(Math.Sin(angle) * radiusX, -Math.Cos(angle) * radiusY);

                //Offsetting the point to the Avalable rectangular area which is FinalSize.
                var actualChildPoint = new Point(finalSize.Width / 2 + childPoint.X - elem.DesiredSize.Width / 2, finalSize.Height / 2 + childPoint.Y - elem.DesiredSize.Height / 2);

                //Call Arrange method on the child element by giving the calculated point as the placementPoint.
                elem.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, elem.DesiredSize.Width, elem.DesiredSize.Height));

                //Calculate the new _angle for the next element
                angle += incrementalAngularSpace;
            }

            return finalSize;
        }
    }
}
