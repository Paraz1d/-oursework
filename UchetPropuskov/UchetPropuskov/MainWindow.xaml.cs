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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace UchetPropuskov
{
    public partial class MainWindow : Window
    {
        string cs = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public MainWindow() => InitializeComponent();

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT ПользовательID, Роль FROM Пользователи WHERE Логин=@l AND Пароль=@p", con);
                cmd.Parameters.AddWithValue("@l", tbLogin.Text);
                cmd.Parameters.AddWithValue("@p", tbPassword.Password);

                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int userId = reader.GetInt32(0);
                    string role = reader.GetString(1);

                    if (role == "Администратор")
                        new AdminWindow(userId).Show();
                    else
                        new TeacherWindow(userId).Show();

                    Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
        }
    }
}
