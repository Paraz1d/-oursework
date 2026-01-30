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

namespace UchetPropuskov
{
    public partial class AbsenceTypeWindow : Window
    {
        public string SelectedMark { get; private set; }
        public int SelectedTypeId { get; private set; }

        public AbsenceTypeWindow()
        {
            InitializeComponent();
        }

        private void U_Click(object sender, RoutedEventArgs e)
        {
            SelectedMark = "У";
            SelectedTypeId = 1; 
            DialogResult = true;
        }

        private void N_Click(object sender, RoutedEventArgs e)
        {
            SelectedMark = "Н";
            SelectedTypeId = 2; 
            DialogResult = true;
        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            SelectedMark = "Б";
            SelectedTypeId = 3; 
            DialogResult = true;
        }
    }
}
