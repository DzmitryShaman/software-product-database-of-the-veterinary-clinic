using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Infrastructure;
using System.Collections.ObjectModel;

namespace MyPro.Frames
{
    /// <summary>
    /// Логика взаимодействия для MembersPage.xaml
    /// </summary>
    public partial class MembersPage : Page
    {
        VetClinicEntities _context = new VetClinicEntities();
        private List<Сотрудники> checkedEmploies = new List<Сотрудники>();
        public MembersPage()
        {
            InitializeComponent();

            UpdateDataGrid();
            //allSotrud.ItemsSource = _context.Сотрудники.ToList();
            AddDoljnost.ItemsSource = _context.Должности.ToList();
            EditDoljnost.ItemsSource = _context.Должности.ToList();
            AllSotrFIO.ItemsSource = _context.Сотрудники.OrderBy(item => item.ФИО).ToList();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Data.MainFrame.Content = new AddCotrydniki();
        }

        private void textBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = textBoxFilter.Text;
            //var filteredSotrudniki = _context.Сотрудники.Where(s => s.ФИО.Contains(searchQuery)).ToList();

            // Пример:
            var startIndex = (currentPage - 1) * itemsPerPage;
            var sortedData = _context.Сотрудники.Where(s => s.ФИО.Contains(searchQuery) || s.Должности.Наименование_должности.Contains(searchQuery)).ToList();
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            allSotrud.ItemsSource = pageData;
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Сохранить?", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (Сотрудники item in allSotrud.Items)
                    {
                        Сотрудники CurrentUser = _context.Сотрудники.FirstOrDefault(item1 => item1.Код_сотрудника == item.Код_сотрудника);
                        CurrentUser.ФИО = item.ФИО;
                        CurrentUser.Адрес = item.Адрес;
                        CurrentUser.Телефон = item.Телефон;
                        CurrentUser.Email = item.Email;
                        CurrentUser.Примечание = item.Примечание;
                        CurrentUser.Логин = item.Логин;
                        CurrentUser.Пароль = item.Пароль;
                        CurrentUser.Код_должность = _context.Должности.First(item1 => item1.Наименование_должности == item.Должности.Наименование_должности).Код_должности;
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
            if (currentPage < totalPages)
            {
                currentPage++;
                if(!UpdateDataGrid()) return;
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
            

            var startIndex = (currentPage - 1) * itemsPerPage;
            //var pageData = _context.Должности.Skip(startIndex).Take(itemsPerPage).ToList();
            var sortedData = _context.Сотрудники.OrderBy(d => d.Код_сотрудника);
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            if (pageData.Count() == 0)
            {
                return false;
            }

            // Обновите ItemsSource вашего DataGrid
            allSotrud.ItemsSource = pageData;
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

        private void AddDoljnost_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string filter = AddDoljnost.Text.ToLower();
            var filteredPositions = _context.Должности
                                           .Where(position => position.Наименование_должности.ToLower().Contains(filter))
                                           .ToList();

            AddDoljnost.ItemsSource = filteredPositions;

            // Открываем список для отображения результатов фильтрации
            AddDoljnost.IsDropDownOpen = true;
        }

        private void addNewEmloyee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка заполнения всех полей
                if (string.IsNullOrWhiteSpace(AddFIO.Text) || string.IsNullOrWhiteSpace(AddAdress.Text) ||
            string.IsNullOrWhiteSpace(AddTelephone.Text) || string.IsNullOrWhiteSpace(AddEmail.Text) ||
            AddDoljnost.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка");
                    return;
                }

                // Проверка, существует ли сотрудник с таким ФИО
                string fio = AddFIO.Text;
                if (_context.Сотрудники.Any(s => s.ФИО == fio))
                {
                    MessageBox.Show("Такой сотрудник уже существует", "Ошибка");
                    return;
                }

                // Проверка корректности заполнения поля ФИО
                if (!IsCorrectFIO(fio))
                {
                    MessageBox.Show("Поле ФИО должно содержать три слова без цифр", "Ошибка");
                    return;
                }

                Сотрудники newEmployee = new Сотрудники()
                {
                    ФИО = fio,
                    Адрес = AddAdress.Text,
                    Телефон = AddTelephone.Text,
                    Email = AddEmail.Text,
                    Примечание = AddPrimich.Text,
                    Должности = AddDoljnost.SelectedItem as Должности
                };
                _context.Сотрудники.Add(newEmployee);
                _context.SaveChanges();
                MessageBox.Show("Сотрудник успешно добавлен");
                AddFIO.Clear(); AddAdress.Clear(); AddTelephone.Clear(); AddEmail.Clear(); AddPrimich.Clear(); AddDoljnost.SelectedIndex = -1;
                UpdateDataGrid();
                AllSotrFIO.ItemsSource = _context.Сотрудники.OrderBy(item => item.ФИО).ToList();
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте правильность заполнения полей", "Ошибка");
            }
        }

