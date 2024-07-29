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
    /// Логика взаимодействия для MainJivotnie.xaml
    /// </summary>
    public partial class MainJivotnie : Page
    {
        VetClinicEntities _context = new VetClinicEntities();
        private List<Животные> checkedEmploies = new List<Животные>();
        
        public MainJivotnie()
        {
            InitializeComponent();
            allSotrud.ItemsSource = _context.Животные.ToList();
            AddVladelec.ItemsSource = _context.Клиенты.ToList();
            AddPoroda.ItemsSource = _context.Породы.ToList();

            porodaInput.ItemsSource = _context.Породы.ToList();
            ownerInput.ItemsSource = _context.Клиенты.ToList();
            UpdateDataGrid();
            searchQueryAnimals();
        }

        private void searchQueryAnimals(string searchTxt)
        {
            AllSotrFIO.ItemsSource = null;
            var query = from item in _context.Клиенты
                        join item1 in _context.Животные
                        on item.Код_клиента equals item1.Владелец
                        where item.ФИО.Contains(searchTxt)
                        select new
                        {
                            item,
                            item1,
                            qqq = item.ФИО + " " + item1.Кличка + " " + item1.Пол
                        };
            AllSotrFIO.ItemsSource = query.ToList();
        }

        private void searchQueryAnimals()
        {
            AllSotrFIO.ItemsSource = null;
            var query = from item in _context.Клиенты
                        join item1 in _context.Животные
                        on item.Код_клиента equals item1.Владелец
                        select new
                        {
                            item,
                            item1,
                            qqq = item.ФИО + " " + item1.Кличка + " " + item1.Пол
                        };
            AllSotrFIO.ItemsSource = query.ToList();
        }

        private void cancellation_Click(object sender, RoutedEventArgs e)
        {
            Data.MainFrame.Content = new MembersPage();
        }
        private void textBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = textBoxFilter.Text;
            //var filteredSotrudniki = _context.Сотрудники.Where(s => s.ФИО.Contains(searchQuery)).ToList();

            // Пример:
            var startIndex = (currentPage - 1) * itemsPerPage;
            var sortedData = _context.Животные.Where(s => s.Клиенты.ФИО.Contains(searchQuery)).ToList();
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            allSotrud.ItemsSource = pageData;
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Сохранить?", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    foreach (Животные item in allSotrud.Items)
                    {
                        Животные CurrentUser = _context.Животные.FirstOrDefault(item1 => item1.Код_животного == item.Код_животного);
                        CurrentUser.Владелец = item.Владелец;
                        CurrentUser.Порода = item.Порода;
                        CurrentUser.Кличка = item.Кличка;
                        CurrentUser.Пол = item.Пол;
                        CurrentUser.Дата_рождения = item.Дата_рождения;
                        CurrentUser.Окрас = item.Окрас;
                        CurrentUser.Информация = item.Информация;
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

            var startIndex = (currentPage - 1) * itemsPerPage;
            //var pageData = _context.Должности.Skip(startIndex).Take(itemsPerPage).ToList();
            var sortedData = _context.Животные.OrderBy(d => d.Код_животного);
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

        private void AddVladelec_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string filter = AddVladelec.Text.ToLower();
            var filteredOwners = _context.Клиенты
                                         .Where(owner => owner.ФИО.ToLower().Contains(filter))
                                         .ToList();

            AddVladelec.ItemsSource = filteredOwners;
            AddVladelec.IsDropDownOpen = true;
        }

        private void AddPoroda_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string filter = AddPoroda.Text.ToLower();
            var filteredBreeds = _context.Породы
                                        .Where(breed => breed.Наименование_породы.ToLower().Contains(filter))
                                        .ToList();

            AddPoroda.ItemsSource = filteredBreeds;
            AddPoroda.IsDropDownOpen = true;
        }
        private void addNewEmloyee_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверка заполнения обязательных полей
                if (string.IsNullOrWhiteSpace(AddVladelec.Text) ||
                    string.IsNullOrWhiteSpace(AddPoroda.Text) ||
                    string.IsNullOrWhiteSpace(AddKlichka.Text) ||
                    string.IsNullOrWhiteSpace(AddPol.Text) ||
                    string.IsNullOrWhiteSpace(AddData_roj.Text) ||
                    string.IsNullOrWhiteSpace(AddOkras.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля", "Ошибка");
                    return;
                }

                // Получение кода владельца
                int vladelecId = _context.Клиенты.FirstOrDefault(item => item.ФИО.Contains(AddVladelec.Text))?.Код_клиента ?? -1;

                // Получение кода породы
                int porodaId = _context.Породы.FirstOrDefault(item => item.Наименование_породы == AddPoroda.Text)?.Код_породы ?? -1;

                // Проверка, существует ли животное с таким же владельцем, породой и кличкой
                string klichka = AddKlichka.Text;
                if (_context.Животные.Any(z => z.Кличка == klichka && z.Владелец == vladelecId && z.Порода == porodaId))
                {
                    MessageBox.Show("Владелец с таким животным уже существует", "Ошибка");
                    return;
                }

                Животные newAnimal = new Животные()
                {
                    Владелец = vladelecId,
                    Порода = porodaId,
                    Кличка = AddKlichka.Text,
                    Пол = AddPol.Text,
                    Дата_рождения = DateTime.Parse(AddData_roj.Text),
                    Окрас = AddOkras.Text,
                    Информация = AddInfo.Text
                };
                _context.Животные.Add(newAnimal);
                _context.SaveChanges();
                MessageBox.Show("Животное успешно добавлено", "Уведомление");
                AddVladelec.SelectedIndex = -1; AddPoroda.SelectedIndex = -1; AddKlichka.Clear(); AddPol.SelectedIndex = -1; AddOkras.Clear(); AddInfo.Clear(); AddData_roj.SelectedDate = null;
                allSotrud.ItemsSource = _context.Животные.ToList();
                UpdateDataGrid();
                searchQueryAnimals();
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте правильность заполнения полей", "Ошибка");
            }
        }

        private void AllSotrFIO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var q = AllSotrFIO.SelectedItem as dynamic;
            if (q is null)
            {
                return;
            }
            //DataContext = q.item1;
            Животные curJiv = q.item1 as Животные;
            DataContext = curJiv.Код_животного;
            KlichInput.Text = curJiv.Кличка;
            porodaInput.Text = curJiv.Породы.Наименование_породы;
            ownerInput.Text = curJiv.Клиенты.ФИО;
            polInput.Text = curJiv.Пол;
            dataInput.Text = curJiv.Дата_рождения.ToShortDateString();
            OkrasInput.Text = curJiv.Окрас;
            PrimichInput.Text = curJiv.Информация;

            //porodaInput.Text = _context.Породы.FirstOrDefault(item => item.Код_породы == (DataContext as Животные).Породы.Код_породы).Наименование_породы;
            //polInput.Text = (q.item1 as Животные).Пол;
            //EditDoljnost.Text = (DataContext as Животные).Должности.Наименование_должности;           
        }
        private void porodaInput_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string filter = porodaInput.Text.ToLower();
            var filteredBreeds = _context.Породы
                                        .Where(breed => breed.Наименование_породы.ToLower().Contains(filter))
                                        .ToList();

            porodaInput.ItemsSource = filteredBreeds;
            porodaInput.IsDropDownOpen = true;
        }

        private void ownerInput_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string filter = ownerInput.Text.ToLower();
            var filteredOwners = _context.Клиенты
                                        .Where(owner => owner.ФИО.ToLower().Contains(filter))
                                        .ToList();

            ownerInput.ItemsSource = filteredOwners;
            ownerInput.IsDropDownOpen = true;
        }
        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите изменить?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Животные SelectedEmployer = _context.Животные.ToList().FirstOrDefault(item => item.Код_животного == int.Parse(DataContext.ToString()));
                    if (SelectedEmployer is null)
                    {
                        MessageBox.Show("Выберите владельца", "Error");
                        return;
                    }

                    SelectedEmployer.Порода = _context.Породы.FirstOrDefault(item => item.Наименование_породы == porodaInput.Text).Код_породы;
                    SelectedEmployer.Кличка = KlichInput.Text;
                    SelectedEmployer.Владелец = _context.Клиенты.FirstOrDefault(item => item.ФИО == ownerInput.Text).Код_клиента;
                    SelectedEmployer.Пол = polInput.Text;
                    SelectedEmployer.Дата_рождения = DateTime.Parse(dataInput.Text);
                    SelectedEmployer.Окрас = OkrasInput.Text;
                    SelectedEmployer.Информация = PrimichInput.Text;

                    _context.SaveChanges();
                    MessageBox.Show("Изменения успешно сохранено!", "Уведомление");
                    AllSotrFIO.Text = ""; porodaInput.SelectedIndex = -1; KlichInput.Clear(); ownerInput.SelectedIndex = -1; polInput.SelectedIndex = -1; OkrasInput.Clear(); PrimichInput.Clear(); AddData_roj.SelectedDate = null; dataInput.SelectedDate = null;
                    searchQueryAnimals();
                    allSotrud.ItemsSource = _context.Животные.ToList();
                    UpdateDataGrid();
                }
                catch (Exception)
                {
                    MessageBox.Show("Проверьте правильность заполнения полей", "Error");
                }
            }
        }

        private void RemoveEmployee_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Животные SelectedEmployer = _context.Животные.ToList().FirstOrDefault(item => item.Код_животного == int.Parse(DataContext.ToString()));
                    if (SelectedEmployer is null)
                    {
                        MessageBox.Show("Выберите владельца", "Error");
                        return;
                    }

                    IEnumerable<Журнал_посещений> curJur = _context.Журнал_посещений.ToList().Where(item => item.Код_животного == SelectedEmployer.Код_животного);
                    foreach (var item in curJur)
                    {
                        Оказание_услуг curOkaz = _context.Оказание_услуг.ToList().FirstOrDefault(a => a.C__журнала_посещения == item.Код_номера_журнала);
                        if (curOkaz != null)
                        {
                            _context.Оказание_услуг.Remove(curOkaz);
                            _context.SaveChanges();
                        }
                    }
                    if (curJur.Count() != 0)
                    {
                        _context.Журнал_посещений.RemoveRange(curJur);
                        _context.SaveChanges();
                    }

                    _context.Животные.Remove(SelectedEmployer);
                    _context.SaveChanges();
                    MessageBox.Show("Животное успешно удалено!", "Уведомление");
                    AllSotrFIO.SelectedIndex = -1; porodaInput.SelectedIndex = -1; KlichInput.Clear(); ownerInput.SelectedIndex = -1; polInput.SelectedIndex = -1; OkrasInput.Clear(); PrimichInput.Clear(); AddData_roj.SelectedDate = null; dataInput.SelectedDate = null;
                    searchQueryAnimals();
                    UpdateDataGrid();
                    AllSotrFIO.Text = "";
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

        private void AllSotrFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchQueryAnimals(AllSotrFIO.Text);
        }

        private void Otchet_Click(object sender, RoutedEventArgs e)
        {
            if (!Data.isWordWindowOpen)
            {
                Data.isWordWindowOpen = true;
                WordWindow ww = new WordWindow();
                ww.ShowDialog();
            }
        }
    }
}
