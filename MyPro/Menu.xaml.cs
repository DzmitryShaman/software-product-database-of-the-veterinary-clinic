using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using MyPro.Frames;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace MyPro
{
    
    public partial class Menu : Window
    {
        private const string ConfigFilePath = "config.json";
        private DispatcherTimer timer;
        public Menu()
        {
            InitializeComponent();
            LoadImage();
            nameText.Text = Data.CurrentUser.ФИО;
            mainFrame.Content = new MainPage();
            Data.MainFrame = mainFrame;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;

            // Запускаем таймер
            timer.Start();

        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            }
        }
        private bool IsMaximized = false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximized)
                {
                    this.WindowState=WindowState.Normal;
                    this.Width = 1080;
                    this.Height = 720;

                    IsMaximized=false;
                }
                else
                {
                    this.WindowState=WindowState.Maximized;
                    

                    IsMaximized=true;
                }
            }
        }
        private void AddPersonButton_Click(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Изображения (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif|Все файлы (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                try
                {
                    // Отобразите выбранное изображение в ImageBrush вашей кнопки:
                    BitmapImage bitmapImage = new BitmapImage(new Uri(selectedFilePath));
                    ImageBrush imageBrush = new ImageBrush(bitmapImage);
                    AddPersonButton.Background = imageBrush;

                    // Вы можете также сохранить путь к файлу или обрабатывать изображение по своему усмотрению.
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при открытии изображения: {ex.Message}");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти из аккаунта?", "Завершение работы", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Закрываем приложение, если пользователь подтверждает выход
                MainWindow MainMenu = new MainWindow();
                MainMenu.Show();
                this.Close();
            }           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new MembersPage();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new MainPage();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new MainJivotnie();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            // Обновите текст в TextBlock для времени
            timeText.Text = now.ToString("HH:mm:ss");

            // Обновите текст в TextBlock для месяца и года
            dateText.Text = now.ToString("MMMM yyyy");
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new Client();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new UsedMedicines();
        }
        private void AddPersonButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedImagePath = openFileDialog.FileName;

                // Создание BitmapImage из выбранного файла
                BitmapImage bitmap = new BitmapImage(new Uri(selectedImagePath));

                // Установка BitmapImage как источника заполнения Ellipse
                ((Ellipse)((Border)sender).Child).Fill = new ImageBrush(bitmap);

                SaveImagePath(selectedImagePath);
                // Получение текущей директории
                string currentDirectory = System.IO.Directory.GetCurrentDirectory();

                // Поиск папки "MyPro" в текущей директории
                DirectoryInfo parentDirectory = Directory.GetParent(currentDirectory);
                while (parentDirectory != null && parentDirectory.Name != "MyPro")
                {
                    parentDirectory = parentDirectory.Parent;
                }

                if (parentDirectory == null)
                {
                    MessageBox.Show("Папка 'MyPro' не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Получение пути к папке "картинки" внутри папки "MyPro"
                string destinationFolder = System.IO.Path.Combine(parentDirectory.FullName, "картинки");
                string destinationPath = System.IO.Path.Combine(destinationFolder, System.IO.Path.GetFileName(selectedImagePath));

                try
                {
                    if (!Directory.Exists(destinationFolder))
                        Directory.CreateDirectory(destinationFolder);

                    File.Copy(selectedImagePath, destinationPath, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при копировании файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Подгрузка изображения в ресурсы приложения
                var uriSource = new Uri(destinationPath, UriKind.RelativeOrAbsolute);
                var bitmapImage = new BitmapImage(uriSource);
                ((Ellipse)((Border)sender).Child).Fill = new ImageBrush(bitmapImage);
            }
        }

        private void SaveImagePath(string imagePath)
        {
            try
            {
                // Сохранение пути к выбранной картинке в файл конфигурации
                string jsonData = JsonConvert.SerializeObject(imagePath);
                File.WriteAllText(ConfigFilePath, jsonData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadImage()
        {
            string selectedImagePath = null;

            if (File.Exists(ConfigFilePath))
            {
                try
                {
                    // Загрузка пути к картинке из файла конфигурации
                    string jsonData = File.ReadAllText(ConfigFilePath);
                    selectedImagePath = JsonConvert.DeserializeObject<string>(jsonData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (!string.IsNullOrEmpty(selectedImagePath) && File.Exists(selectedImagePath))
            {
                // Если исходное изображение существует, загрузить его
                BitmapImage bitmapImage = new BitmapImage(new Uri(selectedImagePath));
                ((Ellipse)AddPersonButton.Child).Fill = new ImageBrush(bitmapImage);
            }
            else
            {
                // Иначе загрузить изображение из папки "картинки", если оно там есть
                DirectoryInfo parentDirectory = null;
                string currentDirectory = System.IO.Directory.GetCurrentDirectory();
                parentDirectory = Directory.GetParent(currentDirectory);

                while (parentDirectory != null && parentDirectory.Name != "MyPro")
                {
                    parentDirectory = parentDirectory.Parent;
                }

                if (parentDirectory == null)
                {
                    MessageBox.Show("Папка 'MyPro' не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string destinationFolder = System.IO.Path.Combine(parentDirectory.FullName, "картинки");
                string destinationPath = System.IO.Path.Combine(destinationFolder, System.IO.Path.GetFileName(selectedImagePath));

                if (File.Exists(destinationPath))
                {
                    // Если изображение найдено в папке "картинки", загрузить его
                    BitmapImage bitmapImage = new BitmapImage(new Uri(destinationPath));
                    ((Ellipse)AddPersonButton.Child).Fill = new ImageBrush(bitmapImage);
                }
                else
                {
                    // Если изображение не найдено в папке "картинки", установить фон на Ellipse какой-то заглушкой или пустым значением.
                    ((Ellipse)AddPersonButton.Child).Fill = Brushes.Gray; // Например, серый цвет в качестве заглушки
                }
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new Spravochnik();
        }
    }
}
