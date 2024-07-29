using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace MyPro
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VetClinicEntities _context = new VetClinicEntities();
        public MainWindow()
        {
            InitializeComponent();           
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //строка подключения анимации переливания
            var storyboard = (Storyboard)FindResource("TextAnimation");
            storyboard.Begin();
            if (Properties.Settings.Default.Username != string.Empty)
            {
                loginInput.Text = Properties.Settings.Default.Username;
                passwordInput.Password = Properties.Settings.Default.Password;
                rememberMeCheckBox.IsChecked = true;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите выйти?", "Завершение работы", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Закрываем приложение, если пользователь подтверждает выход
                Application.Current.Shutdown();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (e.Uri != null)
            {
                try
                {
                    // Открываем URL mail.ru в браузере
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

        private void go_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на заполнение полей
            if (string.IsNullOrWhiteSpace(loginInput.Text) || string.IsNullOrWhiteSpace(passwordInput.Password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка аутентификации
            if (_context.Сотрудники.Any(item => item.Логин == loginInput.Text && item.Пароль == passwordInput.Password))
            {
                // Вход выполнен успешно
                Data.CurrentUser = _context.Сотрудники.First(item => item.Логин == loginInput.Text);
                Menu menu = new Menu();
                menu.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                // Попытка подключения к БД
            }
            catch (Exception)
            {
                MessageBox.Show($"Ошибка подключения к БД", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Сохранение учетных данных, если отмечено "Запомнить меня"
            if (rememberMeCheckBox.IsChecked == true)
            {
                Properties.Settings.Default.Username = loginInput.Text;
                Properties.Settings.Default.Password = passwordInput.Password;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.Username = "";
                Properties.Settings.Default.Password = "";
                Properties.Settings.Default.Save();
            }

        }
    }
}



