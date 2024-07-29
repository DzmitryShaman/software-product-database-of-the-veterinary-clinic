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
    /// Логика взаимодействия для AddCotrydniki.xaml
    /// </summary>
    public partial class AddCotrydniki : Page
    {
        VetClinicEntities _context = new VetClinicEntities();
        private List<Сотрудники> checkedEmploies = new List<Сотрудники>();
        public AddCotrydniki()
        {
            InitializeComponent();
            AddDoljnost.ItemsSource = _context.Должности.ToList();
        }

        private void cancellation_Click(object sender, RoutedEventArgs e)
        {
            Data.MainFrame.Content = new MembersPage();
        }        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранное наименование должности
            string selectedDoljnost = AddDoljnost.Text;

            if (selectedDoljnost.Length > 3)
            {
                MessageBox.Show("Введи правильную должность > 4 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Поиск записи с выбранным наименованием должности в таблице "Должности"
            Должности selectedDoljnostRecord = _context.Должности.FirstOrDefault(d => d.Наименование_должности == selectedDoljnost);

            if (selectedDoljnostRecord == null)
            {
                selectedDoljnostRecord = new Должности()
                {
                    Наименование_должности = selectedDoljnost,
                };
                _context.Должности.Add(selectedDoljnostRecord);
                _context.SaveChanges();
            }

            // Если запись с выбранным наименованием должности найдена,
            // создаем нового сотрудника и устанавливаем соответствующую должность
            Сотрудники newEmployee = new Сотрудники
            {
                ФИО = AddFIO.Text,
                Адрес = AddAdress.Text,
                Телефон = AddTelephone.Text,
                Email = AddEmail.Text,
                Должности = selectedDoljnostRecord,
                Примечание = AddPrimich.Text
            };

            // Добавляем нового сотрудника в базу данных
            _context.Сотрудники.Add(newEmployee);
            _context.SaveChanges();

            // Очищаем поля после добавления
            AddFIO.Clear();
            AddAdress.Clear();
            AddTelephone.Clear();
            AddEmail.Clear();
            AddDoljnost.SelectedIndex = -1;
            AddDoljnost.Text = "";
            AddPrimich.Clear();
            MessageBox.Show("Вы успешно добавили сотрудника", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void AddDoljnost_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = AddDoljnost.Text.ToLower(); // Получаем текст поиска в нижнем регистре

            // Фильтруем элементы источника данных
            var filteredItems = _context.Должности.Where(d => d.Наименование_должности.ToLower().Contains(searchText)).ToList();

            // Устанавливаем отфильтрованные элементы как источник данных ComboBox
            AddDoljnost.ItemsSource = filteredItems;
        }
    }
}
