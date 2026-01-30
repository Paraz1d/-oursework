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
    public partial class UsersWindow : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public UsersWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        void LoadUsers()
        {
            lbUsers.Items.Clear();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT ФИО, Роль FROM Пользователи", con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        lbUsers.Items.Add(reader.GetString(0) + " (" + reader.GetString(1) + ")");
                }
            }
        }

        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbFio.Text) ||
                string.IsNullOrWhiteSpace(tbLogin.Text) ||
                cbRole.SelectedItem == null)
                return;

            string role = ((ComboBoxItem)cbRole.SelectedItem).Content.ToString();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Пользователи(ФИО, Логин, Пароль, Роль) VALUES(@f,@l,@p,@r)", con))
                {
                    cmd.Parameters.AddWithValue("@f", tbFio.Text);
                    cmd.Parameters.AddWithValue("@l", tbLogin.Text);
                    cmd.Parameters.AddWithValue("@p", tbPass.Password);
                    cmd.Parameters.AddWithValue("@r", role);
                    cmd.ExecuteNonQuery();
                }
            }

            tbFio.Clear();
            tbLogin.Clear();
            tbPass.Clear();
            LoadUsers();
        }
    }
}
