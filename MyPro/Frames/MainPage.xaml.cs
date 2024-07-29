using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace MyPro.Frames
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        VetClinicEntities _context = new VetClinicEntities();
        private string[] imageFiles; // Массив для хранения путей к изображениям
        private int currentIndex = 0; // Индекс текущего изображения
        private DispatcherTimer timer; // Таймер для автоматического переключения

        public MainPage()
        {
            InitializeComponent();
            LoadImagesFromFolder(); // Загрузка изображений из папки проекта
            DisplayCurrentImage(); // Отображение первого изображения

            // Создание и настройка таймера
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(15); // Время переключения в секундах
            timer.Tick += Timer_Tick;
            timer.Start();
            DataContext = this;
            DoubleAnimation animation = new DoubleAnimation
            {
                To = -runningText.ActualWidth, // конечная позиция (полностью за пределами окна)
                Duration = TimeSpan.FromSeconds(10), // продолжительность анимации (10 секунд, настройте по своему усмотрению)
            };

            int currentYear = DateTime.Now.Year;

            for (int i = currentYear - 8; i < currentYear + 2; i++)
            {
                yearGrid.Items.Add(i);
            }
            yearGrid.SelectedItem = currentYear;

            // Назначаем обработчик события завершения анимации для воспроизведения анимации снова
            animation.Completed += (sender, e) =>
            {
                Canvas.SetLeft(runningText, 500); // Восстанавливаем начальную позицию
                runningText.BeginAnimation(Canvas.LeftProperty, animation);
            };

            runningText.BeginAnimation(Canvas.LeftProperty, animation);

            var query = from item in _context.Оказание_услуг
                        join item1 in _context.Журнал_посещений
                        on item.C__журнала_посещения equals item1.Код_номера_журнала //тут было item.код услуги
                        join item2 in _context.Сотрудники
                        on item.Код_сотрудника equals item2.Код_сотрудника
                        select item;

            Labels = new[] { "Янв", "Фев", "Мар", "Апр", "Май", "Июн", "Июл", "Авг", "Сент", "Окт", "Ноя", "Дек" };
            YFormatter = value => value.ToString("N");

            int allOper = query.Count();
            foreach (var item in query.GroupBy(a => a.Код_сотрудника).ToList())
            {
                var curUser = _context.Оказание_услуг.FirstOrDefault(a => a.Код_сотрудника == item.Key);
                pieChart.Series.Add(new PieSeries
                {
                    Title = curUser.Сотрудники.ФИО,
                    Values = new ChartValues<ObservableValue> { new ObservableValue(double.Parse(((float)item.Count() / allOper * 100).ToString("##.##"))) },
                    DataLabels = true
                });
            };

            changeChart(currentYear);
            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }

        private Brush PickBrush()
        {
            Brush result = Brushes.Transparent;

            Random rnd = new Random();

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            int random = rnd.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);

            return result;
        }

        private void sortClick(object sender, RoutedEventArgs e)
        {
            pieChart.Series.Clear();

            var query = from item in _context.Оказание_услуг
                        join item1 in _context.Журнал_посещений
                        on item.C__журнала_посещения equals item1.Код_номера_журнала
                        join item2 in _context.Сотрудники
                        on item.Код_сотрудника equals item2.Код_сотрудника
                        where item1.Дата >= fromDate.SelectedDate && item1.Дата <= toDate.SelectedDate
                        select item;

            int allOper = query.Count();
            foreach (var item in query.GroupBy(a => a.Код_сотрудника).ToList())
            {
                var curUser = _context.Оказание_услуг.FirstOrDefault(a => a.Код_сотрудника == item.Key);
                pieChart.Series.Add(new PieSeries
                {
                    Title = curUser.Сотрудники.ФИО,
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Math.Round((float)item.Count() / allOper * 100, 2)) },
                    DataLabels = true
                });
            };

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (e.Uri != null)
            {
                try
                {
                    // Открываем URL в браузере по умолчанию
                    Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                    e.Handled = true;
                }
                catch (Exception ex)
                {
                    // Обработка исключения, если не удалось открыть URL
                    MessageBox.Show("Произошла ошибка при открытии ссылки: " + ex.Message);
                }
            }
        }
        private void LoadImagesFromFolder()
        {
            string basePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\"));
            string folderPath = Path.Combine(basePath, "MyPro", "slider");

            // Проверяем существование папки
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"Не удалось найти папку: {folderPath}");
            }

            imageFiles = Directory.GetFiles(folderPath, "*.jpg"); // Получаем все файлы jpg из указанной папки
        }

        // Метод для отображения текущего изображения
        private void DisplayCurrentImage()
        {
            if (imageFiles.Length > 0)
            {
                // Создаем BitmapImage из файла
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageFiles[currentIndex]);
                bitmap.EndInit();

                // Устанавливаем изображение в элемент Image
                imgSlider.Source = bitmap;
            }
        }

        // Обработчик события таймера для автоматического переключения изображений
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Увеличиваем индекс, обеспечивая зацикливание
            currentIndex = (currentIndex + 1) % imageFiles.Length;
            DisplayCurrentImage();
        }

        // Обработчик нажатия на кнопку "Вперед"
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            // Переключаемся на следующее изображение
            currentIndex = (currentIndex + 1) % imageFiles.Length;
            DisplayCurrentImage();
        }

        // Обработчик нажатия на кнопку "Назад"
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            // Переключаемся на предыдущее изображение
            currentIndex = (currentIndex + imageFiles.Length - 1) % imageFiles.Length;
            DisplayCurrentImage();
        } 

        private void yearGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SeriesCollection.Clear();
            changeChart(int.Parse(yearGrid.SelectedItem.ToString()));
        }

        private void changeChart(int year)
        {
            var query = from item in _context.Оказание_услуг
                        join item1 in _context.Журнал_посещений
                        on item.C__журнала_посещения equals item1.Код_номера_журнала //тут было item.код услуги
                        join item2 in _context.Сотрудники
                        on item.Код_сотрудника equals item2.Код_сотрудника
                        select item;
            foreach (var item in query.ToList())
            {
                LineSeries curSeries = (LineSeries)SeriesCollection.FirstOrDefault(a => a.Title.Contains(item.Сотрудники.ФИО));
                int CM = item.Журнал_посещений.Дата.Month;
                int[] vals = new int[12];

                // СТАТА БЕРЕТСЯ С ТОГО КОГДА КОТОРЫЙ ВЫБРАН
                if (item.Журнал_посещений.Дата.Year != year)
                {
                    continue;
                }
                //vals[CM - 1] = _context.Journal.Where(b => b.JourID == item.ID_Journal && b.DateOf.Year == DateTime.Now.Year).Count();
                vals[CM - 1] = _context.Журнал_посещений.Any(b => b.Код_номера_журнала == item.Код_услуги) ? 1 : 0;

                if (curSeries != null)
                {
                    ChartValues<int> q = (ChartValues<int>)curSeries.Values;
                    q[CM - 1]++;
                    continue;
                }

                SeriesCollection.Add(new LineSeries
                {
                    Title = item.Сотрудники.ФИО,
                    Values = new ChartValues<int>(vals),
                    LineSmoothness = 0, //0: straight lines, 1: really smooth lines
                                        //PointGeometry = Geometry.Parse("m 25 70.36218 20 -28 -20 22 -8 -6 z"),
                                        //PointGeometrySize = 25,
                    PointForeground = PickBrush()
                });

            }
        }
    }
}
