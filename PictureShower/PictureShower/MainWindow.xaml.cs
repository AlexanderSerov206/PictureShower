using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            SetMargin(images, 5, 5, 15, 5);

            MainGrid.UpdateLayout();

            PicturesPanel picturesPanel = new PicturesPanel();
            picturesPanel.Orientation = Orientation.Horizontal;
            picturesPanel.HorizontalAlignment = HorizontalAlignment.Center;
            picturesPanel.VerticalAlignment = VerticalAlignment.Center;

            picturesPanel.Children.Add(images[0]);
            picturesPanel.Children.Add(images[1]);

            PicturesPanel picturesPanel2 = new PicturesPanel();
            picturesPanel2.Children.Add(images[2]);
            picturesPanel2.Children.Add(images[3]);
            picturesPanel2.Orientation = Orientation.Vertical;

            picturesPanel.Children.Add(picturesPanel2);

            MainGrid.Children.Add(picturesPanel);

            UpdateLayout();
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
    public class PicturesPanel : StackPanel
    {
        protected override Size MeasureOverride(Size constraint)
        {
            constraint.Width = (double)Parent.GetValue(ActualWidthProperty);
            constraint.Height = (double)Parent.GetValue(ActualHeightProperty);

            if (Orientation == Orientation.Horizontal)
            {
                double totalWidth = GetChildrenWidth();

                foreach (UIElement child in InternalChildren)
                {
                    double percentWidth = (double)child.GetValue(ActualWidthProperty) / totalWidth;
                    double finalWidth = constraint.Width * percentWidth;
                    child.SetValue(WidthProperty, finalWidth);
                }
            }
            else if (Orientation == Orientation.Vertical)
            {
                double totalHeight = GetChildrenHeight();

                foreach (UIElement child in InternalChildren)
                {
                    double percentHeight = (double)child.GetValue(ActualHeightProperty) / totalHeight;
                    double finalHeight = constraint.Height * percentHeight;
                    child.SetValue(HeightProperty, finalHeight);
                }
            }

            return base.MeasureOverride(constraint);
        }
        private double GetChildrenWidth()
        {
            double totalWidth = 0;

            foreach (UIElement child in InternalChildren)
            {
                totalWidth += (double)child.GetValue(ActualWidthProperty);
            }

            return totalWidth;
        }

        private double GetChildrenHeight()
        {
            double totalHeight = 0;

            foreach (UIElement child in InternalChildren)
            {
                totalHeight += (double)child.GetValue(ActualHeightProperty);
            }

            return totalHeight;
        }
    }
}
