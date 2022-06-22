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
        }

        /// <summary>
        /// Запускает формирование коллажа.
        /// </summary>
        private void StartCollage(List<Image> images)
        {
            CollagePanel panel1 = new CollagePanel(Orientation.Horizontal);
            CollagePanel panel2 = new CollagePanel(Orientation.Vertical);
            CollagePanel panel3 = new CollagePanel(Orientation.Horizontal);
            CollagePanel panel4 = new CollagePanel(Orientation.Vertical);
            CollageGrid.Children.Add(panel1);

            panel1.AddChild(GetRandomImageFromList(images));
            panel1.AddChild(panel2);
            panel2.AddChild(panel3);
            panel3.AddChild(GetRandomImageFromList(images));
            panel3.AddChild(panel4);
            panel4.AddChild(GetRandomImageFromList(images));
            panel4.AddChild(GetRandomImageFromList(images));
            panel2.AddChild(GetRandomImageFromList(images));
            panel1.AddChild(GetRandomImageFromList(images));
            panel1.AddChild(GetRandomImageFromList(images));

            CollageGrid.UpdateLayout();
        }

        /// <summary>
        /// Возвращает случайный элемент из коллекции, реализующей IList<T>. Элемент при этом удаляется из коллекции.
        /// </summary>
        private T GetRandomImageFromList<T>(IList<T> elements)
        {
            Random random = new Random();

            int randomIndex = random.Next(0, elements.Count() - 1);
            T element = elements[randomIndex];

            elements.Remove(element);

            return element;
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
        /// Устанавливает отступ со всех сторон для всех элементов в коллекции.
        /// </summary>
        /// <param name="elements">Коллекция элементов</param>
        /// <param name="margin">Отступ</param>
        private static void SetMargin(IEnumerable<UIElement> elements, int margin)
        {
            SetMargin(elements, margin, margin, margin, margin);
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

        /// <summary>
        /// Удаляет все элементы панели, если она не пуста.
        /// </summary>
        private void ClearPanel(Panel panel)
        {
            if (panel.Children.Count > 0)
            {
                panel.Children.Clear();
            }
        }

        /// <summary>
        /// Возвращает значение Margin из текстбокса на форме. Если значение пустое или некорректное, возвращает 5.
        /// </summary>
        /// <returns></returns>
        private int GetMargin()
        {
            if (int.TryParse(MarginTextBox.Text, out int margin))
            {
                return margin;
            }
            else
            {
                return 5;
            }
        }

        /// <summary>
        /// Устанавливает размеры панели исходя из данных, введённых в поле WidthTextBox.
        /// </summary>
        private void SetPanelSize(Panel panel)
        {
            if (int.TryParse(WidthTextBox.Text, out int width))
            {
                panel.Width = width;
                panel.Height = Width / 2;
            }
            else
            {
                panel.Width = 1000;
                panel.Height = 500;
            }
        }

        /// <summary>
        /// Клик на кнопку "Запустить коллаж"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            ClearPanel(CollageGrid);
            SetPanelSize(CollageGrid);
            int margin = GetMargin();
            List<Image> images = GetImages();
            SetMargin(images, margin);

            StartCollage(images);
        }
    }
}
