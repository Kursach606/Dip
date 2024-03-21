using System;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Diagnostics; // For SaveFileDialog


namespace WpfApp7
{
    public partial class MainWindow : Window
    {
        private bool isDarkTheme = false;
        private LineSeries selectedLineSeries;
        public ObservableCollection<LineSeries> lineSeriesCollection { get; set; } = new ObservableCollection<LineSeries>();
     
        public MainWindow()
        {
            InitializeComponent();
            ApplyTheme();
            cmbLines.DataContext = this;
          
        }

        private void btnDeleteLine_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную линию из ComboBox
            var selectedLine = cmbLines.SelectedItem as LineSeries;
            if (selectedLine != null)
            {
                // Удаляем линию из коллекции
                lineSeriesCollection.Remove(selectedLine);

                // Удаляем линию из графика
                var currentTabItem = tabControl.SelectedItem as TabItem;
                if (currentTabItem != null)
                {
                    var currentChart = currentTabItem.Content as CartesianChart;
                    if (currentChart != null)
                    {
                        currentChart.Series.Remove(selectedLine);
                        currentChart.Update(true, true);
                    }
                }

                // Обновляем ComboBox
                cmbLines.Items.Refresh();
            }
        }

      
        //импорт
        private void btnImportChart_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.png;*.jpeg;*.jpg;*.bmp)|*.png;*.jpeg;*.jpg;*.bmp";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                string tabName = Path.GetFileNameWithoutExtension(fileName);

                // Создаем новый TabItem
                TabItem newTab = new TabItem();
                newTab.Header = tabName;

                // Создаем Image для отображения графика
                Image chartImage = new Image();
                BitmapImage bitmapImage = new BitmapImage(new Uri(fileName, UriKind.Absolute));
                chartImage.Source = bitmapImage;

                // Добавляем Image в TabItem
                newTab.Content = chartImage;

                // Добавляем TabItem в TabControl
                tabControl.Items.Add(newTab);

                // Выбираем новую вкладку
                newTab.IsSelected = true;
            }
        }
        private void OpenCalculatorButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Путь к калькулятору в Windows
                string calculatorPath = "calc.exe";

                // Путь к калькулятору в Linux (gnome-calculator)
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    calculatorPath = "gnome-calculator";
                }

                // Путь к калькулятору в macOS (Calculator.app)
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    calculatorPath = "open -a Calculator";
                }

                // Запуск калькулятора
                Process.Start(calculatorPath);
            }
            catch (Exception ex)
            {
                // Обработка ошибок, если калькулятор не может быть запущен
                MessageBox.Show("Ошибка при открытии калькулятора: " + ex.Message);
            }
        }
        private void ApplyTheme()
        {
            this.Style = (Style)FindResource(isDarkTheme ? "DarkTheme" : "LightTheme");
        }
        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            isDarkTheme = !isDarkTheme;
            ApplyTheme();
        }
        private void btnAddLine_Click(object sender, RoutedEventArgs e)
        {
            var lineName = txtLineName.Text;
            if (string.IsNullOrWhiteSpace(lineName))
            {
                MessageBox.Show("Введите название линии.");
                return;
            }

            var currentTabItem = tabControl.SelectedItem as TabItem;
            if (currentTabItem != null)
            {
                var currentChart = currentTabItem.Content as CartesianChart;
                if (currentChart != null)
                {
                    // Инициализируем Series, если она еще не инициализирована
                    if (currentChart.Series == null)
                    {
                        currentChart.Series = new SeriesCollection();
                    }

                    var newLineSeries = new LineSeries
                    {
                        Title = lineName,
                        Values = new ChartValues<double>(),
                        DataLabels = true
                    };

                    // Добавляем LineSeries в коллекцию и в ComboBox
                    lineSeriesCollection.Add(newLineSeries);
                    cmbLines.Items.Refresh(); // Обновляем ComboBox

                    // Добавляем LineSeries в коллекцию Series графика
                    currentChart.Series.Add(newLineSeries);

                    // Выбираем новую линию в ComboBox
                    cmbLines.SelectedItem = newLineSeries;

                    // Обновляем график
                    currentChart.Update(true, true);
                }
                else
                {
                    MessageBox.Show("Выбранный график не является CartesianChart.");
                }
            }
        }
        private void CartesianChart_DataClick(object sender, ChartPoint chartPoint)
        {
            var currentChart = sender as CartesianChart;
            if (currentChart != null && chartPoint.SeriesView != null && chartPoint.SeriesView.Values != null)
            {
                var values = chartPoint.SeriesView.Values;
                var index = (int)chartPoint.X;
                if (index >= 0 && index < values.Count)
                {
                    // Показываем диалог для изменения значения точки
                    double newValue;
                    if (double.TryParse(txtY.Text, out newValue))
                    {
                        values[index] = newValue;
                        currentChart.Update(true, true); // Обновляем график
                    }
                    else
                    {
                        MessageBox.Show("Неверный формат входных данных.");
                    }
                }
            }
        }
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную линию
            var selectedLine = selectedLineSeries;

            // Получаем выбранную точку
            var selectedPointIndex = cmbPoints.SelectedIndex;

            // Обновляем значение выбранной точки
            if (selectedLine != null && selectedPointIndex >= 0 && selectedPointIndex < selectedLine.Values.Count)
            {
                double newValue;
                if (double.TryParse(txtPointValueX.Text, out newValue))
                {
                    selectedLine.Values[selectedPointIndex] = newValue;
                    // Обновляем график
                    var currentChart = GetCurrentChart();
                    currentChart.Update(true, true);
                }
                else
                {
                    MessageBox.Show("Неверный формат входных данных.");
                }
            }
            else
            {
                MessageBox.Show("Не выбрана линия или точка для обновления.");
            }
        }
        private void cmbLines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedLineSeries = cmbLines.SelectedItem as LineSeries;
            UpdatePointsComboBox();
        }

        private void UpdatePointsComboBox()
        {
            cmbPoints.Items.Clear();
            if (selectedLineSeries != null)
            {
                for (int i = 0; i < selectedLineSeries.Values.Count; i++)
                {
                    cmbPoints.Items.Add($"Точка {i + 1}");
                }
            }
        }
        private void btnAddPoint_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную линию
            var selectedLine = selectedLineSeries;

            // Проверяем, что выбранная линия не null
            if (selectedLine != null)
            {
                // Получаем значения для X и Y
                double newValueX, newValueY;
                if (double.TryParse(txtX.Text, out newValueX) && double.TryParse(txtY.Text, out newValueY))
                {
                    // Добавляем точку в выбранную линию
                    selectedLine.Values.Add(newValueX);
                    //selectedLine.Values.Add(newValueY);

                    // Обновляем ComboBox с точками
                    UpdatePointsComboBox();

                    // Обновляем график
                    var currentChart = GetCurrentChart();
                    currentChart.Update(true, true);
                }
                else
                {
                    MessageBox.Show("Неверный формат входных данных.");
                }
            }
            else
            {
                MessageBox.Show("Не выбрана линия для добавления точки.");
            }
        }
        private CartesianChart GetCurrentChart()
        {
            var currentTabItem = tabControl.SelectedItem as TabItem;
            if (currentTabItem != null)
            {
                return currentTabItem.Content as CartesianChart;
            }
            return null;
        }
        private void btnNewChart_Click(object sender, RoutedEventArgs e)
        {
            // Создаем новую коллекцию серий
            var seriesCollection = new SeriesCollection();

            // Создаем новый график с пустой коллекцией серий
            var cartesianChart = new CartesianChart
            {
                Series = seriesCollection,
                LegendLocation = LegendLocation.Right
            };

            // Создаем новую вкладку
            var newTabItem = new TabItem
            {
                Header = "График ",
                Content = cartesianChart
            };

            // Находим индекс выделенной вкладки
            int selectedIndex = tabControl.SelectedIndex;

            // Если выделенная вкладка не последняя, вставляем новую после нее
            if (selectedIndex < tabControl.Items.Count - 1)
            {
                tabControl.Items.Insert(selectedIndex + 1, newTabItem);
            }
            else
            {
                // Если выделенная вкладка последняя, добавляем новую в конец
                tabControl.Items.Add(newTabItem);
            }

            // Выбираем новую вкладку
            tabControl.SelectedIndex = selectedIndex + 1;
        }

        private void btnRemoveChart_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl.SelectedIndex > 0 && tabControl.Items.Count > 2) // Проверяем, что выбранная вкладка не является вкладкой "+" или "График 1" и что вкладок больше двух
            {
                tabControl.Items.RemoveAt(tabControl.SelectedIndex);
                if (tabControl.SelectedIndex == tabControl.Items.Count) // Если удаленная вкладка была последней, выбираем предыдущую
                {
                    tabControl.SelectedIndex = tabControl.Items.Count - 1;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Button closeButton = (Button)sender;
            TabItem tabItem = (TabItem)closeButton.TemplatedParent;
            tabControl.Items.Remove(tabItem);
        }
        //экспорт
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            var currentTabItem = tabControl.SelectedItem as TabItem;
            if (currentTabItem != null)
            {
                var currentChart = currentTabItem.Content as CartesianChart;
                if (currentChart != null)
                {
                    // Создать RenderTargetBitmap для рендеринга графика в растровое изображение
                    var renderTargetBitmap = new RenderTargetBitmap((int)currentChart.ActualWidth, (int)currentChart.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                    renderTargetBitmap.Render(currentChart);

                    // Создать PngBitmapEncoder для сохранения растрового изображения в файл формата PNG
                    var pngEncoder = new PngBitmapEncoder();
                    pngEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

                    // Создать SaveFileDialog для выбора места и имени файла, в который будет сохранен график
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "PNG Image|*.png",
                        Title = "Сохранить как"
                    };

                    // Показать SaveFileDialog и получить результат
                    var result = saveFileDialog.ShowDialog();

                    // Если пользователь нажал OK, сохранить растровое изображение в выбранный файл
                    if (result == true)
                    {
                        using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        {
                            pngEncoder.Save(fileStream);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выбранный график не является CartesianChart.");
                }
            }

        }
    }
}