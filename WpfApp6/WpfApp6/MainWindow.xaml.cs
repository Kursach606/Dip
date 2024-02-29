using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using LiveCharts;
using LiveCharts.Wpf;

namespace WpfApp6
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateNewChart(); // Создаем первый график при запуске приложения
        }

        private void CreateNewChart()
        {
            var seriesCollection = new SeriesCollection();

            var cartesianChart = new CartesianChart
            {
                Series = seriesCollection,
                LegendLocation = LegendLocation.Right
            };

            var newTabItem = new TabItem
            {
                Header = "График", // Set the header without the number
                Content = cartesianChart
            };

            tabControl.Items.Add(newTabItem);
            tabControl.SelectedItem = newTabItem; // Select the new tab
        }

        private void btnAddLine_Click(object sender, RoutedEventArgs e)
        {
            var currentTabItem = tabControl.SelectedItem as TabItem;
            if (currentTabItem != null)
            {
                var currentChart = currentTabItem.Content as CartesianChart;
                if (currentChart != null)
                {
                    if (currentChart.Series == null)
                    {
                        currentChart.Series = new SeriesCollection();
                    }

                    var newLineSeries = new LineSeries
                    {
                        Title = "Новая линия",
                        Values = new ChartValues<double>()
                    };

                    currentChart.Series.Add(newLineSeries);
                    currentChart.Update(true, true); // Обновляем график
                }
                else
                {
                    MessageBox.Show("Выбранный график не является CartesianChart.");
                }
            }
        }

        private void btnAddPoint_Click(object sender, RoutedEventArgs e)
        {
            double y;
            if (double.TryParse(txtY.Text, out y))
            {
                var currentTabItem = tabControl.SelectedItem as TabItem;
                var currentChart = currentTabItem?.Content as CartesianChart;

                if (currentChart != null)
                {
                    if (currentChart.Series.Count > 0)
                    {
                        var selectedLineSeries = currentChart.Series[currentChart.Series.Count - 1] as LineSeries;
                        if (selectedLineSeries != null)
                        {
                            selectedLineSeries.Values.Add(y);
                            currentChart.Update(true, true); // Обновляем график
                        }
                        else
                        {
                            MessageBox.Show("Последний серийный элемент не является линией.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Нет серийных элементов для добавления точки.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Неверный формат входных данных.");
            }
        }

        private void btnNewChart_Click(object sender, RoutedEventArgs e)
        {
            CreateNewChart();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Button closeButton = (Button)sender;
            TabItem tabItem = (TabItem)closeButton.TemplatedParent;
            tabControl.Items.Remove(tabItem);
        }
        private void btnSaveChart_Click(object sender, RoutedEventArgs e)
        {
            var currentTabItem = tabControl.SelectedItem as TabItem;
            if (currentTabItem != null)
            {
                var currentChart = currentTabItem.Content as CartesianChart;
                if (currentChart != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "PNG Image (*.png)|*.png|JPEG Image (*.jpeg)|*.jpeg|BMP Image (*.bmp)|*.bmp|GIF Image (*.gif)|*.gif",
                        Title = "Сохранить график как изображение"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string filePath = saveFileDialog.FileName;
                        SaveChartAsImage(currentChart, filePath);
                    }
                }
            }
        }
        
        private void SaveChartAsImage(CartesianChart chart, string filePath)
        {
            var renderSize = new System.Windows.Size(chart.ActualWidth, chart.ActualHeight);
            chart.Measure(renderSize);
            chart.Arrange(new Rect(renderSize));

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderSize.Width, (int)renderSize.Height, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(chart);

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }
    }
}