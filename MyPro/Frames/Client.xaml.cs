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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyPro.Frames
{
    /// <summary>
    /// Логика взаимодействия для Client.xaml
    /// </summary>
    public partial class Client : Page
    {
        VetClinicEntities _context = new VetClinicEntities();
        private List<Клиенты> checkedClient = new List<Клиенты>();
        public Client()
        {
            InitializeComponent();
            allClient.ItemsSource = _context.Клиенты.ToList();
            AllClientFIO.ItemsSource = _context.Клиенты.ToList();
            UpdateDataGrid();
        }

        private void cancellation_Click(object sender, RoutedEventArgs e)
        {
            Data.MainFrame.Content = new MembersPage();
        }
        private void textBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = textBoxFilter.Text;

            // Пример:
            var startIndex = (currentPage - 1) * itemsPerPage;
            var sortedData = _context.Клиенты.Where(s => s.ФИО.Contains(searchQuery)).ToList();
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            allClient.ItemsSource = pageData;
        }
        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Сохранить?", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (Клиенты item in allClient.Items)
                    {
                        Клиенты CurrentUser = _context.Клиенты.FirstOrDefault(item1 => item1.Код_клиента == item.Код_клиента);
                        CurrentUser.ФИО = item.ФИО;
                        CurrentUser.Адрес = item.Адрес;
                        CurrentUser.Телефон = item.Телефон;
                        CurrentUser.Email = item.Email;
                        CurrentUser.Примечание = item.Примечание;
                        _context.SaveChanges();
                    }
                    MessageBox.Show("Успешно сохранено");
                }
                catch (Exception)
                {
                    MessageBox.Show("Проверьте правильность заполнения полей", "Повторите попытку");
                    //тут вообще надо вернуть но хз items source
                    return;
                }
                //allSotrud.Items
            }
        }
        private int currentPage = 1;
        private int totalPages = 9; // Общее количество страниц (настройте по вашим данным)
        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdateDataGrid();
                ColorSetter(sender);
            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage * itemsPerPage < _context.Клиенты.Count())
            {
                currentPage++;
                if (!UpdateDataGrid()) return;
                ColorSetter(sender);
            }
        }

        private void ColorSetter(object currentBtn)
        {
            var q = currentBtn as Button;

            foreach (var item in allButtons.Children)
            {
                if (item is Button)
                {
                    Button button = item as Button;
                    button.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#6C7682");
                    button.Background = Brushes.Transparent;
                    if (button.Content.ToString().Contains(currentPage.ToString()))
                    {
                        button.Foreground = Brushes.White;
                        button.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#7950F2");
                    }
                }
            }

            if (!q.Name.Contains("p"))
            {
                q.Foreground = Brushes.White;
                q.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#7950F2");
            }
        }

        int itemsPerPage = 14; // Количество элементов на странице
        private bool UpdateDataGrid()
        {
            int startIndex;
            if (currentPage == 1)
            {
                startIndex = 0; // Начинаем с первой записи
            }
            else
            {
                startIndex = (currentPage - 1) * itemsPerPage; // Начинаем с нужной записи для текущей страницы
            }

            var sortedData = _context.Клиенты.OrderBy(d => d.Код_клиента);
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            if (pageData.Count() == 0)
            {
                return false;
            }

            // Обновите ItemsSource вашего DataGrid
            allClient.ItemsSource = pageData;
            return true;
        }
        private void PageButton_Click(object sender, RoutedEventArgs e)
        {
            // Преобразуйте отправителя (sender) в кнопку
            Button button = sender as Button;

            if (button != null)
            {
                // Получите текст (номер страницы) из кнопки
                string pageNumberText = button.Content.ToString();

                // Попробуйте преобразовать текст в число
                if (int.TryParse(pageNumberText, out int selectedPage))
                {
                    // Установите новое значение currentPage
                    currentPage = selectedPage;

                    // Вызовите метод обновления DataGrid для текущей страницы
                    UpdateDataGrid();
                    ColorSetter(sender);
                }
            }
        }
        private void addNewEmloyee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка заполнения полей
                if (string.IsNullOrWhiteSpace(Vladelec.Text) || string.IsNullOrWhiteSpace(AddAddress.Text) || string.IsNullOrWhiteSpace(AddTelephone.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля (Владелец, Адрес, Телефон)", "Ошибка");
                    return;
                }

                // Проверка корректности заполнения поля ФИО
                string fio = Vladelec.Text;
                if (!IsCorrectFIO(fio))
                {
                    MessageBox.Show("Поле ФИО должно содержать три слова без цифр", "Ошибка");
                    return;
                }

                // Проверка, существует ли клиент с таким же ФИО
                if (_context.Клиенты.Any(c => c.ФИО == fio))
                {
                    MessageBox.Show("Такой клиент уже существует", "Ошибка");
                    return;
                }

                Клиенты newEmployee = new Клиенты()
                {
                    ФИО = fio,
                    Адрес = AddAddress.Text,
                    Телефон = AddTelephone.Text,
                    Email = AddEmail.Text,
                    Примечание = primichInput.Text
                };
                _context.Клиенты.Add(newEmployee);
                _context.SaveChanges();
                MessageBox.Show("Клиент успешно добавлен");
                Vladelec.Clear(); AddAddress.Clear(); AddTelephone.Clear(); AddEmail.Clear(); primichInput.Clear();
                allClient.ItemsSource = _context.Клиенты.ToList();
                AllClientFIO.ItemsSource = _context.Клиенты.ToList();
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте правильность заполнения полей", "Ошибка");
            }
        }

        private bool IsCorrectFIO(string fio)
        {
            // Проверяем, что ФИО состоит из трех слов без цифр
            string[] words = fio.Split(' ');
            if (words.Length != 3)
                return false;

            foreach (string word in words)
            {
                if (ContainsDigit(word))
                    return false;
            }

            return true;
        }

        private bool ContainsDigit(string str)
        {
            // Проверяем, содержит ли строка хотя бы одну цифру
            foreach (char c in str)
            {
                if (char.IsDigit(c))
                    return true;
            }
            return false;
        }
        private void AllClientFIO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataContext = AllClientFIO.SelectedItem as Клиенты;
            //polInput.Text = (q.item1 as Животные).Пол;
            //EditDoljnost.Text = (DataContext as Животные).Должности.Наименование_должности;           
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Клиенты SelectedEmployer = DataContext as Клиенты;
                if (SelectedEmployer is null)
                {
                    MessageBox.Show("Выберите клиента", "Ошибка");
                    return;
                }

                _context.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены!", "Уведомление");
                AllClientFIO.SelectedIndex = -1; adressInput.Clear(); TelInput.Clear(); emailInput.Clear(); primichInput.Clear(); AddInfo.Clear();
                AllClientFIO.ItemsSource = _context.Клиенты.ToList();
                allClient.ItemsSource = _context.Клиенты.ToList();
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте правильность заполнения полей", "Ошибка");
            }
        }
        private void RemoveEmployee_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Клиенты SelectedEmployer = DataContext as Клиенты;
                    if (SelectedEmployer is null)
                    {
                        MessageBox.Show("Выберите клиента", "Ошибка");
                        return;
                    }

                    _context.Клиенты.Remove(SelectedEmployer);
                    _context.SaveChanges();
                    MessageBox.Show("Клиент успешно удалён!", "Уведомление");
                    AllClientFIO.ItemsSource = _context.Клиенты.ToList();
                    allClient.ItemsSource = _context.Клиенты.ToList();
                    //EditDoljnost.SelectedIndex = -1;
                }
                catch (Exception)
                {
                    MessageBox.Show("Проверьте правильность заполнения полей", "Error");
                }
            }
            else
            {
                return;
            }
            
        }

        private void AllClientFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            AllClientFIO.ItemsSource = _context.Клиенты.ToList().Where(item => item.ФИО.Contains(AllClientFIO.Text));
        }
    }
}
