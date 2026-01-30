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
using Microsoft.Win32;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;


namespace UchetPropuskov
{
    public partial class ImportWindow : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        string filePath;

        public ImportWindow()
        {
            InitializeComponent();
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "CSV файлы (*.csv)|*.csv";

            if (dlg.ShowDialog() == true)
            {
                filePath = dlg.FileName;
                tbFile.Text = filePath;
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Выберите файл");
                return;
            }

            if (!rbGroups.IsChecked.Value &&
                !rbStudents.IsChecked.Value &&
                !rbSubjects.IsChecked.Value &&
                !rbUsers.IsChecked.Value)
            {
                MessageBox.Show("Выберите тип данных");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    if (rbGroups.IsChecked.Value)
                        ImportGroup(con, line);

                    else if (rbSubjects.IsChecked.Value)
                        ImportSubject(con, line);

                    else if (rbStudents.IsChecked.Value)
                        ImportStudent(con, line);

                    else if (rbUsers.IsChecked.Value)
                        ImportUser(con, line);
                }
            }

            MessageBox.Show("Импорт завершён");
            Close();
        }

        void ImportGroup(SqlConnection con, string name)
        {
            SqlCommand cmd = new SqlCommand(
                "INSERT INTO Группы(Название) VALUES(@n)", con);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.ExecuteNonQuery();
        }

        void ImportSubject(SqlConnection con, string name)
        {
            SqlCommand cmd = new SqlCommand(
                "INSERT INTO Предметы(Название) VALUES(@n)", con);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.ExecuteNonQuery();
        }

        void ImportStudent(SqlConnection con, string line)
        {
            string[] p = line.Split(';');
            if (p.Length < 2) return;

            SqlCommand cmd = new SqlCommand(
                "INSERT INTO Студенты(ФИО, ГруппаID) VALUES(@f,@g)", con);
            cmd.Parameters.AddWithValue("@f", p[0]);
            cmd.Parameters.AddWithValue("@g", int.Parse(p[1]));
            cmd.ExecuteNonQuery();
        }

        void ImportUser(SqlConnection con, string line)
        {
            string[] p = line.Split(';');
            if (p.Length < 4) return;

            SqlCommand cmd = new SqlCommand(
                "INSERT INTO Пользователи(ФИО, Логин, Пароль, Роль) VALUES(@f,@l,@p,@r)", con);
            cmd.Parameters.AddWithValue("@f", p[0]);
            cmd.Parameters.AddWithValue("@l", p[1]);
            cmd.Parameters.AddWithValue("@p", p[2]);
            cmd.Parameters.AddWithValue("@r", p[3]);
            cmd.ExecuteNonQuery();
        }
    }
}
