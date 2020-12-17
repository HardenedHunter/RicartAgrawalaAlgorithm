using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RicartAgrawalaAlgorithm
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DpiFix();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var view = new View();
            var presenter = new Presenter(view);
            Application.Run(view);
        }

        /// <summary>
        /// Исправление блюра при включенном масштабировании в ОС windows 8 и выше
        /// </summary>
        public static void DpiFix()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }
        }

        /// <summary>
        /// WinAPI SetProcessDPIAware 
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
