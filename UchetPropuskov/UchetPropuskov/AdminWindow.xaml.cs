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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace UchetPropuskov
{
    public partial class AdminWindow : Window
    {
        int userId;

        public AdminWindow(int uid)
        {
            InitializeComponent();
            userId = uid;
        }

        private void BtnGroups_Click(object sender, RoutedEventArgs e)
        {
            new GroupsWindow().ShowDialog();
        }

        private void BtnStudents_Click(object sender, RoutedEventArgs e)
        {
            new StudentsWindow().ShowDialog();
        }

        private void BtnSubjects_Click(object sender, RoutedEventArgs e)
        {
            new SubjectsWindow().ShowDialog();
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            new UsersWindow().ShowDialog();
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            new ReportsWindow().ShowDialog();
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            new ImportWindow().ShowDialog();
        }
    }
}