        // Метод для проверки корректности заполнения поля ФИО
        private bool IsCorrectFIO(string fio)
        {
            // Разбиваем строку ФИО на слова по пробелам
            string[] words = fio.Split(' ');

            // Проверяем количество слов и отсутствие цифр в каждом слове
            if (words.Length != 3)
                return false;

            foreach (string word in words)
            {
                foreach (char c in word)
                {
                    if (char.IsDigit(c))
                        return false;
                }
            }

            return true;
        }

        private void AllSotrFIO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataContext = AllSotrFIO.SelectedItem as Сотрудники;
            if (DataContext is null)
            {
                return;
            }
            EditDoljnost.Text = (DataContext as Сотрудники).Должности.Наименование_должности;
        }
        private void ДолжностиViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is Должности должность)
            {
                if (string.IsNullOrEmpty(AddDoljnost.Text))
                {
                    e.Accepted = true;
                    return;
                }

                
                e.Accepted = должность.Наименование_должности.ToLower().Contains(AddDoljnost.Text.ToLower());
            }
        }
        private void ДолжнViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is Должности должность)
            {
                if (string.IsNullOrEmpty(EditDoljnost.Text))
                {
                    e.Accepted = true;
                    return;
                }

                // Фильтрация по ФИО
                e.Accepted = должность.Наименование_должности.ToLower().Contains(EditDoljnost.Text.ToLower());
            }
        }
        private void EditDoljnost_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string filter = EditDoljnost.Text.ToLower();
            var filteredPositions = _context.Должности
                                           .Where(position => position.Наименование_должности.ToLower().Contains(filter))
                                           .ToList();

            EditDoljnost.ItemsSource = filteredPositions;

            // Открываем список для отображения результатов фильтрации
            EditDoljnost.IsDropDownOpen = true;
        }
        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите изменить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                try
                {
                Сотрудники SelectedEmployer = DataContext as Сотрудники;
                if (SelectedEmployer is null)
                {
                    MessageBox.Show("Выберите сотрудника", "Ошибка");
                    return;
                }

                SelectedEmployer.Должности = _context.Должности.FirstOrDefault(item => item.Наименование_должности == EditDoljnost.Text);
                _context.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены!", "Уведомление");
                AllSotrFIO.SelectedIndex = -1; FioInput.Clear(); adressInput.Clear(); TelInput.Clear(); emailInput.Clear(); EditDoljnost.SelectedIndex = -1; PrimichInput.Clear();
                AllSotrFIO.ItemsSource = _context.Сотрудники.OrderBy(item => item.ФИО).ToList();
                EditDoljnost.SelectedIndex = -1;
                UpdateDataGrid();
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте правильность заполнения полей", "Error");
            }        
            else
            {
                return;
            }
        }

        private void RemoveEmployee_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Сотрудники SelectedEmployer = DataContext as Сотрудники;
                    if (SelectedEmployer is null)
                    {
                        MessageBox.Show("Выберите сотрудника", "Error");
                        return;
                    }

                    _context.Сотрудники.Remove(SelectedEmployer);
                    _context.SaveChanges();
                    MessageBox.Show("Сотрудник успешно удалён!", "Уведомление");
                    AllSotrFIO.ItemsSource = _context.Сотрудники.OrderBy(item => item.ФИО).ToList();
                    EditDoljnost.SelectedIndex = -1;
                    UpdateDataGrid();
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

        private void AllSotrFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            AllSotrFIO.ItemsSource = _context.Сотрудники.ToList().Where(item => item.ФИО.Contains(AllSotrFIO.Text)).OrderBy(item => item.ФИО);
        }

        private void Otchet_Click(object sender, RoutedEventArgs e)
        {
            if (!Data.isWordWindowOpen)
            {
                Data.isWordWindowOpen = true;
                SotrudWordWindow ww = new SotrudWordWindow();
                ww.ShowDialog();
            }
        }
    }
}
