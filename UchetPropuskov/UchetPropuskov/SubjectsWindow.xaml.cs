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
    public partial class SubjectsWindow : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public SubjectsWindow()
        {
            InitializeComponent();
            LoadSubjects();
        }

        void LoadSubjects()
        {
            lbSubjects.Items.Clear();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Название FROM Предметы", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lbSubjects.Items.Add(reader.GetString(0));
                }
            }
        }

        private void AddSubject_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbSubject.Text)) return;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Предметы(Название) VALUES(@n)", con))
                {
                    cmd.Parameters.AddWithValue("@n", tbSubject.Text);
                    cmd.ExecuteNonQuery();
                }
            }

            tbSubject.Clear();
            LoadSubjects();
        }
    }
}
