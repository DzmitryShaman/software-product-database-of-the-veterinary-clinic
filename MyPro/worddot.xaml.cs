using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
using Window = System.Windows.Window;
using Word = Microsoft.Office.Interop.Word;

namespace MyPro
{
    /// <summary>
    /// Логика взаимодействия для worddot.xaml
    /// </summary>
    public partial class worddot : Window
    {
        VetClinicEntities _context = new VetClinicEntities();
        private List<Сотрудники> checkedYSL = new List<Сотрудники>();
        private Сотрудники selectedEmployee;
        private CollectionViewSource _сотрудникиViewSource;
        public worddot()
        {
            InitializeComponent();
            try
            {
                // Получаем список сотрудников из базы данных через Entity Framework
                var сотрудники = _context.Сотрудники.ToList();

                // Устанавливаем источник данных для ComboBox
                allVrach.ItemsSource = сотрудники;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }
        private void allVrach_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранного сотрудника
            selectedEmployee = allVrach.SelectedItem as Сотрудники;
        }
        private void СотрудникиViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is Сотрудники сотрудник)
            {
                if (string.IsNullOrEmpty(allVrach.Text))
                {
                    e.Accepted = true;
                    return;
                }

                // Фильтрация по ФИО
                e.Accepted = сотрудник.ФИО.ToLower().Contains(allVrach.Text.ToLower());
            }
        }
        private void allVrach_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Обновляем фильтр при изменении текста
            _сотрудникиViewSource.View.Refresh();
        }


        private void WordClick(object sender, RoutedEventArgs e)
        {
            var application = new Word.Application();

            Word.Document document = application.Documents.Add();

            // Проверка, что сотрудник выбран и даты установлены
            if (selectedEmployee == null)
            {
                MessageBox.Show("Сотрудник не выбран.");
                return;
            }

            if (startDate.SelectedDate == null)
            {
                MessageBox.Show("Начальная дата не установлена.");
                return;
            }

            if (endDate.SelectedDate == null)
            {
                MessageBox.Show("Конечная дата не установлена.");
                return;
            }

            // Получаем значения дат
            DateTime startDateValue = startDate.SelectedDate.Value;
            DateTime endDateValue = endDate.SelectedDate.Value;

            // Формируем запрос
            var query = from ou in _context.Оказание_услуг
                        join s in _context.Сотрудники on ou.Код_сотрудника equals s.Код_сотрудника
                        join u in _context.Услуги on ou.Код_услуги equals u.Код_услуги
                        join jp in _context.Журнал_посещений on ou.C__журнала_посещения equals jp.Код_номера_журнала
                        join z in _context.Животные on jp.Код_животного equals z.Код_животного
                        join k in _context.Клиенты on z.Владелец equals k.Код_клиента
                        where s.Код_сотрудника == selectedEmployee.Код_сотрудника
                              && jp.Дата >= startDateValue
                              && jp.Дата <= endDateValue
                        select new
                        {
                            СотрудникФИО = s.ФИО,
                            КлиентФИО = k.ФИО,
                            ДатаПосещения = jp.Дата,
                            НаименованиеУслуги = u.Наименование,
                            ЦенаУслуги = u.Цена
                        };

            var resultList = query.ToList();

            if (resultList.Count == 0)
            {
                MessageBox.Show("Нет данных для выбранного сотрудника и дат.");
                return;
            }

            // Заполняем документ
            Word.Paragraph headParagraph = document.Paragraphs.Add();
            Word.Range headRange = headParagraph.Range;
            headRange.Text = "Отчёт журнал посещений";
            headParagraph.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            headRange.InsertParagraphAfter();

            Word.Paragraph dateParagraph = document.Paragraphs.Add();
            Word.Range dateRange = dateParagraph.Range;
            dateRange.Text = $"с {startDateValue.ToString("dd.MM.yyyy")} по {endDateValue.ToString("dd.MM.yyyy")}";
            dateRange.InsertParagraphAfter();

            Word.Paragraph employeeParagraph = document.Paragraphs.Add();
            Word.Range employeeRange = employeeParagraph.Range;
            employeeRange.Text = $"Сотрудник: {selectedEmployee.ФИО}";
            employeeRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            employeeRange.InsertParagraphAfter();

            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table reportTable = document.Tables.Add(tableRange, resultList.Count + 2, 4);
            reportTable.Borders.InsideLineStyle = reportTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            reportTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            Word.Range cellRange;

            // Заголовки таблицы
            cellRange = reportTable.Cell(1, 1).Range;
            cellRange.Text = "ФИО Клиента";
            cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            cellRange.Bold = 1;

            cellRange = reportTable.Cell(1, 2).Range;
            cellRange.Text = "Дата посещения";
            cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            cellRange.Bold = 1;

            cellRange = reportTable.Cell(1, 3).Range;
            cellRange.Text = "Наименование услуги";
            cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            cellRange.Bold = 1;

            cellRange = reportTable.Cell(1, 4).Range;
            cellRange.Text = "Цена услуги";
            cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            cellRange.Bold = 1;

            // Заполнение данных таблицы
            decimal totalSum = 0;
            int rowId = 2;
            foreach (var item in resultList)
            {
                // Заполняем ФИО клиента
                cellRange = reportTable.Cell(rowId, 1).Range;
                cellRange.Text = item.КлиентФИО;
                cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                // Заполняем дату посещения
                cellRange = reportTable.Cell(rowId, 2).Range;
                cellRange.Text = item.ДатаПосещения.ToString("dd.MM.yyyy");
                cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                // Заполняем наименование услуги
                cellRange = reportTable.Cell(rowId, 3).Range;
                cellRange.Text = item.НаименованиеУслуги;
                cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                // Заполняем цену услуги
                cellRange = reportTable.Cell(rowId, 4).Range;
                cellRange.Text = item.ЦенаУслуги;
                cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                // Parse the price and add to total sum
                if (decimal.TryParse(item.ЦенаУслуги, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                {
                    totalSum += price;
                }
                rowId++;
            }

            // Добавляем строку с общей суммой
            cellRange = reportTable.Cell(rowId, 3).Range;
            cellRange.Text = "Общая сумма:";
            cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
            cellRange.Bold = 1;

            cellRange = reportTable.Cell(rowId, 4).Range;
            cellRange.Text = totalSum.ToString("F2");
            cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            cellRange.Bold = 1;

            application.Visible = true;
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            Data.isWordWindowOpen = false;
            this.Close();
        }
    }
}
