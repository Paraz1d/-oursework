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
using System.Data.SqlClient;
using System.Configuration;


namespace UchetPropuskov
{
    public partial class StudentsWindow : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public StudentsWindow()
        {
            InitializeComponent();
            LoadGroups();
            LoadStudents();
        }

        void LoadGroups()
        {
            cbGroups.Items.Clear();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT ГруппаID, Название FROM Группы", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cbGroups.Items.Add(new
                        {
                            ГруппаID = reader.GetInt32(0),
                            Название = reader.GetString(1)
                        });
                    }
                }
            }
        }

        void LoadStudents()
        {
            lbStudents.Items.Clear();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT s.ФИО, g.Название FROM Студенты s JOIN Группы g ON s.ГруппаID=g.ГруппаID", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lbStudents.Items.Add(reader.GetString(0) + " (" + reader.GetString(1) + ")");
                }
            }
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbFio.Text) || cbGroups.SelectedItem == null) return;

            dynamic g = cbGroups.SelectedItem;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Студенты(ФИО, ГруппаID) VALUES(@f,@g)", con))
                {
                    cmd.Parameters.AddWithValue("@f", tbFio.Text);
                    cmd.Parameters.AddWithValue("@g", g.ГруппаID);
                    cmd.ExecuteNonQuery();
                }
            }

            tbFio.Clear();
            LoadStudents();
        }
    }
}
