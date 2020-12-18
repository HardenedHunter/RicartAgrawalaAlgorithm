using System;
using System.Windows.Forms;

namespace RicartAgrawalaAlgorithm
{
    public partial class View : Form, IView
    {
        public View()
        {
            InitializeComponent();
        }

        public event Action ProgramStarted;

        public void AddMessage(string message)
        {
            richTextBoxLog.Text += $"{message}\n";
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            ProgramStarted?.Invoke();
        }
    }
}
