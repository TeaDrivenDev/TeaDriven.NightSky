using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TeaDriven.StarrySky
{
    public class SimpleCircleAdorner : DragAndDropAdornerBase
    {
        // Be sure to call the base class constructor.
        public SimpleCircleAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            DoubleAnimation animation = new DoubleAnimation(0.3, 1,
                new Duration(TimeSpan.FromSeconds(1)))
            {
                AutoReverse = true,
                RepeatBehavior = System.Windows.Media.Animation.RepeatBehavior.Forever
            };
            adornedElementBrush.BeginAnimation(System.Windows.Media.Brush.OpacityProperty, animation);

            child.Fill = adornedElementBrush;
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

    public abstract class DragAndDropAdornerBase : Adorner
    {
        protected readonly VisualBrush adornedElementBrush;

        // Be sure to call the base class constructor.
        protected DragAndDropAdornerBase(UIElement adornedElement)
            : base(adornedElement)
        {
            adornedElementBrush = new VisualBrush(adornedElement);
            child = new Rectangle
            {
                Width = adornedElement.RenderSize.Width,
                Height = adornedElement.RenderSize.Height
            };
        }

        protected override sealed Size MeasureOverride(Size constraint)
        {
            child.Measure(constraint);
            return child.DesiredSize;
        }

        protected override sealed Size ArrangeOverride(Size finalSize)
        {
            child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override sealed Visual GetVisualChild(int index)
        {
            return child;
        }

        protected override sealed int VisualChildrenCount => 1;

        public double LeftOffset
        {
            get
            {
                return _leftOffset;
            }
            set
            {
                _leftOffset = value;
                UpdatePosition();
            }
        }

        public double TopOffset
        {
            get
            {
                return _topOffset;
            }
            set
            {
                _topOffset = value;
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            AdornerLayer adornerLayer = this.Parent as AdornerLayer;
            adornerLayer?.Update(AdornedElement);
        }

        public override sealed GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
            return result;
        }

        protected Rectangle child = null;
        private double _leftOffset = 0;
        private double _topOffset = 0;
    }
}