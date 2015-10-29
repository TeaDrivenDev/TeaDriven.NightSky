using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TeaDriven.StarrySky
{
    // https://msdn.microsoft.com/en-us/library/vstudio/bb295243%28v=vs.90%29.aspx
    public partial class CanvasItemsControl
    {
        #region Drag and drop state

        private Point startPoint;
        private Vector dragStartOffset;

        private bool mouseDown;
        private bool mouseDragging;

        private UIElement originalElement;
        private SimpleCircleAdorner adornerElement;

        #endregion Drag and drop state

        #region Constructors

        public CanvasItemsControl()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Drag and drop implementation

        private void ItemsPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source != ItemsPanel)
            {
                mouseDown = true;
                startPoint = GetRelativeEventPosition(e);
                originalElement = GetParent<ContentPresenter>(e.Source as UIElement, ItemsPanel) as UIElement;
                ItemsPanel.CaptureMouse();
                e.Handled = true;
            }
        }

        private void ItemsPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                var position = GetRelativeEventPosition(e);

                if ((!mouseDragging) &&
                    ((Math.Abs(position.X - startPoint.X) >
                      SystemParameters.MinimumHorizontalDragDistance) ||
                     (Math.Abs(position.Y - startPoint.Y) >
                      SystemParameters.MinimumVerticalDragDistance)))
                {
                    DragStarted();
                }

                if (mouseDragging)
                {
                    DragMoved();
                }
            }
        }

        private void ItemsPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseDown)
            {
                DragFinished(false);
                e.Handled = true;
            }
        }

        private void DragStarted()
        {
            mouseDragging = true;

            dragStartOffset = VisualTreeHelper.GetOffset(originalElement);

            adornerElement = new SimpleCircleAdorner(originalElement);
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(originalElement);
            layer.Add(adornerElement);
        }

        private void DragMoved()
        {
            Point CurrentPosition = Mouse.GetPosition(ItemsPanel);

            adornerElement.LeftOffset = CurrentPosition.X - startPoint.X;
            adornerElement.TopOffset = CurrentPosition.Y - startPoint.Y;
        }

        private void DragFinished(bool cancelled)
        {
            Mouse.Capture(null);
            if (mouseDragging)
            {
                AdornerLayer.GetAdornerLayer(adornerElement.AdornedElement).Remove(adornerElement);

                if (cancelled == false)
                {
                    Canvas.SetTop(originalElement, dragStartOffset.Y + adornerElement.TopOffset);
                    Canvas.SetLeft(originalElement, dragStartOffset.X + adornerElement.LeftOffset);
                }
                adornerElement = null;
            }
            mouseDragging = false;
            mouseDown = false;
        }

        #endregion Drag and drop implementation

        #region Auxiliary code

        private Canvas ItemsPanel => FindItemsPanel(this);

        private Point GetRelativeEventPosition(MouseEventArgs e)
        {
            return e.GetPosition(ItemsPanel);
        }

        // https://snipt.net/raw/9444c2cebe488c6f1bb5730caf6b5a1f/?nice
        public static Canvas FindItemsPanel(Visual visual)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual child = VisualTreeHelper.GetChild(visual, i) as Visual;

                if (child != null)
                {
                    if (child is object && VisualTreeHelper.GetParent(child) is ItemsPresenter)
                    {
                        object temp = child;
                        return (Canvas)temp;
                    }

                    object panel = FindItemsPanel(child);

                    if (panel != null)
                    {
                        object temp = panel;
                        return (Canvas)temp;
                    }
                }
            }
            return default(Canvas);
        }

        public static DependencyObject GetParent<TParent>(
            DependencyObject reference,
            DependencyObject superParent)
        {
            var parent = VisualTreeHelper.GetParent(reference);

            return ((null == parent) ||
                    ((parent is TParent) && (VisualTreeHelper.GetParent(parent) == superParent))
                ? parent
                : GetParent<TParent>(parent, superParent));
        }

        #endregion Auxiliary code
    }
}