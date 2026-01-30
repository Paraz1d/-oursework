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
using System.IO;

namespace UchetPropuskov
{
    public partial class TeacherWindow : Window
    {
        int userId;
        string cs = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        Dictionary<(int studentId, int pair), int> changedAbsences
    = new Dictionary<(int, int), int>();

        public TeacherWindow(int uid)
        {
            InitializeComponent();
            userId = uid;
            dpDate.SelectedDate = DateTime.Today;
            LoadGroups();
            LoadSubjects();
        }

        void LoadGroups()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT ГруппаID, Название FROM Группы", con);

                List<dynamic> list = new List<dynamic>();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                    list.Add(new { ГруппаID = r.GetInt32(0), Название = r.GetString(1) });

                cbGroups.ItemsSource = list;
            }
        }

        void LoadSubjects()
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "SELECT ПредметID, Название FROM Предметы", con);

                List<dynamic> list = new List<dynamic>();
                SqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                    list.Add(new { ПредметID = r.GetInt32(0), Название = r.GetString(1) });

                cbSubjects.ItemsSource = list;
            }
        }
        private void DateChanged(object sender, SelectionChangedEventArgs e)
        {
            changedAbsences.Clear();
            LoadJournal();
        }

        void LoadJournal()
        {
            if (cbGroups.SelectedValue == null ||
                cbSubjects.SelectedValue == null ||
                dpDate.SelectedDate == null)
                return;

            List<JournalRow> list = new List<JournalRow>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // 1. Студенты
                SqlCommand cmdStudents = new SqlCommand(
                    "SELECT СтудентID, ФИО FROM Студенты WHERE ГруппаID=@g", con);
                cmdStudents.Parameters.AddWithValue("@g", cbGroups.SelectedValue);

                SqlDataReader r = cmdStudents.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new JournalRow
                    {
                        СтудентID = r.GetInt32(0),
                        ФИО = r.GetString(1)
                    });
                }
                r.Close();

                // 2. Пропуски
                SqlCommand cmdAbs = new SqlCommand(@"
            SELECT СтудентID, НомерПары, ТипПропускаID
            FROM Пропуски
            WHERE ПредметID=@p AND Дата=@d", con);

                cmdAbs.Parameters.AddWithValue("@p", cbSubjects.SelectedValue);
                cmdAbs.Parameters.AddWithValue("@d", dpDate.SelectedDate.Value);

                SqlDataReader a = cmdAbs.ExecuteReader();
                while (a.Read())
                {
                    int sid = a.GetInt32(0);
                    int pair = a.GetByte(1);
                    int type = a.GetInt32(2);

                    string TypeToMark(int typeId)
                    {
                        if (typeId == 1) return "У";
                        if (typeId == 2) return "Н";
                        if (typeId == 3) return "Б";
                        return "";
                    }

                    JournalRow row = list.FirstOrDefault(x => x.СтудентID == sid);
                    if (row != null)
                        SetCell(row, pair, TypeToMark(type));
                }
            }

            dgJournal.ItemsSource = list;
        }


        private void FilterChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            LoadJournal();
        }

        private void dgJournal_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (cbSubjects.SelectedValue == null || dpDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите предмет и дату");
                return;
            }

            JournalRow row = dgJournal.SelectedItem as JournalRow;
            if (row == null) return;

            int pair = dgJournal.CurrentCell.Column.DisplayIndex;
            if (pair == 0) return;

            AbsenceTypeWindow win = new AbsenceTypeWindow();
            if (win.ShowDialog() == true)
            {
                SetCell(row, pair, win.SelectedMark);

                changedAbsences[(row.СтудентID, pair)] = win.SelectedTypeId;
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (changedAbsences.Count == 0)
            {
                MessageBox.Show("Нет изменений");
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                foreach (var item in changedAbsences)
                {
                    SqlCommand cmd = new SqlCommand(
                        "sp_ДобавитьИлиОбновитьПропуск", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@СтудентID", item.Key.studentId);
                    cmd.Parameters.AddWithValue("@ПредметID", cbSubjects.SelectedValue);
                    cmd.Parameters.AddWithValue("@Дата", dpDate.SelectedDate.Value);
                    cmd.Parameters.AddWithValue("@НомерПары", item.Key.pair);
                    cmd.Parameters.AddWithValue("@ТипПропускаID", item.Value);
                    cmd.Parameters.AddWithValue("@ВнесилПользовательID", userId);

                    cmd.ExecuteNonQuery();
                }
            }

            changedAbsences.Clear();
            MessageBox.Show("Пропуски сохранены");
        }

        


        void SaveAbsence(int studentId, int pair, int typeId)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    "sp_ДобавитьИлиОбновитьПропуск", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@СтудентID", studentId);
                cmd.Parameters.AddWithValue("@ПредметID", cbSubjects.SelectedValue);
                cmd.Parameters.AddWithValue("@Дата", dpDate.SelectedDate.Value);
                cmd.Parameters.AddWithValue("@НомерПары", pair);
                cmd.Parameters.AddWithValue("@ТипПропускаID", typeId);
                cmd.Parameters.AddWithValue("@ВнесилПользовательID", userId);

                cmd.ExecuteNonQuery();
            }
        }


        void SetCell(JournalRow r, int p, string v)
        {
            if (p == 1) r.P1 = v;
            if (p == 2) r.P2 = v;
            if (p == 3) r.P3 = v;
            if (p == 4) r.P4 = v;
        }

        private void Report_Click(object sender, RoutedEventArgs e)
        {
            new ReportsWindow().ShowDialog();
        }
    }
}
