/*
Reversi

Zuzana Vopálková, 1. ročník

Programování 2 (NPRG031)
letní semestr 2020/21
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Reversi
{
    public partial class MainWindow : Form
    {
        public enum State { White, Black, End };

        private readonly Start parent;
        private State state = State.Black;
        private List<Button> buttons = new List<Button>();
        private ReversiGame game;
        private readonly StatePlace[,] board;
        private readonly Stopwatch timer = new Stopwatch();
        private GameState gameResult;
        private bool close = true;
        private readonly bool ai;

        public MainWindow(Start par, bool computer)
        {
            InitializeComponent();

            InitButtons();
            parent = par;
            ai = computer;

            // nastavení textu nápovědy
            helpLabel.Text = "Hráči střídavě pokládají na herní desku „kameny“ své barvy. Během tahu hráč položí jeden " +
                "\nkámen a to tak, aby mezi dvěma kameny jeho vlastní barvy vznikla souvislá svislá, " +
                "\nvodorovná nebo příčná řada kamenů soupeřovy barvy. Všechny takto obklíčené soupeřovy " +
                "\nkameny se otočí (změní barvu) a hráč je tímto způsobem získá. Vítězem je hráč, " +
                "\nkterý má na herní desce více kamenů na konci hry, tedy v okamžiku, kdy je deska zaplněna " +
                "\nkameny nebo pokud hráč, který je právě na tahu, nemá kam položit svůj kámen.";

            //nastavení počáteční plochy
            board = new StatePlace[8, 8];
            NewStart();

            game.SetMinimaxDepth(10); //nastavení hloubky prohledávání

        }

        private void InitButtons()
        {
            for (int i = 0; i < 64; i++)
            {
                buttons.Add(new Button());
            }

            // Add buttons to the list
            foreach (var layout in Controls)
            {
                if (layout is TableLayoutPanel table)
                {
                    foreach (var button in table.Controls)
                    {
                        if (button is Button gameButton)
                        {
                            buttons[gameButton.TabIndex] =  gameButton;
                        }
                    }
                }
            }
        }

        private void ButtonClick(int buttonIndex)
        {
            if (state != State.End)
            {
                switch (state)
                {
                    case State.Black:
                        if (game.MakeMove((buttonIndex / 8), buttonIndex % 8))
                        {
                            StatePlace[,] gameBoard = game.Board;
                            ChangeBoard(gameBoard);

                            Control(true);

                            if (ai && state != State.End) //pokud tah hráče byl vítězný už není potřeba hledat tah počítače
                            {
                                ComputerMove(gameBoard);

                            }
                            else if (!ai && state != State.End)
                            {
                                state = State.White;
                                messageBox.Text = "Bílá je na tahu.";
                            }
                        }
                        else
                        {
                            messageBox.Text = "Neplatný tah. Zkuste to znovu. Černá je na tahu";
                        }
                        break;
                    case State.White:
                        if (game.MakeMove(buttonIndex / 8, buttonIndex % 8))
                        {
                            StatePlace[,] gameBoard = game.Board;
                            ChangeBoard(gameBoard);

                            Control(false);

                            state = State.Black;
                            messageBox.Text = "Černá je na tahu.";
                        }
                        else
                        {
                            messageBox.Text = "Neplatný tah. Zkuste to znovu. Bílá je na tahu.";
                        }
                        break;
                }
            }
            else End();

            if (state == State.End) End();
        }

        private void ComputerMove(StatePlace[,] gameBoard)
        {
            timer.Start();

            messageBox.Text = "Počítač je na tahu. Prosím vyčkejte...";
            Update();

            var move = game.Search(board, true);

            if (move.Row != -1) // pokud nebyl nalezen možný krok, je možnost tahu přeskočena
            {
                if (game.MakeMove(move.Row, move.Column))
                {
                    timer.Stop();
                    int time = (int)timer.Elapsed.TotalMilliseconds;
                    Thread.Sleep(((1000 - time) >= 0 ? (1000 - time) : 0)); //aby byl na chvilku (sekundu) vidět důsledek tahu hráče

                    gameBoard = game.Board;
                    ChangeBoard(gameBoard);

                    Control(false);
                }
            }

            messageBox.Text = "Jste (černá) na tahu.";
        }

        private void ChangeBoard(StatePlace[,] gameBoard)
        {
            // provedení tahu na plochu
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!gameBoard[i, j].Equals(board[i, j]))
                    {
                        board[i, j] = gameBoard[i, j];
                        buttons[(i * 8) + j].BackColor = (board[i, j] == StatePlace.black) ? Color.Black : Color.White;
                    }
                }
            }
        }

        private void Control(bool isWhite)
        {
            //kontrola konce hry
            gameResult = game.GetGameResult(isWhite).GameState;
            if (gameResult.Equals(GameState.WhiteWon) || gameResult.Equals(GameState.BlackWon) || gameResult.Equals(GameState.Draw))
            {
                state = State.End;
                End();
            }
        }

        private void End()
        {
            // zobrazení konce hry v okně
            messageBox.Text = "Konec hry. ";
            messageBox.Text += !gameResult.Equals(GameState.WhiteWon) ? (gameResult.Equals(GameState.BlackWon) ? "Černá vyhrála." : "Remíza.") : "Bílá vyhrála.";

            Update();
            Thread.Sleep(1000);

            endBox.Visible = true;
            endRectangle.Visible = true;
            endBox.BackColor = Color.FromArgb(1, 250, 250, 250);
            endRectangle.BackColor = Color.Transparent;
        }

        private void NewStart()
        {
            // zahájení nové hry
            game = new ReversiGame(8, 8, false);

            // obarvení tlačítek
            for (int i = 0; i < 64; i++)
            {
                buttons[i].BackColor = Color.FromName("ActiveBorder");
            }
            buttons[28].BackColor = Color.Black;
            buttons[27].BackColor = Color.White;
            buttons[35].BackColor = Color.Black;
            buttons[36].BackColor = Color.White;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = StatePlace.empty;
                }
            }

            board[3, 3] = StatePlace.white;
            board[4, 4] = StatePlace.white;
            board[4, 3] = StatePlace.black;
            board[3, 4] = StatePlace.black;

            state = State.Black;
            messageBox.Text = ai ? "Jste (černá) na tahu." : "Černá je na tahu.";

            endBox.Visible = false;
            endRectangle.Visible = false;
        }

        private void BackToParent()
        {
            // zobrazení menu a zavření herního okna
            close = false;
            parent.Show();
            Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ButtonClick(0);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            ButtonClick(1);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            ButtonClick(2);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            ButtonClick(3);
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            ButtonClick(4);
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            ButtonClick(5);
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            ButtonClick(6);
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            ButtonClick(7);
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            ButtonClick(8);
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            ButtonClick(9);
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            ButtonClick(10);
        }

        private void Button12_Click(object sender, EventArgs e)
        {
            ButtonClick(11);
        }

        private void Button13_Click(object sender, EventArgs e)
        {
            ButtonClick(12);
        }

        private void Button14_Click(object sender, EventArgs e)
        {
            ButtonClick(13);
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            ButtonClick(14);
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            ButtonClick(15);
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            ButtonClick(16);
        }

        private void Button18_Click(object sender, EventArgs e)
        {
            ButtonClick(17);
        }

        private void Button19_Click(object sender, EventArgs e)
        {
            ButtonClick(18);
        }

        private void Button20_Click(object sender, EventArgs e)
        {
            ButtonClick(19);
        }

        private void Button21_Click(object sender, EventArgs e)
        {
            ButtonClick(20);
        }

        private void Button22_Click(object sender, EventArgs e)
        {
            ButtonClick(21);
        }

        private void Button23_Click(object sender, EventArgs e)
        {
            ButtonClick(22);
        }

        private void Button24_Click(object sender, EventArgs e)
        {
            ButtonClick(23);
        }

        private void Button25_Click(object sender, EventArgs e)
        {
            ButtonClick(24);
        }

        private void Button26_Click(object sender, EventArgs e)
        {
            ButtonClick(25);
        }

        private void Button27_Click(object sender, EventArgs e)
        {
            ButtonClick(26);
        }

        private void Button28_Click(object sender, EventArgs e)
        {
            ButtonClick(27);
        }

        private void Button29_Click(object sender, EventArgs e)
        {
            ButtonClick(28);
        }

        private void Button30_Click(object sender, EventArgs e)
        {
            ButtonClick(29);
        }

        private void Button31_Click(object sender, EventArgs e)
        {
            ButtonClick(30);
        }

        private void Button32_Click(object sender, EventArgs e)
        {
            ButtonClick(31);
        }

        private void Button33_Click(object sender, EventArgs e)
        {
            ButtonClick(32);
        }

        private void Button34_Click(object sender, EventArgs e)
        {
            ButtonClick(33);
        }

        private void Button35_Click(object sender, EventArgs e)
        {
            ButtonClick(34);
        }

        private void Button36_Click(object sender, EventArgs e)
        {
            ButtonClick(35);
        }

        private void Button37_Click(object sender, EventArgs e)
        {
            ButtonClick(36);
        }

        private void Button38_Click(object sender, EventArgs e)
        {
            ButtonClick(37);
        }

        private void Button39_Click(object sender, EventArgs e)
        {
            ButtonClick(38);
        }

        private void Button40_Click(object sender, EventArgs e)
        {
            ButtonClick(39);
        }

        private void Button41_Click(object sender, EventArgs e)
        {
            ButtonClick(40);
        }

        private void Button42_Click(object sender, EventArgs e)
        {
            ButtonClick(41);
        }

        private void Button43_Click(object sender, EventArgs e)
        {
            ButtonClick(42);
        }

        private void Button44_Click(object sender, EventArgs e)
        {
            ButtonClick(43);
        }

        private void Button45_Click(object sender, EventArgs e)
        {
            ButtonClick(44);
        }

        private void Button46_Click(object sender, EventArgs e)
        {
            ButtonClick(45);
        }

        private void Button47_Click(object sender, EventArgs e)
        {
            ButtonClick(46);
        }

        private void Button48_Click(object sender, EventArgs e)
        {
            ButtonClick(47);
        }

        private void Button49_Click(object sender, EventArgs e)
        {
            ButtonClick(48);
        }

        private void Button50_Click(object sender, EventArgs e)
        {
            ButtonClick(49);
        }

        private void Button51_Click(object sender, EventArgs e)
        {
            ButtonClick(50);
        }

        private void Button52_Click(object sender, EventArgs e)
        {
            ButtonClick(51);
        }

        private void Button53_Click(object sender, EventArgs e)
        {
            ButtonClick(52);
        }

        private void Button54_Click(object sender, EventArgs e)
        {
            ButtonClick(53);
        }

        private void Button55_Click(object sender, EventArgs e)
        {
            ButtonClick(54);
        }

        private void Button56_Click(object sender, EventArgs e)
        {
            ButtonClick(55);
        }

        private void Button57_Click(object sender, EventArgs e)
        {
            ButtonClick(56);
        }

        private void Button58_Click(object sender, EventArgs e)
        {
            ButtonClick(57);
        }

        private void Button59_Click(object sender, EventArgs e)
        {
            ButtonClick(58);
        }

        private void Button60_Click(object sender, EventArgs e)
        {
            ButtonClick(59);
        }

        private void Button61_Click(object sender, EventArgs e)
        {
            ButtonClick(60);
        }

        private void Button62_Click(object sender, EventArgs e)
        {
            ButtonClick(61);
        }

        private void Button63_Click(object sender, EventArgs e)
        {
            ButtonClick(62);
        }

        private void Button64_Click(object sender, EventArgs e)
        {
            ButtonClick(63);
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (close)
            {
                parent.Close();
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            BackToParent();
        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {
            helpLabel.Visible = helpLabel.Visible ? false : true;
        }

        private void HelpLabel_Click(object sender, EventArgs e)
        {
            if (helpLabel.Visible)
            {
                helpLabel.Visible = false;
            }
        }

        private void EndBox_Click(object sender, EventArgs e)
        {
            if (state == State.End && endBox.Visible) // po konci hry a zobrazení ikony je možné kliknutím obnovit hru
            {
                NewStart();
            }
        }

        private void EndRectangle_Click(object sender, EventArgs e)
        {
            if (state == State.End && endRectangle.Visible)
            {
                NewStart();
            }
        }
    }

}
