using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace MyPro
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
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
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            myWindowState = WindowState.Minimized;
        }
        private WindowState myWindowState;
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
    }
}
