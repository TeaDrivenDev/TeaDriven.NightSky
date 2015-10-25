// This sample shows how to drag and drop objects on the screen.
// To visually indicate that the object is being dragged, a simple adorner
// is applied to an object as you drag it.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace SDKSample
{
    #region Namespaces.

    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Threading;

    #endregion Namespaces.

    // https://msdn.microsoft.com/en-us/library/vstudio/bb295243%28v=vs.90%29.aspx
    public partial class AdornerDragDropExample
    {
        private ObservableCollection<Thing> mainItems;

        public ObservableCollection<Thing> MainItems
        {
            get { return mainItems; }
            set
            {
                mainItems = value;
                RaisePropertyChanged("MainItems");
            }
        }

        public void OnPageLoad(Object sender, RoutedEventArgs e)
        {
            DataContext = this;

            var random = new Random();

            var maxLeft = this.Width - 100;
            var maxTop = this.Height - 100;

            MainItems =
                new ObservableCollection<Thing>(
                    new[] { "Regulus", "Dubhe", "Denebola", "Gienah", "Acrux" }
                    .Select(
                        name =>
                            new Thing(name, random.Next((int)maxLeft), random.Next((int)maxTop))));
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged(string propName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        private void window1_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape && _isDragging)
            {
                DragFinished(true);
            }
        }

        private void MyCanvas_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_isDown)
            {
                DragFinished(false);
                e.Handled = true;
            }
        }

        private void DragFinished(bool cancelled)
        {
            System.Windows.Input.Mouse.Capture(null);
            if (_isDragging)
            {
                AdornerLayer.GetAdornerLayer(_adornerElement.AdornedElement).Remove(_adornerElement);

                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop + _adornerElement.TopOffset);
                    Canvas.SetLeft(_originalElement, _originalLeft + _adornerElement.LeftOffset);
                }
                _adornerElement = null;
            }
            _isDragging = false;
            _isDown = false;
        }

        private void MyCanvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isDown)
            {
                if ((_isDragging == false) &&
                    ((Math.Abs(e.GetPosition(ItemsPanel).X - _startPoint.X) >
                      SystemParameters.MinimumHorizontalDragDistance) ||
                     (Math.Abs(e.GetPosition(ItemsPanel).Y - _startPoint.Y) >
                      SystemParameters.MinimumVerticalDragDistance)))
                {
                    DragStarted();
                }
                if (_isDragging)
                {
                    DragMoved();
                }
            }
        }

        private void DragStarted()
        {
            _isDragging = true;

            Vector offset =
                VisualTreeHelper.GetOffset(_originalElement);

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
            get { return FindItemsPanel(ItemsControl); }
        }

        private void MyCanvas_PreviewMouseLeftButtonDown(
            object sender,
            System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.Source != ItemsPanel)
            {
                _isDown = true;
                _startPoint = e.GetPosition(ItemsPanel);
                _originalElement = VisualTreeHelper.GetParent(e.Source as UIElement) as UIElement;
                ItemsPanel.CaptureMouse();
                e.Handled = true;
            }
        }

        private Point _startPoint;
        private double _originalLeft;
        private double _originalTop;
        private bool _isDown;
        private bool _isDragging;
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
                        return (Canvas)temp; // return the panel up the call stack
                    }
                }
            }
            return default(Canvas);
        }
    }

    public class Thing
    {
        public string Name { get; private set; }
        public double Left { get; set; }
        public double Top { get; set; }

        public Thing(string name, double top, double left)
        {
            Name = name;
            Left = left;
            Top = top;
        }
    }
}