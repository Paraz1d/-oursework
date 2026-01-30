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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace UchetPropuskov
{
    public partial class ReportsWindow : Window
    {
        string cs = System.Configuration.ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;

        public ReportsWindow()
        {
            InitializeComponent();

            dpStart.SelectedDate = DateTime.Today;
            dpEnd.SelectedDate = DateTime.Today;
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            if (dpStart.SelectedDate == null || dpEnd.SelectedDate == null)
            {
                MessageBox.Show("Выберите даты");
                return;
            }

            SqlConnection con = new SqlConnection(cs);
            con.Open();

            SqlCommand cmd = new SqlCommand(
                "SELECT s.ФИО, COUNT(p.ПропускID) AS ВсегоПропусков " +
                "FROM Пропуски p " +
                "JOIN Студенты s ON p.СтудентID = s.СтудентID " +
                "WHERE p.Дата BETWEEN @start AND @end " +
                "GROUP BY s.ФИО", con);

            cmd.Parameters.AddWithValue("@start", dpStart.SelectedDate.Value);
            cmd.Parameters.AddWithValue("@end", dpEnd.SelectedDate.Value);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            dgReport.ItemsSource = dt.DefaultView;

            con.Close();
        }
    }
}
