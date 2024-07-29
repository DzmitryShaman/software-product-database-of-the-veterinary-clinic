using Microsoft.Office.Interop.Word;
using MyPro.Frames;
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
    /// Логика взаимодействия для WordWindow.xaml
    /// </summary>
    public partial class WordWindow : Window
    {
        VetClinicEntities _context = new VetClinicEntities();
        public WordWindow()
        {
            InitializeComponent();

            allClients.ItemsSource = _context.Клиенты.ToList();
        }

        private void allClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (allClients.SelectedItem is null)
            {
                return;
            }
            int curId = (allClients.SelectedItem as Клиенты).Код_клиента;
            allClientAnimals.ItemsSource = _context.Животные.Where(item => item.Владелец == curId).ToList();
        }

        private void WordClick(object sender, RoutedEventArgs e)
        {
            if (allClientAnimals.SelectedItem is null || allClientAnimals.SelectedItem is null)
            {
                return;
            }

            var application = new Word.Application();

            Word.Document document = application.Documents.Add();

            var CurClient = allClients.SelectedItem as Клиенты;
            var CurAnimal = allClientAnimals.SelectedItem as Животные;

            var query = from item in _context.Клиенты
                        join item1 in _context.Животные
                        on item.Код_клиента equals item1.Владелец
                        join item2 in _context.Журнал_посещений
                        on item1.Код_животного equals item2.Код_животного
                        join item3 in _context.Оказание_услуг
                        on item2.Код_номера_журнала equals item3.C__журнала_посещения
                        join item4 in _context.Использованы_медикаменты
                        on item2.Код_номера_журнала equals item4.C__журнала_посещения
                        where item.Код_клиента == CurClient.Код_клиента && item1.Код_животного == CurAnimal.Код_животного && item2.Дата >= startDate.SelectedDate && item2.Дата <= endDate. SelectedDate
                        select new
                        {
                            item,
                            item1,
                            item2,
                            item3,
                            item4
                        };
            var qq = query.ToList();
            //var CurClient = query.ToList().First().item;
            //var CurAnimal = query.ToList().First().item1;

            Word.Paragraph titleParagraph = document.Paragraphs.Add();
            Word.Range titleRange = titleParagraph.Range;
            titleRange.Text = $"Владелец {CurClient.ФИО}\nПитомец (Кличка): {CurAnimal.Кличка}\nАдрес проживания: {CurClient.Адрес}\nКонтактный телефон: {CurClient.Телефон}";
            titleRange.InsertParagraphAfter();

            Word.Paragraph headParagraph = document.Paragraphs.Add();
            Word.Range headRange = headParagraph.Range;
            headRange.Text = $"АКТ ОБСЛЕДОВАНИЯ";
            headParagraph.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            headRange.InsertParagraphAfter();

            Word.Paragraph dateParagraph = document.Paragraphs.Add();
            Word.Range dateRange = dateParagraph.Range;
            dateRange.Text = $"Дата обследования: с {startDate.SelectedDate.ToString().Split(' ')[0]} по {endDate.SelectedDate.ToString().Split(' ')[0]}";
            dateParagraph.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
            dateRange.InsertParagraphAfter();

            Word.Paragraph tableParagraph = document.Paragraphs.Add();
            Word.Range tableRange = tableParagraph.Range;
            Word.Table animalsTable = document.Tables.Add(tableRange, query.ToList().Count() + 1, 4);
            animalsTable.Borders.InsideLineStyle = animalsTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            animalsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

            Word.Range cellRange;

            cellRange = animalsTable.Cell(1, 1).Range;
            cellRange.Text = "Врач";
            cellRange = animalsTable.Cell(1, 2).Range;
            cellRange.Text = "Услуга";
            cellRange = animalsTable.Cell(1, 3).Range;
            cellRange.Text = "Диагноз";
            cellRange = animalsTable.Cell(1, 4).Range;
            cellRange.Text = "Препараты";

            cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

            int rowid = 2;
            foreach (var Current in query.ToList())
            {
                cellRange = animalsTable.Cell(rowid, 1).Range;
                cellRange.Text = Current.item3.Сотрудники.ФИО;

                cellRange = animalsTable.Cell(rowid, 2).Range;
                cellRange.Text = Current.item3.Услуги.Наименование;

                cellRange = animalsTable.Cell(rowid, 3).Range;
                cellRange.Text = Current.item3.Диагноз;

                cellRange = animalsTable.Cell(rowid, 4).Range;
                //cellRange.Text = _context.Медикаменты.Наименование;
                cellRange.Text = Current.item4.Медикаменты.Наименование;
                rowid++;
            }

            Word.Paragraph noteParagraph = document.Paragraphs.Add();
            Word.Range noteRange = noteParagraph.Range;
            noteRange.Text = $"Рекомендации:";
            noteParagraph.Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
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
