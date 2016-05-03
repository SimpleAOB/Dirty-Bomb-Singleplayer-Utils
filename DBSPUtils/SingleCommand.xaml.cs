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

namespace DBSPUtils
{
    /// <summary>
    /// Interaction logic for SingleCommand.xaml
    /// </summary>
    public partial class SingleCommand : Window
    {
        public SingleCommand()
        {
            InitializeComponent();
        }

        public void setCommand(string c)
        {
            cp_cmd.Text = c;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(cp_cmd.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cp_cmd.Text = "";
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            Button_Click(null, null);
        }
    }
}
