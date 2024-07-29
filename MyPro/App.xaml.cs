using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyPro
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        
    }

    public static class Data
    {
        public static Сотрудники CurrentUser { get; set; }
        public static Frame MainFrame { get; set; }
        public static bool isWordWindowOpen = false;
    }
}
