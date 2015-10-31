using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TeaDriven.StarrySky
{
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
                Height = adornedElement.RenderSize.Height,
                Fill = adornedElementBrush
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
                return leftOffset;
            }
            set
            {
                leftOffset = value;
                UpdatePosition();
            }
        }

        public double TopOffset
        {
            get
            {
                return topOffset;
            }
            set
            {
                topOffset = value;
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
            result.Children.Add(new TranslateTransform(leftOffset, topOffset));
            return result;
        }

        protected Rectangle child;
        private double leftOffset;
        private double topOffset;
    }
}