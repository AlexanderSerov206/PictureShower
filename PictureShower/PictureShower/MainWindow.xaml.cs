using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PictureShower
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<Image> images = GetImages();
            SetMargin(images, 0, 0, 0, 0);
            PicturesPanel panel1 = new PicturesPanel(Orientation.Horizontal, "Main Horizontal");
            PicturesPanel panel2 = new PicturesPanel(Orientation.Vertical, "Second Vertical");
            PicturesPanel panel3 = new PicturesPanel(Orientation.Horizontal, "Third Horizontal");
            MainGrid.Children.Add(panel1);

            panel1.AddChild(images[0]);
            panel1.AddChild(panel2);
            panel2.AddChild(panel3);
            panel3.AddChild(images[4]);
            panel3.AddChild(images[5]);
            panel2.AddChild(images[1]);
            panel1.AddChild(images[3]);
            panel1.AddChild(images[6]);

            MainGrid.UpdateLayout();
        }

        /// <summary>
        /// Устанавливает отступ для всех элементов в коллекции.
        /// </summary>
        /// <param name="elements">Коллекция элементов</param>
        /// <param name="left">Отступ слева</param>
        /// <param name="top">Отступ сверху</param>
        /// <param name="right">Отступ справа</param>
        /// <param name="bottom">Отступ снизу</param>
        private static void SetMargin(IEnumerable<UIElement> elements, double bottom, double left, double right, double top)
        {
            Thickness margin = new Thickness();

            margin.Bottom = bottom;
            margin.Left = left;
            margin.Right = right;
            margin.Top = top;

            foreach (UIElement element in elements)
            {
                element.SetValue(MarginProperty, margin);
            }
        }

        /// <summary>
        /// Получает список изображений, которые находятся в папке Images, расположенной в каталоге запуска программы.
        /// </summary>
        /// <returns>Список изображений</returns>
        private static List<Image> GetImages()
        {
            FileInfo[] files = new FileInfo("Images/").Directory.GetFiles();
            List<Image> images = new List<Image>();

            foreach (FileInfo file in files)
            {
                if (file.Name.ToUpper().EndsWith(".JPG"))
                {
                    Image image = new Image();

                    BitmapImage bitmap = new BitmapImage(new Uri($"Images/{file.Name}", UriKind.Relative));

                    image.Source = bitmap;

                    images.Add(image);
                }
            }

            return images;
        }
    }
    public class PicturesPanel : Grid
    {
        public Orientation Orientation { get; private set; }
        public string Name { get; set; }
        public PicturesPanel(Orientation orientation, string name)
        {
            Name = name;
            SetOrientation(orientation);

            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
        }
        private void SetOrientation(Orientation orientation)
        {
            Orientation = orientation;

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    RowDefinitions.Add(new RowDefinition());
                    break;
                case Orientation.Vertical:
                    ColumnDefinitions.Add(new ColumnDefinition());
                    break;
                default:
                    break;
            }
        }
        public void AddChild(FrameworkElement element)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = new GridLength(1, GridUnitType.Auto);
                    ColumnDefinitions.Add(column);
                    Children.Add(element);
                    //SetDock(element, Dock.Left);
                    SetColumn(element, Children.IndexOf(element));
                    break;

                case Orientation.Vertical:
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(1, GridUnitType.Auto);
                    RowDefinitions.Add(row);
                    Children.Add(element);
                    //SetDock(element, Dock.Top);
                    SetRow(element, Children.IndexOf(element));
                    break;

                default:
                    break;
            }
        }
        protected override Size MeasureOverride(Size constraint)
        {
            if (double.IsInfinity(constraint.Width) && !double.IsInfinity(constraint.Height))
            {
                constraint.Width = DesiredSize.Width;
            }
            else if (double.IsInfinity(constraint.Height) && !double.IsInfinity(constraint.Width))
            {
                constraint.Height = DesiredSize.Height;
            }

            Size finalSize = new Size();
            Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            Size tempSize = constraint;
            string name = this.Name;

            foreach (UIElement child in InternalChildren)
            {
                if (child.DesiredSize.Equals(new Size(0, 0)))
                {
                    child.Measure(infiniteSize);

                    switch (Orientation)
                    {
                        case Orientation.Horizontal:
                            tempSize.Height = Math.Min(child.DesiredSize.Height, tempSize.Height);
                            break;
                        case Orientation.Vertical:
                            tempSize.Width = Math.Min(child.DesiredSize.Width, tempSize.Width);
                            break;
                    }
                }
            }

            foreach (UIElement child in InternalChildren)
            {
                child.Measure(tempSize);

                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        finalSize.Height = Math.Max(finalSize.Height, child.DesiredSize.Height);
                        finalSize.Width += child.DesiredSize.Width;
                        break;
                    case Orientation.Vertical:
                        finalSize.Width = Math.Max(finalSize.Width, child.DesiredSize.Width);
                        finalSize.Height += child.DesiredSize.Height;
                        break;
                }
            }

            if (!double.IsInfinity(constraint.Height) && !double.IsInfinity(constraint.Width))
            {
                double coefficient;
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        coefficient = constraint.Width / finalSize.Width;
                        finalSize.Height = finalSize.Height * coefficient;
                        finalSize.Width = finalSize.Width * coefficient;
                        break;
                    case Orientation.Vertical:
                        coefficient = finalSize.Width / finalSize.Height;
                        finalSize.Width = constraint.Height * coefficient;
                        finalSize.Height = finalSize.Width / coefficient;
                        break;
                }
            }

            //size.Width = double.IsPositiveInfinity(constraint.Width) ? size.Width : constraint.Width;
            //size.Height = double.IsPositiveInfinity(constraint.Height) ? size.Height : constraint.Height;

            if (finalSize.Width > DesiredSize.Width && finalSize.Height > DesiredSize.Height && !DesiredSize.Equals(new Size(0, 0)))
            {
                return base.MeasureOverride(DesiredSize);
            }
            else
            {
                return base.MeasureOverride(finalSize);
            }
        }
    }
}
