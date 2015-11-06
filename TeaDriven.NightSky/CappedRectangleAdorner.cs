using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TeaDriven.NightSky
{
    public class CappedRectangleAdorner : DragAndDropAdornerBase
    {
        // Be sure to call the base class constructor.
        public CappedRectangleAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            DoubleAnimation animation = new DoubleAnimation(0.3, 1,
                new Duration(TimeSpan.FromSeconds(1)))
            {
                AutoReverse = true,
                RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
            };
            adornedElementBrush.BeginAnimation(System.Windows.Media.Brush.OpacityProperty, animation);
        }

        // A common way to implement an adorner's rendering behavior is to override the OnRender
        // method, which is called by the layout subsystem as part of a rendering pass.
        protected override void OnRender(DrawingContext drawingContext)
        {
            // Get a rectangle that represents the desired size of the rendered element
            // after the rendering pass.  This will be used to draw at the corners of the
            // adorned element.
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green) { Opacity = 0.2 };
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            double renderRadius = 5.0;

            // Just draw a circle at each corner.
            drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
        }
    }
}