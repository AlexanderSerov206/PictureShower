using System;
using System.Windows;
using System.Windows.Controls;

namespace PictureShower
{
    /// <summary>
    /// Панель, позволяющая отображать коллажи.
    /// </summary>
    public class CollagePanel : Grid
    {
        public Orientation Orientation { get; private set; }
        public CollagePanel(Orientation orientation)
        {
            Orientation = orientation;
        }

        /// <summary>
        /// Корректно добавляет дочерние элементы в коллекцию. 
        /// т.к. панель унаследована от Grid, требуется создание RowDefinition и ColumnDefinition в зависимости от ориентации панели.
        /// </summary>
        /// <param name="element"></param>
        public void AddChild(FrameworkElement element)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    ColumnDefinition column = new ColumnDefinition();
                    column.Width = new GridLength(1, GridUnitType.Auto);
                    ColumnDefinitions.Add(column);
                    Children.Add(element);
                    SetColumn(element, Children.IndexOf(element));
                    break;

                case Orientation.Vertical:
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(1, GridUnitType.Auto);
                    RowDefinitions.Add(row);
                    Children.Add(element);
                    SetRow(element, Children.IndexOf(element));
                    break;
            }
        }

        /// <summary>
        /// Перегрузка метода замера размеров панели.
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size finalSize = new Size();
            Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            Size tempSize = constraint;
            string name = this.Name;

            /* Проходим по всем дочерним элементам и производим замер их размеров без ограничений.
             * Замеряем самый маленький элемент по ширине или высоте в зависимости от ориентации панели. */
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

            /* Проходим по всем дочерним элементам и ограничиваем размер самым маленьким элементом в коллекции по ширине или высоте
             * в зависимости от ориентации панели. Расчитываем ширину или высоту всех дочерних элементов. */
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

            /* Рассчитываем пропорциональное изменение размера панели для того, чтобы уместить её в заданные размеры */
            double proportion;
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    proportion = constraint.Width / finalSize.Width;
                    finalSize.Height *= proportion;
                    finalSize.Width *= proportion;
                    break;
                case Orientation.Vertical:
                    proportion = finalSize.Width / finalSize.Height;
                    finalSize.Width = constraint.Height * proportion;
                    finalSize.Height = finalSize.Width / proportion;
                    break;
            }

            return base.MeasureOverride(finalSize);
        }

        /// <summary>
        /// Перегрузка метода установки размеров панели.
        /// </summary>
        /// <param name="arrangeSize"></param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElement parent = (UIElement)Parent;
            Size parentSize = new Size((double)parent.GetValue(WidthProperty), (double)parent.GetValue(HeightProperty));

            // Изменение размеров панели с учётом пропорций в зависимости от размеров родительского элемента.
            double proportion;
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    proportion = parentSize.Width / arrangeSize.Width;
                    arrangeSize.Height *= proportion;
                    arrangeSize.Width = parentSize.Width;
                    Height = arrangeSize.Height;
                    Width = arrangeSize.Width;
                    break;

                case Orientation.Vertical:
                    proportion = parentSize.Height / arrangeSize.Height;
                    arrangeSize.Width *= proportion;
                    arrangeSize.Height = parentSize.Height;
                    Height = arrangeSize.Height;
                    Width = arrangeSize.Width;
                    break;
            }

            return base.ArrangeOverride(arrangeSize);
        }
    }
}
