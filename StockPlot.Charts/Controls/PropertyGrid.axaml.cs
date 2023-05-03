using System.ComponentModel;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using StockPlot.Indicators;

namespace StockPlot.Charts.Controls
{
    public partial class PropertyGrid : UserControl
    {
        public Button OkButton;
        public PropertyGrid()
        {
            InitializeComponent();
            OkButton = this.Find<Button>("okButton");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static readonly StyledProperty<object> ItemProperty = AvaloniaProperty.Register<PropertyGrid, object>(nameof(Item), defaultValue: new object());

        public object Item
        {
            get { return GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); SetItem(value); }
        }


        public string GetCategory(PropertyInfo property)
        {
            CategoryAttribute[] attributes = (CategoryAttribute[])property.GetCustomAttributes(typeof(CategoryAttribute), false);

            if (attributes.Any())
                return attributes[0].Category;

            return "Informations";
        }

        public void SetItem(object item)
        {
            var stackProperties = this.FindControl<StackPanel>("StackProperties");
            stackProperties.Children.Clear();

            List<PropertyInfo> properties = item.GetType().GetProperties().Where(x=> x.GetCustomAttribute<IndicatorParameter>() != null).ToList();

            List<string> categories = properties.Select(prop => GetCategory(prop)).Distinct().ToList();

            foreach (var category in categories)
            {
                StackPanel panel = new StackPanel() { Margin = new Thickness(10, 5, 10, 2), Width = Math.Abs(Width - 20), HorizontalAlignment = HorizontalAlignment.Center };

                foreach (PropertyInfo property in properties.Where(prop => GetCategory(prop).Equals(category)))
                {
                    var gridItem = new DockPanel() { Margin = new Thickness(0, 1) };
                    var text = new TextBlock()
                    {
                        Text = property.Name,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(8, 4, 4, 4),
                        FontSize = 14,
                        Width = Math.Abs(Width / 2 - 40)

                    };

                    gridItem.Children.Add(text);
                    DockPanel.SetDock(text, Dock.Left);

                    AddProperty(gridItem, property);

                    panel.Children.Add(gridItem);
                }

                Expander groupBox = new Expander() { Header = category, Content = panel, Margin = new Thickness(0, 4), IsExpanded = true };
                stackProperties.Children.Add(groupBox);

            }
        }

        public void AddProperty(DockPanel gridItem, PropertyInfo property)
        {
            if (property.GetValue(Item) == null)
                return;

            if (property.GetValue(Item).GetType() == typeof(string))
            {
                var prop = new TextBox() { Width = Width / 2.5,
                    Text = property.GetValue(Item).ToString(),
                    Padding = new Thickness(6),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(3) };
                prop.GetObservable(TextBox.TextProperty).Subscribe(value => property.SetValue(Item, value));
                gridItem.Children.Add(prop);
                Grid.SetColumn(prop, 1);

            }

            else if (property.GetValue(Item).GetType() == typeof(int))
            {
                var prop = new NumericUpDown() { Width = Width / 2.5, 
                    Value = Decimal.Parse(property.GetValue(Item).ToString()),
                    Increment = 1, Padding = new Thickness(0, 0, 5, 0),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(3) };
                prop.GetObservable(NumericUpDown.ValueProperty).Subscribe(value => property.SetValue(Item, Int32.Parse(value.ToString())));
                gridItem.Children.Add(prop);
                Grid.SetColumn(prop, 1);
            }

            else if (property.GetValue(Item).GetType() == typeof(double))
            {
                var prop = new NumericUpDown() { Width = Width / 2.5,
                    Value = (decimal?)(double)property.GetValue(Item),
                    Increment = 0.001M, Padding = new Thickness(0, 0, 5, 0),
                    HorizontalAlignment = HorizontalAlignment.Right, 
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(3) };
                prop.GetObservable(NumericUpDown.ValueProperty).Subscribe(value => property.SetValue(Item, Double.Parse(value.ToString())));
                gridItem.Children.Add(prop);
                Grid.SetColumn(prop, 1);
            }

            else if (property.GetValue(Item).GetType() == typeof(bool))
            {
                var prop = new ToggleButton() { Margin = new Thickness(0, 0, 5, 0),
                    HorizontalContentAlignment = HorizontalAlignment.Right, 
                    HorizontalAlignment = HorizontalAlignment.Right, 
                    IsChecked = (bool)property.GetValue(Item) };
                prop.GetObservable(ToggleButton.IsCheckedProperty).Subscribe(value => property.SetValue(Item, (bool)value));
                gridItem.Children.Add(prop);
                Grid.SetColumn(prop, 1);
            }

            else if (property.GetValue(Item) is Enum)
            {
                var type = property.GetValue(Item).GetType();
                var names = Enum.GetNames(type);
                var prop = new ComboBox() { Width = Width / 2.5, Items = Enum.GetValues(type), SelectedItem = property.GetValue(Item), Height = 36, HorizontalContentAlignment = HorizontalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 2, 0, 2) };
                prop.GetObservable(ComboBox.SelectedItemProperty).Subscribe(value => property.SetValue(Item, value));
                gridItem.Children.Add(prop);
                Grid.SetColumn(prop, 1);
            }
            else if (property.GetValue(Item).GetType() == typeof(List<int>))
            {
                var scrollviewer = new ScrollViewer();
                var stack = new StackPanel() { Orientation = Orientation.Horizontal };
                foreach (var item in (List<int>)(property.GetValue(Item)))
                {
                    stack.Children.Add(new TextBlock() { Text = item.ToString() + ", ", VerticalAlignment = VerticalAlignment.Center });
                }

                var grid = new Grid() { Width = Width / 2.5, Height = 36, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 0, 10, 2) };
                scrollviewer.Content = stack;
                grid.Children.Add(scrollviewer);
                gridItem.Children.Add(grid);
                Grid.SetColumn(grid, 1);
            }

            else
            {
                var prop = new Button() { HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 6, 10, 2), Content = new TextBlock() { Text = "More Info" } };
                prop.Click += (object sender, Avalonia.Interactivity.RoutedEventArgs e) => {

                    var content = new Border()
                    {
                        //Background = new SolidColorBrush((Color)Application.Current.FindResource("SukiBackground")),
                        Width = 300,
                        Padding = new Thickness(5),
                        Child = new PropertyGrid() { Item = property.GetValue(Item), Width = 280, HorizontalAlignment = HorizontalAlignment.Center }
                    };



                    var window = new Window()
                    {
                        Height = 500,
                        Width = 300,
                        Content = content
                    };
                    window.ShowDialog((Window)this.VisualRoot);
                };

                gridItem.Children.Add(prop);
                Grid.SetColumn(prop, 1);
            }

        }


    }
}
