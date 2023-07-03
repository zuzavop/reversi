/*
Reversi

Zuzana Vopálková, 1. roèník

Programování 2 (NPRG031)
letní semestr 2020/21
*/

using System;
using System.Windows.Forms;

namespace Reversi
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Start());
        }
    }
}
