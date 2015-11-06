using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using TeaDriven.NightSky;

namespace DragDropTest
{
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

        public Func<UIElement, DragAndDropAdornerBase> CreateAdorner
        {
            get { return control => new CappedRectangleAdorner(control); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Argh");
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