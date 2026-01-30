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
    public partial class GroupsWindow : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public GroupsWindow()
        {
            InitializeComponent();
            LoadGroups();
        }

        void LoadGroups()
        {
            lbGroups.Items.Clear();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT Название FROM Группы", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lbGroups.Items.Add(reader.GetString(0));
                }
            }
        }

        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbGroupName.Text)) return;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Группы(Название) VALUES(@n)", con))
                {
                    cmd.Parameters.AddWithValue("@n", tbGroupName.Text);
                    cmd.ExecuteNonQuery();
                }
            }

            tbGroupName.Clear();
            LoadGroups();
        }
    }
}
