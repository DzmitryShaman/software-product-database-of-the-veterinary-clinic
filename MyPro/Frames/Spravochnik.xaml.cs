using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
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
using System.Xaml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace MyPro.Frames
{
    /// <summary>
    /// Логика взаимодействия для Spravochnik.xaml
    /// </summary>
    public partial class Spravochnik : Page
    {
        string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=Vet_clinic;Integrated Security=True";
        private SqlConnection sqlConnection;

        VetClinicEntities _context = new VetClinicEntities();
        private List<Услуги> checkedYSL = new List<Услуги>();
        public string[] tempDate { get; set; }
        public Spravochnik()
        {
            InitializeComponent();
            UpdateDataGrid();
            UpdateDataGrid1();
            sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            LoadServices();
            Spravoch1.ItemsSource = _context.Услуги.ToList();
            Spravoch2.ItemsSource = _context.Медикаменты.ToList();
            Spravoch3.ItemsSource = _context.Должности.ToList();
            Spravoch4.ItemsSource = _context.Породы.ToList();
            ViborYS.ItemsSource = _context.Услуги.ToList();

            ViborYS.ItemsSource = _context.Услуги.ToList();
            ViborYS.ItemsSource = ViborYS.ItemsSource;
        }
        private int currentPage = 1;
        private int totalPages = 9; // Общее количество страниц (настройте по вашим данным)
        private bool UpdateDataGridYsl1()
        {
            var startIndex = (currentPage - 1) * itemsPerPage;
            var sortedData = _context.Услуги.OrderBy(d => d.Код_услуги);
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            if (pageData.Count() == 0)
            {
                return false;
            }

            // Обновите ItemsSource вашего DataGrid
            Spravoch1.ItemsSource = pageData;
            return true;
        }
        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdateDataGridYsl1();

            }
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                if (!UpdateDataGridYsl1()) return;

            }
        }

        int itemsPerPage = 14; // Количество элементов на странице

        private ObservableCollection<Услуги> pageDataCollectionYsl = new ObservableCollection<Услуги>();
        private ObservableCollection<Должности> pageDataCollectionDol = new ObservableCollection<Должности>();
        private ObservableCollection<Медикаменты> pageDataCollectionMed = new ObservableCollection<Медикаменты>();
        private ObservableCollection<Породы> pageDataCollectionPorodi = new ObservableCollection<Породы>();
        private bool UpdateDataGridPorodi()
        {
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                // Запрос для получения данных
                SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Породы ORDER BY Код_породы", sqlConnection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count == 0)
                {
                    return false;
                }

                pageDataCollectionPorodi.Clear();

                foreach (DataRow row in dataTable.Rows)
                {
                    pageDataCollectionPorodi.Add(new Породы
                    {
                        Код_породы = (int)row["Код_породы"],
                        Наименование_вида = row["Наименование_вида"].ToString(),
                        Наименование_породы = row["Наименование_породы"].ToString()
                    });
                }

                // Привязка данных к DataGrid
                Spravoch1.ItemsSource = pageDataCollectionPorodi;
                Spravoch2.ItemsSource = pageDataCollectionPorodi;
                Spravoch3.ItemsSource = pageDataCollectionPorodi;
                Spravoch4.ItemsSource = pageDataCollectionPorodi;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении DataGrid: " + ex.Message);
                return false;
            }
            finally
            {
                if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }
        private bool UpdateDataGridYsl()
        {
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                // Запрос для получения данных
                SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Услуги ORDER BY Код_услуги", sqlConnection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count == 0)
                {
                    return false;
                }

                pageDataCollectionYsl.Clear();

                foreach (DataRow row in dataTable.Rows)
                {
                    pageDataCollectionYsl.Add(new Услуги
                    {
                        Код_услуги = (int)row["Код_услуги"],
                        Наименование = row["Наименование"].ToString(),
                        Цена = row["Цена"].ToString()
                    });
                }

                // Привязка данных к DataGrid
                Spravoch1.ItemsSource = pageDataCollectionYsl;
                Spravoch2.ItemsSource = pageDataCollectionYsl;
                Spravoch3.ItemsSource = pageDataCollectionYsl;
                Spravoch4.ItemsSource = pageDataCollectionYsl;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении DataGrid: " + ex.Message);
                return false;
            }
            finally
            {
                if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }
        private bool UpdateDataGridMed()
        {
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                // Запрос для получения данных
                SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Медикаменты ORDER BY Код_медикамента", sqlConnection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count == 0)
                {
                    return false;
                }

                pageDataCollectionMed.Clear();

                foreach (DataRow row in dataTable.Rows)
                {
                    pageDataCollectionMed.Add(new Медикаменты
                    {
                        Код_медикамента = (int)row["Код_медикамента"],
                        Наименование = row["Наименование"].ToString(),
                        Единицы_измерения = row["Единицы_измерения"].ToString(),
                        Описание = row["Описание"].ToString()
                    });
                }

                // Привязка данных к DataGrid
                Spravoch1.ItemsSource = pageDataCollectionMed;
                Spravoch2.ItemsSource = pageDataCollectionMed;
                Spravoch3.ItemsSource = pageDataCollectionMed;
                Spravoch4.ItemsSource = pageDataCollectionMed;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении DataGrid: " + ex.Message);
                return false;
            }
            finally
            {
                if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }
        private bool UpdateDataGrid()
        {
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                // Запрос для получения данных
                SqlDataAdapter dataAdapter = new SqlDataAdapter("SELECT * FROM Должности ORDER BY Код_должности", sqlConnection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count == 0)
                {
                    return false;
                }

                pageDataCollectionDol.Clear();

                foreach (DataRow row in dataTable.Rows)
                {
                    pageDataCollectionDol.Add(new Должности
                    {
                        Код_должности = (int)row["Код_должности"],
                        Наименование_должности = row["Наименование_должности"].ToString()
                    });
                }

                // Привязка данных к DataGrid
                Spravoch1.ItemsSource = pageDataCollectionDol;
                Spravoch2.ItemsSource = pageDataCollectionDol;
                Spravoch3.ItemsSource = pageDataCollectionDol;
                Spravoch4.ItemsSource = pageDataCollectionDol;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении DataGrid: " + ex.Message);
                return false;
            }
            finally
            {
                if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
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
                    UpdateDataGridYsl1();

                }
            }
        }
        private bool UpdateDataGridDol1()
        {
            var startIndex = (currentPage - 1) * itemsPerPage;
            var sortedData = _context.Должности.OrderBy(d => d.Код_должности);
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            if (pageData.Count() == 0)
            {
                return false;
            }

            // Обновите ItemsSource вашего DataGrid
            Spravoch3.ItemsSource = pageData;
            return true;
        }
        private void PageButton_Click3(object sender, RoutedEventArgs e)
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
                    UpdateDataGridDol1();

                }
            }
        }

        private void PrevPageButton_Click3(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdateDataGridDol1();

            }
        }

        private void NextPageButton_Click3(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                if (!UpdateDataGridDol1()) return;

            }
        }
        private bool UpdateDataGridpor1()
        {
            var startIndex = (currentPage - 1) * itemsPerPage;
            var sortedData = _context.Породы.OrderBy(d => d.Код_породы);
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            if (pageData.Count() == 0)
            {
                return false;
            }

            // Обновите ItemsSource вашего DataGrid
            Spravoch4.ItemsSource = pageData;
            return true;
        }
        private void PageButton_Click4(object sender, RoutedEventArgs e)
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
                    UpdateDataGridpor1();

                }
            }
        }

        private void PrevPageButton_Click4(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdateDataGridpor1();

            }
        }

        private void NextPageButton_Click4(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                if (!UpdateDataGridpor1()) return;

            }
        }
        private bool UpdateDataGrid1()
        {
            var startIndex = (currentPage - 1) * itemsPerPage;
            var sortedData = _context.Медикаменты.OrderBy(d => d.Код_медикамента);
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            if (pageData.Count() == 0)
            {
                return false;
            }

            // Обновите ItemsSource вашего DataGrid
            Spravoch2.ItemsSource = pageData;
            return true;
        }
        private void PageButton_Click1(object sender, RoutedEventArgs e)
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
                    UpdateDataGrid1();

                }
            }
        }

        private void PrevPageButton_Click1(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdateDataGrid1();

            }
        }

        private void NextPageButton_Click1(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                if (!UpdateDataGrid1()) return;

            }
        }
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is System.Windows.Controls.TabControl)
            {
                System.Windows.Controls.TabItem selectedTab = e.AddedItems[0] as System.Windows.Controls.TabItem;

                if (selectedTab == TabItem1)
                {
                    SearchInУслуги();
                }
                else if (selectedTab == TabItem2)
                {
                    SearchInМедикаменты();
                }
            }
        }

        private void textBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TabItem1.IsSelected)
            {
                SearchInУслуги();
            }
            else if (TabItem2.IsSelected)
            {
                SearchInМедикаменты();
            }
            else if (TabItem3.IsSelected)
            {
                SearchInДолжности();
            }
            else if (TabItem4.IsSelected)
            {
                SearchInПороды();
            }
        }

        private void SearchInУслуги()
        {
            string searchQuery = textBoxFilter.Text;

            var startIndex = (currentPage - 1) * itemsPerPage;

            var sortedData = _context.Услуги.Where(s => s.Наименование.Contains(searchQuery)).ToList();
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            Spravoch1.ItemsSource = pageData;
        }

        private void SearchInМедикаменты()
        {
            string searchQuery = textBoxFilter.Text;

            var startIndex = (currentPage - 1) * itemsPerPage;

            var sortedData = _context.Медикаменты.Where(s => s.Наименование.Contains(searchQuery)).ToList();
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            Spravoch2.ItemsSource = pageData;
        }
        private void SearchInДолжности()
        {
            string searchQuery = textBoxFilter.Text;

            var startIndex = (currentPage - 1) * itemsPerPage;

            var sortedData = _context.Должности.Where(s => s.Наименование_должности.Contains(searchQuery)).ToList();
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            Spravoch3.ItemsSource = pageData;
        }
        private void SearchInПороды()
        {
            string searchQuery = textBoxFilter.Text;

            var startIndex = (currentPage - 1) * itemsPerPage;

            var sortedData = _context.Породы.Where(s => s.Наименование_породы.Contains(searchQuery)).ToList();
            var pageData = sortedData.Skip(startIndex).Take(itemsPerPage).ToList();

            Spravoch4.ItemsSource = pageData;
        }

        private void addYSL_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AddNameYS.Text) || string.IsNullOrWhiteSpace(AddSenaYS.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка");
                return;
            }

            try
            {
                // Проверка на существование записи по Наименование
                var existingYSL = _context.Услуги
                    .FirstOrDefault(u => u.Наименование == AddNameYS.Text);

                if (existingYSL != null)
                {
                    MessageBox.Show("Услуга с таким наименованием уже существует", "Ошибка");
                    return;
                }

                Услуги newYSL = new Услуги()
                {
                    Наименование = AddNameYS.Text,
                    Цена = AddSenaYS.Text
                };
                _context.Услуги.Add(newYSL);
                _context.SaveChanges();
                MessageBox.Show("Услуга успешно добавлена");
                AddNameYS.Clear(); AddSenaYS.Clear();
                Spravoch1.ItemsSource = _context.Клиенты.ToList();
                ViborYS.ItemsSource = _context.Услуги.ToList();
                UpdateDataGridYsl();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Проверьте правильность заполнения полей. Ошибка: {ex.Message}", "Error");
            }
        }
        private void AllViborYS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, что подключение к базе данных установлено
            if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    // Создаем команду SQL для выбора всех наименований услуг из таблицы "Услуги"
                    SqlCommand command = new SqlCommand("SELECT Наименование FROM Услуги", sqlConnection);

                    // Выполняем команду и читаем результаты
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // Добавляем наименования услуг в ComboBox
                        ViborYS.Items.Add(reader.GetString(0));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке наименований услуг: " + ex.Message);
                }
                finally
                {
                    // Закрываем соединение
                    sqlConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("Подключение к базе данных не установлено.");
            }
        }
        private void AllViborYS_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ваш код для обработки выбора в ComboBox DropYs
            string selectedService = ViborYS.SelectedItem?.ToString();

        }
        private void LoadServices()
        {
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                vibor_ysl.SelectedIndex = -1;
                vibor_dol.SelectedIndex = -1; Namа.Clear();
                DropYs.Items.Clear();
                vibor_ysl.Items.Clear();
                z.Items.Clear();
                vibor_med.Items.Clear();
                Drмs.Items.Clear();
                vibor_dol.Items.Clear();
                Por.Items.Clear();
                vibor_por.Items.Clear();


                SqlCommand command = new SqlCommand("SELECT Наименование FROM Услуги", sqlConnection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    DropYs.Items.Add(reader.GetString(0));
                    vibor_ysl.Items.Add(reader.GetString(0));
                }

                reader.Close();

                SqlCommand command1 = new SqlCommand("SELECT Наименование FROM Медикаменты", sqlConnection);
                SqlDataReader reader1 = command1.ExecuteReader();

                while (reader1.Read())
                {
                    z.Items.Add(reader1.GetString(0));
                    vibor_med.Items.Add(reader1.GetString(0));
                }
                reader1.Close();

                SqlCommand command2 = new SqlCommand("SELECT Наименование_должности FROM Должности", sqlConnection);
                SqlDataReader reader2 = command2.ExecuteReader();

                while (reader2.Read())
                {
                    Drмs.Items.Add(reader2.GetString(0));
                    vibor_dol.Items.Add(reader2.GetString(0));
                }
                reader2.Close();

                SqlCommand command3 = new SqlCommand("SELECT Наименование_породы FROM Породы", sqlConnection);
                SqlDataReader reader3 = command3.ExecuteReader();

                while (reader3.Read())
                {
                    Por.Items.Add(reader3.GetString(0));
                    vibor_por.Items.Add(reader3.GetString(0));
                }
                reader3.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке наименований: " + ex.Message);
            }
            finally
            {
                // Закрытие соединения
                if (sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }

        private void DropYs_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Проверяем, что подключение к базе данных установлено
            if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    // Создаем команду SQL для выбора всех наименований услуг из таблицы "Услуги"
                    SqlCommand command = new SqlCommand("SELECT Наименование FROM Услуги", sqlConnection);

                    // Выполняем команду и читаем результаты
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // Добавляем наименования услуг в ComboBox
                        DropYs.Items.Add(reader.GetString(0));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке наименований услуг: " + ex.Message);
                }
                finally
                {
                    // Закрываем соединение
                    sqlConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("Подключение к базе данных не установлено.");
            }
        }

        private void DropYs_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ваш код для обработки выбора в ComboBox DropYs
            string selectedService = DropYs.SelectedItem?.ToString();

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранное наименование услуги
            string selectedService = DropYs.SelectedItem?.ToString();
            if (selectedService != null)
            {
                // Отображаем диалоговое окно с вопросом об удалении
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранную услугу?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Проверяем результат нажатия кнопки в диалоговом окне
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();

                        // Создание команды SQL для удаления выбранной услуги
                        SqlCommand command = new SqlCommand($"DELETE FROM Услуги WHERE Наименование = @Наименование", sqlConnection);
                        command.Parameters.AddWithValue("@Наименование", selectedService);

                        // Выполнение команды удаления
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Услуга успешно удалена.");

                            // Очистка ComboBox после удаления
                            DropYs.Items.Clear();
                            UpdateDataGridYsl();
                            // Обновляем список услуг после удаления
                            LoadServices();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить выбранную услугу.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении услуги: " + ex.Message);
                    }
                    finally
                    {
                        if (sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)
                        {
                            sqlConnection.Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите услугу для удаления.");
            }
        }

        private void addMed_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(q.Text) || string.IsNullOrWhiteSpace(w.Text) || string.IsNullOrWhiteSpace(wя.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены", "Ошибка");
                return;
            }

            try
            {
                // Проверка на существование записи по Наименование
                var existingMed = _context.Медикаменты
                    .FirstOrDefault(m => m.Наименование == q.Text);

                if (existingMed != null)
                {
                    MessageBox.Show("Медикамент с таким наименованием уже существует", "Ошибка");
                    return;
                }

                Медикаменты newMed = new Медикаменты()
                {
                    Наименование = q.Text,
                    Единицы_измерения = w.Text,
                    Описание = wя.Text
                };
                _context.Медикаменты.Add(newMed);
                _context.SaveChanges();
                MessageBox.Show("Медикамент успешно добавлен");
                q.Clear(); w.Clear(); wя.Clear();
                Spravoch2.ItemsSource = _context.Медикаменты.ToList();
                UpdateDataGridMed();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Проверьте правильность заполнения полей. Ошибка: {ex.Message}", "Ошибка");
            }
        }
        private void addDol_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Aе.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены", "Ошибка");
                return;
            }

            try
            {
                // Проверка на существование записи по Наименование_должности
                var existingDol = _context.Должности
                    .FirstOrDefault(d => d.Наименование_должности == Aе.Text);

                if (existingDol != null)
                {
                    MessageBox.Show("Должность с таким наименованием уже существует", "Ошибка");
                    return;
                }

                Должности newDol = new Должности()
                {
                    Наименование_должности = Aе.Text,
                };
                _context.Должности.Add(newDol);
                _context.SaveChanges();
                MessageBox.Show("Должность успешно добавлена");
                Aе.Clear();
                Spravoch3.ItemsSource = _context.Должности.ToList();
                UpdateDataGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Проверьте правильность заполнения полей. Ошибка: {ex.Message}", "Ошибка");
            }
        }
        private void add_Porodi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Adа.Text) || string.IsNullOrWhiteSpace(Addа.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены", "Ошибка");
                return;
            }

            try
            {
                // Проверка на существование записи по Наименование_породы
                var existingJiv = _context.Породы
                    .FirstOrDefault(j => j.Наименование_породы == Addа.Text);

                if (existingJiv != null)
                {
                    MessageBox.Show("Запись с таким Наименование породы уже существует", "Ошибка");
                    return;
                }

                Породы newJiv = new Породы()
                {
                    Наименование_вида = Adа.Text,
                    Наименование_породы = Addа.Text,
                };
                _context.Породы.Add(newJiv);
                _context.SaveChanges();
                MessageBox.Show("Порода успешно добавлена");
                Adа.Clear(); Addа.Clear();
                UpdateDataGridPorodi();
                Spravoch4.ItemsSource = _context.Породы.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Проверьте правильность заполнения полей. Ошибка: {ex.Message}", "Ошибка");
            }
        }
        private void DropMed_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Проверяем, что подключение к базе данных установлено
            if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    // Создаем команду SQL для выбора всех наименований услуг из таблицы "Услуги"
                    SqlCommand command = new SqlCommand("SELECT Наименование FROM Медикаменты", sqlConnection);

                    // Выполняем команду и читаем результаты
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // Добавляем наименования услуг в ComboBox
                        z.Items.Add(reader.GetString(0));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке наименований медикамента: " + ex.Message);
                }
                finally
                {
                    // Закрываем соединение
                    sqlConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("Подключение к базе данных не установлено.");
            }
        }
        private void DropMed_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ваш код для обработки выбора в ComboBox DropYs
            string selectedService = z.SelectedItem?.ToString();

        }
        private void Del_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранное наименование услуги
            string selectedService = z.SelectedItem?.ToString();
            if (selectedService != null)
            {
                // Отображаем диалоговое окно с вопросом об удалении
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранный медикамент?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Проверяем результат нажатия кнопки в диалоговом окне
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();

                        // Создание команды SQL для удаления выбранной услуги
                        SqlCommand command = new SqlCommand($"DELETE FROM Медикаменты WHERE Наименование = @Наименование", sqlConnection);
                        command.Parameters.AddWithValue("@Наименование", selectedService);

                        // Выполнение команды удаления
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Медикамент успешно удален.");

                            // Очистка ComboBox после удаления
                            DropYs.Items.Clear();
                            UpdateDataGridMed();
                            // Обновляем список услуг после удаления
                            LoadServices();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить выбранный медикамент.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении медикамента: " + ex.Message);
                    }
                    finally
                    {
                        if (sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)
                        {
                            sqlConnection.Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите услугу для удаления.");
            }
        }
        private void DropSpr_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Проверяем, что подключение к базе данных установлено
            if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    // Создаем команду SQL для выбора всех наименований услуг из таблицы "Услуги"
                    SqlCommand command = new SqlCommand("SELECT Наименование_должности FROM Должности", sqlConnection);

                    // Выполняем команду и читаем результаты
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // Добавляем наименования услуг в ComboBox
                        Drмs.Items.Add(reader.GetString(0));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке наименований должности: " + ex.Message);
                }
                finally
                {
                    // Закрываем соединение
                    sqlConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("Подключение к базе данных не установлено.");
            }
        }
        private void DropSpr_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ваш код для обработки выбора в ComboBox DropYs
            string selectedService = Drмs.SelectedItem?.ToString();

        }
        private void DropDol_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранное наименование услуги
            string selectedService = Drмs.SelectedItem?.ToString();
            if (selectedService != null)
            {
                // Отображаем диалоговое окно с вопросом об удалении
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранный должность?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Проверяем результат нажатия кнопки в диалоговом окне
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();

                        // Создание команды SQL для удаления выбранной услуги
                        SqlCommand command = new SqlCommand($"DELETE FROM Должности WHERE Наименование_должности = @Наименование_должности", sqlConnection);
                        command.Parameters.AddWithValue("@Наименование_должности", selectedService);

                        // Выполнение команды удаления
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Должность успешно удалена.");

                            // Очистка ComboBox после удаления
                            Drмs.Items.Clear();
                            UpdateDataGrid();
                            // Обновляем список услуг после удаления
                            LoadServices();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить выбранную должность.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении должности: " + ex.Message);
                    }
                    finally
                    {
                        if (sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)
                        {
                            sqlConnection.Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите должность для удаления.");
            }
        }
        private void DropPorod_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Проверяем, что подключение к базе данных установлено
            if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
            {
                try
                {
                    // Создаем команду SQL для выбора всех наименований услуг из таблицы "Услуги"
                    SqlCommand command = new SqlCommand("SELECT Наименование_породы FROM Породы", sqlConnection);

                    // Выполняем команду и читаем результаты
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // Добавляем наименования услуг в ComboBox
                        Por.Items.Add(reader.GetString(0));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке наименований породы: " + ex.Message);
                }
                finally
                {
                    // Закрываем соединение
                    sqlConnection.Close();
                }
            }
            else
            {
                MessageBox.Show("Подключение к базе данных не установлено.");
            }
        }
        private void DropPorod_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            // Ваш код для обработки выбора в ComboBox DropYs
            string selectedService = Por.SelectedItem?.ToString();
        }
        private void Drop_Porodi_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранное наименование услуги
            string selectedService = Por.SelectedItem?.ToString();
            if (selectedService != null)
            {
                // Отображаем диалоговое окно с вопросом об удалении
                MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранную породу?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Проверяем результат нажатия кнопки в диалоговом окне
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();

                        // Создание команды SQL для удаления выбранной услуги
                        SqlCommand command = new SqlCommand($"DELETE FROM Породы WHERE Наименование_породы = @Наименование_породы", sqlConnection);
                        command.Parameters.AddWithValue("@Наименование_породы", selectedService);

                        // Выполнение команды удаления
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Порода успешно удалена.");

                            // Очистка ComboBox после удаления
                            Por.Items.Clear();
                            UpdateDataGridPorodi();
                            // Обновляем список услуг после удаления
                            LoadServices();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось удалить выбранную породу.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при удалении породы: " + ex.Message);
                    }
                    finally
                    {
                        if (sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)
                        {
                            sqlConnection.Close();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите породы для удаления.");
            }
        }
        private void viborysl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (vibor_ysl.SelectedItem != null)
            {
                string selectedService = vibor_ysl.SelectedItem.ToString();

                try
                {
                    sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();

                    // Запрос для получения данных по выбранной услуге
                    SqlCommand command = new SqlCommand("SELECT Наименование, Цена FROM Услуги WHERE Наименование = @Наименование", sqlConnection);
                    command.Parameters.AddWithValue("@Наименование", selectedService);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        reNameYS.Text = reader["Наименование"].ToString();
                        reSenaYS.Text = reader["Цена"].ToString();
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных услуги: " + ex.Message);
                }
                finally
                {
                    if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }
        
        private void editYSL_Click(object sender, RoutedEventArgs e)
        {
            if (vibor_ysl.SelectedItem != null)
            {
                string selectedService = vibor_ysl.SelectedItem.ToString();
                string newName = reNameYS.Text;
                string newPrice = reSenaYS.Text;

                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();

                        // Запрос для обновления данных услуги
                        SqlCommand command = new SqlCommand("UPDATE Услуги SET Наименование = @NewName, Цена = @NewPrice WHERE Наименование = @SelectedService", sqlConnection);
                        command.Parameters.AddWithValue("@NewName", newName);
                        command.Parameters.AddWithValue("@NewPrice", newPrice);
                        command.Parameters.AddWithValue("@SelectedService", selectedService);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Услуга успешно изменина");
                            reNameYS.Clear(); reSenaYS.Clear();
                            // Обновление ComboBox и других данных, если необходимо
                            LoadServices();
                            // Выбираем обновленный элемент в ComboBox
                            vibor_ysl.SelectedItem = newName;
                            vibor_ysl.SelectedIndex = -1; reNameYS.Clear(); reSenaYS.Clear();
                            bool isUpdated = UpdateDataGridYsl();
                            if (!isUpdated)
                            {
                                MessageBox.Show("Ошибка при обновлении DataGrid: данные не найдены.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить данные");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
                    }
                    finally
                    {
                        if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                        {
                            sqlConnection.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Изменения отменены");
                }
            }
            else
            {
                MessageBox.Show("Выберите услугу для изменения.");
            }
        }
        private void vibormed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (vibor_med.SelectedItem != null)
            {
                string selectedService = vibor_med.SelectedItem.ToString();

                try
                {
                    sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();

                    // Запрос для получения данных по выбранной услуге
                    SqlCommand command = new SqlCommand("SELECT Наименование, Единицы_измерения, Описание FROM Медикаменты WHERE Наименование = @Наименование", sqlConnection);
                    command.Parameters.AddWithValue("@Наименование", selectedService);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        rename_med.Text = reader["Наименование"].ToString();
                        rename_ed.Text = reader["Единицы_измерения"].ToString();
                        rename_op.Text = reader["Описание"].ToString();
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных услуги: " + ex.Message);
                }
                finally
                {
                    if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }
        private void editMed_Click(object sender, RoutedEventArgs e)
        {
            if (vibor_med.SelectedItem != null)
            {
                string selectedService = vibor_med.SelectedItem.ToString();
                string newName1 = rename_med.Text;
                string newEd = rename_ed.Text;
                string newOp = rename_op.Text;

                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();

                        // Запрос для обновления данных услуги
                        SqlCommand command = new SqlCommand("UPDATE Медикаменты SET Наименование = @NewName1, Единицы_измерения = @NewEd, Описание = @NewOp WHERE Наименование = @SelectedService", sqlConnection);
                        command.Parameters.AddWithValue("@NewName1", newName1);
                        command.Parameters.AddWithValue("@NewEd", newEd);
                        command.Parameters.AddWithValue("@NewOp", newOp);
                        command.Parameters.AddWithValue("@SelectedService", selectedService);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные успешно обновлены");
                            rename_med.Clear(); rename_ed.Clear(); rename_op.Clear();
                            // Обновление ComboBox и других данных, если необходимо
                            LoadServices();
                            // Выбираем обновленный элемент в ComboBox
                            vibor_ysl.SelectedItem = newName1;
                            bool isUpdated = UpdateDataGridMed();
                            if (!isUpdated)
                            {
                                MessageBox.Show("Ошибка при обновлении DataGrid: данные не найдены.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить данные");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
                    }
                    finally
                    {
                        if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                        {
                            sqlConnection.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Изменения отменены");
                }
            }
            else
            {
                MessageBox.Show("Выберите медикамент для изменения.");
            }
        }
        private void vibordol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (vibor_dol.SelectedItem != null)
            {
                string selectedService = vibor_dol.SelectedItem.ToString();

                try
                {
                    sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();

                    // Запрос для получения данных по выбранной услуге
                    SqlCommand command = new SqlCommand("SELECT Наименование_должности FROM Должности WHERE Наименование_должности = @Наименование", sqlConnection);
                    command.Parameters.AddWithValue("@Наименование", selectedService);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        Namа.Text = reader["Наименование_должности"].ToString();
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных должности: " + ex.Message);
                }
                finally
                {
                    if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }
        private void editDol_Click(object sender, RoutedEventArgs e)
        {
            if (vibor_dol.SelectedItem != null)
            {
                string selectedService = vibor_dol.SelectedItem.ToString();
                string newName2 = Namа.Text;

                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();

                        // Запрос для обновления данных услуги
                        SqlCommand command = new SqlCommand("UPDATE Должности SET Наименование_должности = @NewName2 WHERE Наименование_должности = @SelectedService", sqlConnection);
                        command.Parameters.AddWithValue("@NewName2", newName2);
                        command.Parameters.AddWithValue("@SelectedService", selectedService);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные успешно обновлены");

                            // Обновление ComboBox и других данных, если необходимо
                            LoadServices();
                            // Выбираем обновленный элемент в ComboBox
                            vibor_dol.SelectedItem = newName2;
                            vibor_dol.SelectedIndex = -1; Namа.Clear();
                            bool isUpdated = UpdateDataGrid();
                            if (!isUpdated)
                            {
                                MessageBox.Show("Ошибка при обновлении DataGrid: данные не найдены.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить данные");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
                    }
                    finally
                    {
                        if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                        {
                            sqlConnection.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Изменения отменены");
                }
            }
            else
            {
                MessageBox.Show("Выберите должность для изменения.");
            }
        }
        private void viborpor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (vibor_por.SelectedItem != null)
            {
                string selectedService = vibor_por.SelectedItem.ToString();

                try
                {
                    sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();

                    // Запрос для получения данных по выбранной услуге
                    SqlCommand command = new SqlCommand("SELECT Наименование_вида, Наименование_породы FROM Породы WHERE Наименование_породы = @Наименование", sqlConnection);
                    command.Parameters.AddWithValue("@Наименование", selectedService);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        Naп.Text = reader["Наименование_вида"].ToString();
                        Seм.Text = reader["Наименование_породы"].ToString();
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при загрузке данных породы: " + ex.Message);
                }
                finally
                {
                    if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }

        private void edit_Porodi_Click(object sender, RoutedEventArgs e)
        {
            if (vibor_por.SelectedItem != null)
            {
                string selectedService = vibor_por.SelectedItem.ToString();
                string newName3 = Naп.Text;
                string newEd = Seм.Text;

                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите сохранить изменения?", "Подтверждение", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        sqlConnection = new SqlConnection(connectionString);
                        sqlConnection.Open();

                        // Запрос для обновления данных услуги
                        SqlCommand command = new SqlCommand("UPDATE Породы SET Наименование_вида = @NewName3, Наименование_породы = @Newpor WHERE Наименование_породы = @SelectedService", sqlConnection);
                        command.Parameters.AddWithValue("@NewName3", newName3);
                        command.Parameters.AddWithValue("@Newpor", newEd);
                        command.Parameters.AddWithValue("@SelectedService", selectedService);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Данные успешно обновлены");

                            // Обновление ComboBox и других данных, если необходимо
                            LoadServices();
                            vibor_por.SelectedItem = newEd;
                            vibor_por.SelectedIndex = -1; Naп.Clear(); Seм.Clear();
                            bool isUpdated = UpdateDataGridPorodi();
                            if (!isUpdated)
                            {
                                MessageBox.Show("Ошибка при обновлении DataGrid: данные не найдены.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить данные");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при обновлении данных: " + ex.Message);
                    }
                    finally
                    {
                        if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                        {
                            sqlConnection.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Изменения отменены");
                }
            }
            else
            {
                MessageBox.Show("Выберите породу для изменения.");
            }
        }
        private void vibor_por_TextChanged(object sender, EventArgs e)
        {
            string searchText = vibor_por.Text.Trim();

            // Очищаем элементы в ComboBox перед фильтрацией
            vibor_por.Items.Clear();

            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                // Запрос для фильтрации данных в ComboBox
                SqlCommand command = new SqlCommand("SELECT DISTINCT Наименование_породы FROM Породы WHERE Наименование_породы LIKE @SearchText", sqlConnection);
                command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    vibor_por.Items.Add(reader["Наименование_породы"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных породы: " + ex.Message);
            }
            finally
            {
                if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }
        private void vibor_dol_TextChanged(object sender, EventArgs e)
        {
            string searchText = vibor_dol.Text.Trim();

            // Очищаем элементы в ComboBox перед фильтрацией
            vibor_dol.Items.Clear();

            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                // Запрос для фильтрации данных в ComboBox
                SqlCommand command = new SqlCommand("SELECT DISTINCT Наименование_должности FROM Должности WHERE Наименование_должности LIKE @SearchText", sqlConnection);
                command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    vibor_dol.Items.Add(reader["Наименование_должности"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных должности: " + ex.Message);
            }
            finally
            {
                if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }
        private void vibor_med_TextChanged(object sender, EventArgs e)
        {
            string searchText = vibor_med.Text.Trim();

            // Очищаем элементы в ComboBox перед фильтрацией
            vibor_med.Items.Clear();

            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                // Запрос для фильтрации данных в ComboBox
                SqlCommand command = new SqlCommand("SELECT DISTINCT Наименование FROM Медикаменты WHERE Наименование LIKE @SearchText", sqlConnection);
                command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    vibor_med.Items.Add(reader["Наименование"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных медикаментов: " + ex.Message);
            }
            finally
            {
                if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }
        private void vibor_ysl_TextChanged(object sender, EventArgs e)
        {
            string searchText = vibor_ysl.Text.Trim();

            // Очищаем элементы в ComboBox перед фильтрацией
            vibor_ysl.Items.Clear();

            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();

                // Запрос для фильтрации данных в ComboBox
                SqlCommand command = new SqlCommand("SELECT DISTINCT Наименование FROM Услуги WHERE Наименование LIKE @SearchText", sqlConnection);
                command.Parameters.AddWithValue("@SearchText", "%" + searchText + "%");

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    vibor_ysl.Items.Add(reader["Наименование"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных услуг: " + ex.Message);
            }
            finally
            {
                if (sqlConnection != null && sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
            }
        }
    }
}
