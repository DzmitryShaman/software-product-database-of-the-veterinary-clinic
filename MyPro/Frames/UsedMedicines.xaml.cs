using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace MyPro.Frames
{
    /// <summary>
    /// Логика взаимодействия для UsedMedicines.xaml
    /// </summary>
    public partial class UsedMedicines : Page
    {
        VetClinicEntities _context = new VetClinicEntities();
        private string curItem = null;
        public UsedMedicines()
        {
            InitializeComponent();
            UpdateDataGrid();

            var q = (from item in _context.Журнал_посещений
                     join item1 in _context.Животные
                     on item.Код_животного equals item1.Код_животного
                     join item3 in _context.Оказание_услуг
                     on item.Код_номера_журнала equals item3.C__журнала_посещения
                     let serviceCount = _context.Оказание_услуг
                         .Where(a => a.C__журнала_посещения == item.Код_номера_журнала)
                         .Count()
                     select new
                     {
                         Journal = item,
                         Animals = item1,
                         OkazUslugi = item3,
                         SumField = serviceCount
                     }).AsEnumerable()
                        .Select(item => new
                        {
                            item.Journal,
                            item.Animals,
                            item.OkazUslugi,
                            SumField = item.SumField * double.Parse(item.OkazUslugi.Услуги.Цена.Replace('.', ','))
                        })
                        .GroupBy(item => item.OkazUslugi.C__журнала_посещения);

            allPos.ItemsSource = q.ToList();

            //Dobavlenie
            //AnimalCmb.ItemsSource = _context.Животные.ToList();
            CurClientCmb.ItemsSource = _context.Клиенты.ToList().Where(item => item.Животные.Count() != 0);
            UslugaCmb.ItemsSource = _context.Услуги.ToList();
            EmplCmb.ItemsSource = _context.Сотрудники.ToList();

            //Edit/delete
            EditEmplCmb.ItemsSource = _context.Сотрудники.ToList();
            EditUslugaCmb.ItemsSource = UslugaCmb.ItemsSource;
            EditAnimalCmb.ItemsSource = _context.Животные.ToList();
            EditAnimalCmb.ItemsSource = AnimalCmb.ItemsSource;
            EditItemCmb.ItemsSource = q.ToList().Select(item => item.Select(aaa => aaa.Journal).First().Дата.ToShortDateString() + " " + item.Select(aaa => aaa.Animals).First().Клиенты.ФИО);
        }

        private int currentPage = 1;
        private int totalPages = 5; // Общее количество страниц (настройте по вашим данным)
        private int itemsPerPage = 5;
        private void textBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = textBoxFilter.Text;

            var q = (from item in _context.Журнал_посещений
                     join item1 in _context.Животные
                     on item.Код_животного equals item1.Код_животного
                     join item3 in _context.Оказание_услуг
                     on item.Код_номера_журнала equals item3.C__журнала_посещения
                     where item1.Клиенты.ФИО.Contains(searchQuery)
                     let serviceCount = _context.Оказание_услуг
                         .Where(a => a.C__журнала_посещения == item.Код_номера_журнала)
                         .Count()
                     select new
                     {
                         Journal = item,
                         Animals = item1,
                         OkazUslugi = item3,
                         SumField = serviceCount
                     }).AsEnumerable()
                        .Select(item => new
                        {
                            item.Journal,
                            item.Animals,
                            item.OkazUslugi,
                            SumField = item.SumField * double.Parse(item.OkazUslugi.Услуги.Цена.Replace('.', ','))
                        })
                        .GroupBy(item => item.OkazUslugi.C__журнала_посещения);

            allPos.ItemsSource = q.ToList();
        }

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

        private bool UpdateDataGrid()
        {   
            var q = (from item in _context.Журнал_посещений
                     join item1 in _context.Животные
                     on item.Код_животного equals item1.Код_животного
                     join item3 in _context.Оказание_услуг
                     on item.Код_номера_журнала equals item3.C__журнала_посещения
                     let serviceCount = _context.Оказание_услуг
                         .Where(a => a.C__журнала_посещения == item.Код_номера_журнала)
                         .Count()
                     select new
                     {
                         Journal = item,
                         Animals = item1,
                         OkazUslugi = item3,
                         SumField = serviceCount
                     }).AsEnumerable()
                        .Select(item => new
                        {
                            item.Journal,
                            item.Animals,
                            item.OkazUslugi,
                            SumField = item.SumField * double.Parse(item.OkazUslugi.Услуги.Цена.Replace('.', ','))
                        })
                        .GroupBy(item => item.OkazUslugi.C__журнала_посещения);

            var startIndex = (currentPage - 1) * itemsPerPage;
            //var pageData = _context.Должности.Skip(startIndex).Take(itemsPerPage).ToList();
            var sortedData = q.OrderBy(d => d.Select(qq => qq.OkazUslugi).First().C__препарата);
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            if (pageData.Count() == 0)
            {
                return false;
            }

            // Обновите ItemsSource вашего DataGrid
            //allPos.ItemsSource = pageData;
            allPos.ItemsSource = q.ToList();
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
                    if (!UpdateDataGrid()) return;
                    ColorSetter(sender);
                }
            }
        }

        private void CreateNewRowClick(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка заполнения обязательных полей
                if (VisitDate.SelectedDate == null ||
                    AnimalCmb.SelectedItem == null ||
                    EmplCmb.SelectedItem == null ||
                    UslugaCmb.SelectedItem == null)
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля", "Ошибка");
                    return;
                }

                // Создание новой записи в Журнал_посещений
                Журнал_посещений newRow = new Журнал_посещений()
                {
                    Дата = (DateTime)VisitDate.SelectedDate,
                    Код_животного = (AnimalCmb.SelectedItem as Животные).Код_животного
                };

                _context.Журнал_посещений.Add(newRow);
                _context.SaveChanges();

                // Создание новой записи в Оказание_услуг
                Оказание_услуг newOkazUslug = new Оказание_услуг()
                {
                    C__журнала_посещения = _context.Журнал_посещений.ToList().Last().Код_номера_журнала,
                    Код_сотрудника = (EmplCmb.SelectedItem as Сотрудники).Код_сотрудника,
                    Код_услуги = (UslugaCmb.SelectedItem as Услуги).Код_услуги,
                    Диагноз = DiagInput.Text,
                };

                _context.Оказание_услуг.Add(newOkazUslug);
                _context.SaveChanges();

                MessageBox.Show("Запись в журнал посещения успешно добавлена", "Уведомление");

                // Очистка полей после успешного добавления
                AnimalCmb.SelectedIndex = -1;
                EmplCmb.SelectedIndex = -1;
                UslugaCmb.SelectedIndex = -1;
                DiagInput.Clear();
                AnimalCmb.ItemsSource = _context.Животные.ToList();
                CurClientCmb.SelectedIndex = -1;
                VisitDate.SelectedDate = null;

                UpdateDataGrid();
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте правильность заполнения полей", "Ошибка");
            }
        }
        private void EditAnimalCmb_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string filter = EditAnimalCmb.Text.ToLower();
            var filteredAnimals = _context.Животные
                                          .Where(animal => animal.Кличка.ToLower().Contains(filter))
                                          .ToList();

            EditAnimalCmb.ItemsSource = filteredAnimals;

            // Открываем список для отображения результатов фильтрации
            EditAnimalCmb.IsDropDownOpen = true;
        }
        private void EditEmplCmb_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string filter = EditEmplCmb.Text.ToLower();
            var filteredEmployees = _context.Сотрудники
                                           .Where(employee => employee.ФИО.ToLower().Contains(filter))
                                           .ToList();

            EditEmplCmb.ItemsSource = filteredEmployees;

            // Открываем список для отображения результатов фильтрации
            EditEmplCmb.IsDropDownOpen = true;
        }
        private void EditUslugaCmb_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string filter = EditUslugaCmb.Text.ToLower();
            var filteredServices = _context.Услуги
                                           .Where(service => service.Наименование.ToLower().Contains(filter))
                                           .ToList();

            EditUslugaCmb.ItemsSource = filteredServices;

            // Открываем список для отображения результатов фильтрации
            EditUslugaCmb.IsDropDownOpen = true;
        }
        private void EditItemCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sel = EditItemCmb.SelectedItem;
            if (sel == null)
            {
                return;
            }
            curItem = sel.ToString();
            string[] currentString = sel.ToString().Split();
            string FIO = string.Empty;
            for (int i = 1; i < currentString.Length; i++)
            {
                FIO += currentString[i] + " ";
            }

            DateTime selectedDateTime = DateTime.Parse(currentString[0]);

            var q = from item in _context.Журнал_посещений
                    join item1 in _context.Животные
                    on item.Код_животного equals item1.Код_животного
                    join item3 in _context.Оказание_услуг
                    on item.Код_номера_журнала equals item3.C__журнала_посещения
                    where item.Дата == selectedDateTime && item1.Клиенты.ФИО.Equals(FIO.Trim())
                    select new
                    {
                        Journal = item,
                        Animals = item1,
                        OkazUsl = item3
                    };

            var selectedItem = q.ToList().First();

            EditDate.SelectedDate = selectedItem.Journal.Дата;

            // Обновляем ItemsSource и устанавливаем выбранный элемент для EditAnimalCmb
            EditAnimalCmb.ItemsSource = _context.Животные.ToList();
            EditAnimalCmb.SelectedItem = selectedItem.Animals;

            // Устанавливаем выбранный элемент для EditEmplCmb
            EditEmplCmb.ItemsSource = _context.Сотрудники.ToList();
            EditEmplCmb.SelectedItem = selectedItem.OkazUsl.Сотрудники;

            // Устанавливаем выбранный элемент для EditUslugaCmb
            EditUslugaCmb.ItemsSource = _context.Услуги.ToList();
            EditUslugaCmb.SelectedItem = selectedItem.OkazUsl.Услуги;

            EditDiag.Text = selectedItem.OkazUsl.Диагноз;
            UpdateDataGrid();
        } 

        private void changeClick(object sender, RoutedEventArgs e)
        {           
            var sel = curItem;
            if (sel == null)
            {
                return;
            }

            if (MessageBox.Show("Вы действительно хотите изменить?", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    string[] currentString = sel.ToString().Split();
                    string FIO = string.Empty;
                    for (int i = 1; i < currentString.Length; i++)
                    {
                        FIO += currentString[i] + " ";
                    }

                    DateTime selectedDateTime = DateTime.Parse(currentString[0]);

                    string klich = (EditAnimalCmb.SelectedItem as Животные).Кличка;
                    int AnimalId = (EditAnimalCmb.SelectedItem as Животные).Код_животного;
                    Журнал_посещений curJur = _context.Журнал_посещений.FirstOrDefault(item => item.Дата == selectedDateTime && item.Животные.Клиенты.ФИО.Equals(FIO));
                    curJur.Дата = (DateTime)EditDate.SelectedDate;
                    curJur.Код_животного = AnimalId;

                    Оказание_услуг curOkaz = _context.Оказание_услуг.FirstOrDefault(item => item.C__журнала_посещения == curJur.Код_номера_журнала);
                    curOkaz.Код_сотрудника = _context.Сотрудники.FirstOrDefault(item => item.ФИО.Equals(EditEmplCmb.Text)).Код_сотрудника;
                    curOkaz.Код_услуги = _context.Услуги.FirstOrDefault(item => item.Наименование.Equals(EditUslugaCmb.Text)).Код_услуги;
                    curOkaz.Диагноз = EditDiag.Text;

                    _context.SaveChanges();
                    MessageBox.Show("Изменения успешно сохранены!");
                    EditItemCmb.SelectedIndex = -1;
                    EditAnimalCmb.SelectedIndex = -1;
                    EditEmplCmb.SelectedIndex = -1;
                    EditUslugaCmb.SelectedIndex = -1;
                    EditDiag.Clear();
                    EditDate.SelectedDate = null;
                    UpdateDataGrid();
                }
                catch (Exception)
                {
                    MessageBox.Show("Заполните поля правильно", "Ошибка");
                }
            }
        }
    

        private void deleteClick(object sender, RoutedEventArgs e)
        {          

            if (MessageBox.Show("Вы действительно хотите удалить?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    string[] currentString = curItem.Split();
                    string FIO = string.Empty;
                    for (int i = 1; i < currentString.Length; i++)
                    {
                        FIO += currentString[i] + " ";
                    }

                    DateTime selectedDateTime = DateTime.Parse(currentString[0]);

                    Журнал_посещений curJur = _context.Журнал_посещений.FirstOrDefault(item => item.Дата == selectedDateTime && item.Животные.Клиенты.ФИО.Equals(FIO));

                    IEnumerable<Использованы_медикаменты> del = _context.Использованы_медикаменты.Where(item => item.C__журнала_посещения.Equals(curJur.Код_номера_журнала));
                    _context.Использованы_медикаменты.RemoveRange(del);
                    _context.SaveChanges();

                    IEnumerable<Оказание_услуг> del1 = _context.Оказание_услуг.Where(item => item.C__журнала_посещения.Equals(curJur.Код_номера_журнала));
                    _context.Оказание_услуг.RemoveRange(del1);
                    _context.SaveChanges();

                    _context.Журнал_посещений.Remove(curJur);
                    _context.SaveChanges();

                    MessageBox.Show("Запись журнала посещений успешно удалена");
                    EditItemCmb.SelectedIndex = -1;
                    EditAnimalCmb.SelectedIndex = -1;
                    EditEmplCmb.SelectedIndex = -1;
                    EditUslugaCmb.SelectedIndex = -1;
                    EditDiag.Clear();
                    EditDate.SelectedDate = null;
                    UpdateDataGrid();
                }
                catch (Exception)
                {
                    MessageBox.Show("Выберите запись", "Ошибка");
                    return;
                }
            }
        }

        private void EditItemCmb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var q = from item in _context.Журнал_посещений
                    join item1 in _context.Животные
                    on item.Код_животного equals item1.Код_животного
                    join item3 in _context.Оказание_услуг
                    on item.Код_номера_журнала equals item3.C__журнала_посещения
                    where item1.Клиенты.ФИО.Contains(EditItemCmb.Text) || item.Дата.ToString().Contains(EditItemCmb.Text)
                    select new
                    {
                        Journal = item,
                        Animals = item1,
                        OkazUsl = item3
                    };
            
            EditItemCmb.ItemsSource = q.ToList().Select(item => item.Journal.Дата.ToShortDateString() + " " + item.Animals.Клиенты.ФИО);
            UpdateDataGrid();
        }

        private void CurClientCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //AnimalCmb
            var sel = CurClientCmb.SelectedItem;
            if (sel == null)
            {
                return;
            }
            AnimalCmb.ItemsSource = _context.Животные.ToList().Where(item => item.Владелец == (sel as Клиенты).Код_клиента);
            UpdateDataGrid();
        }

        
        /*private void cancellation_Click(object sender, RoutedEventArgs e)
        {
            Data.MainFrame.Content = new MembersPage();
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Сохранить?", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (Клиенты item in allPos.Items)
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
        
        private void addNewEmloyee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Клиенты newEmployee = new Клиенты()
                {
                    ФИО = Vladelec.Text,
                    Адрес = AddAddress.Text,
                    Телефон = AddTelephone.Text,
                    Email = AddEmail.Text,
                    Примечание = primichInput.Text
                };
                _context.Клиенты.Add(newEmployee);
                _context.SaveChanges();
                MessageBox.Show("Клиент успешно добавлен");
                Vladelec.Clear(); AddAddress.Clear(); AddTelephone.Clear(); AddEmail.Clear(); AddInfo.Clear();
                allPos.ItemsSource = _context.Клиенты.ToList();
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте правильность заполнения полей", "Error");
            }
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
                    MessageBox.Show("Выберите клиента", "Error");
                    return;
                }

                _context.SaveChanges();
                MessageBox.Show("Успешно сохранено!", "Уведомление");
                AllClientFIO.SelectedIndex = -1; adressInput.Clear(); TelInput.Clear(); emailInput.Clear(); primichInput.Clear();
                AllClientFIO.ItemsSource = _context.Клиенты.ToList();
                allPos.ItemsSource = _context.Клиенты.ToList();
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте правильность заполнения полей", "Error");
            }
        }
        private void RemoveEmployee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Клиенты SelectedEmployer = DataContext as Клиенты;
                if (SelectedEmployer is null)
                {
                    MessageBox.Show("Выберите клиента", "Error");
                    return;
                }

                _context.Клиенты.Remove(SelectedEmployer);
                _context.SaveChanges();
                MessageBox.Show("Успешно удалено!", "Уведомление");
                AllClientFIO.ItemsSource = _context.Клиенты.ToList();
                allPos.ItemsSource = _context.Клиенты.ToList();
                //EditDoljnost.SelectedIndex = -1;
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте правильность заполнения полей", "Error");
            }
        }

        private void AllClientFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            AllClientFIO.ItemsSource = _context.Клиенты.ToList().Where(item => item.ФИО.Contains(AllClientFIO.Text));
        }*/
        private void Otchet_Click(object sender, RoutedEventArgs e)
        {
            if (!Data.isWordWindowOpen)
            {
                Data.isWordWindowOpen = true;
                worddot ww = new worddot();
                ww.ShowDialog();
            }
        }
    }
}
