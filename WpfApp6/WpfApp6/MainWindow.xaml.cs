using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Windows.Shapes;


namespace WpfApp6
{
        public partial class MainWindow : Window
        {
            public MainWindow()
            {
                InitializeComponent();
                CreateNewChart(); // Создаем первый график при запуске приложения
            }

        //Примеры графиков
        private void ChartTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранный тип графика
            string selectedChartType = (e.AddedItems[0] as ListBoxItem)?.Content.ToString();

            // Создаем пример графика в зависимости от выбранного типа
            UIElement chartExample = CreateChartExample(selectedChartType);

            // Отображаем пример графика в ContentControl
            ChartExampleContentControl.Content = chartExample;
        }

        private UIElement CreateChartExample(string chartType)
            {
                switch (chartType)
                {
                    case "Линия":
                        return CreateLineChart();
                    
                    case "Круговая диаграмма":
                        return CreatePieChart();
                case "Гистограмма":
                    return CreateHistogramChart();
                case "Точечная диаграмма":
                    return CreateScatterChart();
                case "Диаграмма площади":
                    return CreateAreaChart();
                case "Столбчатая диаграмма":
                    return CreateColumnChart();
                case "Сплит-столбчатая диаграмма":
                    return CreateStackedColumnChart();
                case "Круговая диаграмма с легендой":
                    return CreatePieChartWithLegend();
                default:
                        return null; // Возвращаем null, если тип графика не распознан
                }
            }

        private UIElement CreateAreaChart()
        {
            var areaSeries = new LineSeries
            {
                Values = new ChartValues<double> { 2, 1, 6, 4, 6, 5, 3, 2, 1 },
                Fill = System.Windows.Media.Brushes.LightBlue // Заливка области
            };

            return new CartesianChart
            {
                Series = new SeriesCollection
        {
            areaSeries
        },
                // Настройте другие свойства по необходимости
            };
        }

        private UIElement CreateColumnChart()
        {
            var columnSeries = new ColumnSeries
            {
                Values = new ChartValues<double> { 2, 1, 6, 4, 6, 5, 3, 2, 1 }
            };

            return new CartesianChart
            {
                Series = new SeriesCollection
        {
            columnSeries
        },
                // Настройте другие свойства по необходимости
            };
        }

        private UIElement CreateStackedColumnChart()
        {
            var stackedColumnSeries1 = new StackedColumnSeries
            {
                Values = new ChartValues<double> { 2, 1, 6 },
                StackMode = StackMode.Values, // StackMode.Values для сплит-столбчатой диаграммы
                DataLabels = true
            };

            var stackedColumnSeries2 = new StackedColumnSeries
            {
                Values = new ChartValues<double> { 4, 6, 5 },
                StackMode = StackMode.Values, // StackMode.Values для сплит-столбчатой диаграммы
                DataLabels = true
            };

            var stackedColumnSeries3 = new StackedColumnSeries
            {
                Values = new ChartValues<double> { 3, 2, 1 },
                StackMode = StackMode.Values, // StackMode.Values для сплит-столбчатой диаграммы
                DataLabels = true
            };

            return new CartesianChart
            {
                Series = new SeriesCollection
        {
            stackedColumnSeries1,
            stackedColumnSeries2,
            stackedColumnSeries3
        },
                // Настройте другие свойства по необходимости
            };
        }

        private UIElement CreatePieChartWithLegend()
        {
            var pieSeries = new PieSeries
            {
                Values = new ChartValues<double> { 2, 1, 6, 4, 6, 5, 3, 2, 1 },
                DataLabels = true,
                LabelPoint = value => value.Y.ToString()
            };

            return new PieChart
            {
                Series = new SeriesCollection
        {
            pieSeries
        },
                // Настройте другие свойства по необходимости
            };
        }

        private UIElement CreateScatterChart()
        {
            var scatterSeries = new ScatterSeries
            {
                Values = new ChartValues<ScatterPoint>
        {
            new ScatterPoint(0, 10),
            new ScatterPoint(10, 5),
            new ScatterPoint(5, 2),
            new ScatterPoint(2, 6)
        }
            };

            return new CartesianChart
            {
                Series = new SeriesCollection
        {
            scatterSeries
        },
                // Настройте другие свойства по необходимости
            };
        }
        private UIElement CreatePieChart()
        {
            var pieSeries = new PieSeries
            {
                Values = new ChartValues<double> { 2, 1, 6, 4, 6, 5, 3, 2, 1 },
                DataLabels = true
            };

            return new PieChart
            {
                Series = new SeriesCollection
        {
            pieSeries
        },
                // Настройте другие свойства по необходимости
            };
        }
        
        //Линейный график
        private UIElement CreateLineChart()
        {
            var lineSeries = new LineSeries
            {
                Values = new ChartValues<double> { 2, 1, 6, 4, 6, 5, 3, 2, 1 }
            };

            return new CartesianChart
            {
                Series = new SeriesCollection
        {
            lineSeries
        },
                // Настройте другие свойства по необходимости
            };
        }
        private UIElement CreateHistogramChart()
        {
            var histogramSeries = new ColumnSeries
            {
                Values = new ChartValues<double> { 2, 1, 6, 4, 6, 5, 3, 2, 1 },
                DataLabels = true
            };

            return new CartesianChart
            {
                Series = new SeriesCollection
        {
            histogramSeries
        },
                // Настройте другие свойства по необходимости
            };
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