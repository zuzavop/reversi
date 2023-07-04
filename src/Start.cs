/*
Reversi

Zuzana Vopálková, 1. ročník

Programování 2 (NPRG031)
letní semestr 2020/21
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Reversi
{
    public partial class Start : Form
    {
        private MainWindow gameForm;
        private bool ai = false;

        public Start()
        {
            InitializeComponent();
        }

        private void Start_Resize(object sender, EventArgs e)
        {
            int size = tableLayoutPanel1.Size.Width / 10;
            title.Font = new Font("Harlow Solid Italic", size, FontStyle.Italic);
            buttonStartGame.Font = new Font("Comic Sans MS", size / 3);
            button1.Font = new Font("Comic Sans MS", size / 7);
            button2.Font = new Font("Comic Sans MS", size / 7);
        }

        private void ButtonStartGame_Click(object sender, EventArgs e)
        {
            gameForm = new MainWindow(this, ai);
            gameForm.Show();
            Hide();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ai = true;
            button1.BackColor = Color.FromName("ActiveBorder");
            button2.BackColor = Color.FromName("Window");
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            ai = false;
            button1.BackColor = Color.FromName("Window");
            button2.BackColor = Color.FromName("ActiveBorder");
        }
    }
}
