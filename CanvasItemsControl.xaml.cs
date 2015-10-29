using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace TeaDriven.StarrySky
{
    /// <summary>
    /// Interaction logic for CanvasItemsControl.xaml
    /// </summary>
    public partial class CanvasItemsControl
    {
        public CanvasItemsControl()
        {
            InitializeComponent();
        }

        private void ItemsPanel_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mouseDown)
            {
                DragFinished(false);
                e.Handled = true;
            }
        }

        private void DragFinished(bool cancelled)
        {
            System.Windows.Input.Mouse.Capture(null);
            if (mouseDragging)
            {
                AdornerLayer.GetAdornerLayer(_adornerElement.AdornedElement).Remove(_adornerElement);

                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop + _adornerElement.TopOffset);
                    Canvas.SetLeft(_originalElement, _originalLeft + _adornerElement.LeftOffset);
                }
                _adornerElement = null;
            }
            mouseDragging = false;
            mouseDown = false;
        }

        private void ItemsPanel_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mouseDown)
            {
                if ((mouseDragging == false) &&
                    ((Math.Abs(e.GetPosition(ItemsPanel).X - _startPoint.X) >
                      SystemParameters.MinimumHorizontalDragDistance) ||
                     (Math.Abs(e.GetPosition(ItemsPanel).Y - _startPoint.Y) >
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

        private void DragStarted()
        {
            mouseDragging = true;

            Vector offset = VisualTreeHelper.GetOffset(_originalElement);

            _originalLeft = offset.X;
            _originalTop = offset.Y;

            _adornerElement = new SimpleCircleAdorner(_originalElement);
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
            layer.Add(_adornerElement);
        }

        private void DragMoved()
        {
            Point CurrentPosition = System.Windows.Input.Mouse.GetPosition(ItemsPanel);

            _adornerElement.LeftOffset = CurrentPosition.X - _startPoint.X;
            _adornerElement.TopOffset = CurrentPosition.Y - _startPoint.Y;
        }

        private Canvas ItemsPanel
        {
            get { return FindItemsPanel(this); }
        }

        private void MyCanvas_PreviewMouseLeftButtonDown(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.Source != ItemsPanel)
            {
                mouseDown = true;
                _startPoint = e.GetPosition(ItemsPanel);
                _originalElement = VisualTreeHelper.GetParent(e.Source as UIElement) as UIElement;
                ItemsPanel.CaptureMouse();
                e.Handled = true;
            }
        }

        private Point _startPoint;
        private double _originalLeft;
        private double _originalTop;
        private bool mouseDown;
        private bool mouseDragging;
        private UIElement _originalElement;
        private SimpleCircleAdorner _adornerElement;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Argh");
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
    }
}