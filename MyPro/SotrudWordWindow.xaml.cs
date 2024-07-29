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
using Window = System.Windows.Window;
using Word = Microsoft.Office.Interop.Word;

namespace MyPro
{
    /// <summary>
    /// Логика взаимодействия для SotrudWordWindow.xaml
    /// </summary>
    public partial class SotrudWordWindow : Window
    {
        VetClinicEntities _context = new VetClinicEntities();
        public SotrudWordWindow()
        {
            InitializeComponent();

            //allSotrud.ItemsSource = _context.Сотрудники.ToList();
        }

        private void WordClick(object sender, RoutedEventArgs e)
        {
            var application = new Word.Application();

            Word.Document document = application.Documents.Add();

            //var CurEmpl = allSotrud.SelectedValue as Сотрудники;
            //var CurAnimal = allClientAnimals.SelectedItem as Животные;

            var query = from item in _context.Оказание_услуг
                        join item1 in _context.Услуги
                        on item.Код_услуги equals item1.Код_услуги
                        join item2 in _context.Сотрудники
                        on item.Код_сотрудника equals item2.Код_сотрудника
                        join item3 in _context.Журнал_посещений
                        on item.C__журнала_посещения equals item3.Код_номера_журнала
                        where /*item2.ФИО.Equals(CurEmpl.ФИО) &&*/ item3.Дата >= startDate.SelectedDate && item3.Дата <= endDate.SelectedDate
                        select new
                        {
                            item,
                            item1,
                            item2,
                            item3
                        };
            var grouppedQuery = query.ToList().GroupBy(item => item.item.Код_сотрудника);
            //var CurClient = query.ToList().First().item;
            //var CurAnimal = query.ToList().First().item1;

            Word.Paragraph headParagraph = document.Paragraphs.Add();
            Word.Range headRange = headParagraph.Range;
            headRange.Text = "Отчет по доходам клиники";
            headParagraph.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            headRange.InsertParagraphAfter();

            Word.Paragraph dateParagraph = document.Paragraphs.Add();
            Word.Range dateRange = dateParagraph.Range;
            dateRange.Text = $"с {startDate.SelectedDate.ToString().Split(' ')[0]} по {endDate.SelectedDate.ToString().Split(' ')[0]}";
            dateRange.InsertParagraphAfter();

            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table animalsTable = document.Tables.Add(tableRange, grouppedQuery.Count() + 1, 2);
            animalsTable.Borders.InsideLineStyle = animalsTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            animalsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            Word.Range cellRange;

            cellRange = animalsTable.Cell(1, 1).Range;
            cellRange.Text = "ФИО Врача";
            cellRange = animalsTable.Cell(1, 2).Range;
            cellRange.Text = "Сумма за выбранный период";

            cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

            int rowid = 2;
            double sum = 0;
            foreach (var Current in grouppedQuery)
            {
                Сотрудники curEmpl = _context.Сотрудники.FirstOrDefault(item => item.Код_сотрудника == Current.Key);
                cellRange = animalsTable.Cell(rowid, 1).Range;
                cellRange.Text = curEmpl.ФИО;

                cellRange = animalsTable.Cell(rowid, 2).Range;
                double curSum = curEmpl.Оказание_услуг.Sum(item => double.Parse(item.Услуги.Цена.Replace('.', ',')));
                cellRange.Text = curSum.ToString("##.## BYN");

                sum += curSum;
                rowid++;
            }

            Word.Paragraph noteParagraph = document.Paragraphs.Add();
            Word.Range noteRange = noteParagraph.Range;
            noteRange.Text = $"\nИтого: {sum.ToString("##.## BYN")}";
            noteParagraph.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
            noteRange.InsertParagraphAfter();

            application.Visible = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Data.isWordWindowOpen = false;
            this.Close();
        }
    }
}
