using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TeaDriven.NightSky
{
    public abstract class DragAndDropAdornerBase : Adorner
    {
        protected readonly Brush adornedElementBrush;

        // Be sure to call the base class constructor.
        protected DragAndDropAdornerBase(UIElement adornedElement)
            : base(adornedElement)
        {
            adornedElementBrush = new VisualBrush(adornedElement);
            child = new Rectangle
            {
                Width = adornedElement.RenderSize.Width,
                Height = adornedElement.RenderSize.Height,
                Fill = CreateImageBrush(adornedElement as FrameworkElement)
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

        protected override sealed int VisualChildrenCount
        {
            get { return 1; }
        }

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
            if (adornerLayer != null)
            {
                adornerLayer.Update(AdornedElement);
            }
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

        // http://elegantcode.com/2010/12/09/wpf-copy-uielement-as-image-to-clipboard/
        public static Brush CreateImageBrush(FrameworkElement element)
        {
            double width = element.ActualWidth;
            double height = element.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);

            return new ImageBrush(bmpCopied);
        }
    }
}