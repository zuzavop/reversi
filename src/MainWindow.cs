/*
Reversi

Zuzana Vopálková, 1. ročník

Programování 2 (NPRG031)
letní semestr 2020/21
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace Reversi
{
    public partial class MainWindow : Form
    {
        public enum State { White, Black, End };

        private Start parent;
        private State state = State.Black;
        private List<Button> buttons = new List<Button>();
        private ReversiGame game;
        private StatePlace[,] board;
        private Stopwatch timer = new Stopwatch();
        private GameState gameResult;
        private bool close = true;
        private bool ai;

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
            board = new StatePlace[8,8];
            NewStart();

            game.SetMinimaxDepth(10); //nastavení hloubky prohledávání

        }        

        private void InitButtons()
        {
            buttons.Add(button1);
            buttons.Add(button2);
            buttons.Add(button3);
            buttons.Add(button4);
            buttons.Add(button5);
            buttons.Add(button6);
            buttons.Add(button7);
            buttons.Add(button8);
            buttons.Add(button9);
            buttons.Add(button10);
            buttons.Add(button11);
            buttons.Add(button12);
            buttons.Add(button13);
            buttons.Add(button14);
            buttons.Add(button15);
            buttons.Add(button16);
            buttons.Add(button17);
            buttons.Add(button18);
            buttons.Add(button19);
            buttons.Add(button20);
            buttons.Add(button21);
            buttons.Add(button22);
            buttons.Add(button23);
            buttons.Add(button24);
            buttons.Add(button25);
            buttons.Add(button26);
            buttons.Add(button27);
            buttons.Add(button28);
            buttons.Add(button29);
            buttons.Add(button30);
            buttons.Add(button31);
            buttons.Add(button32);
            buttons.Add(button33);
            buttons.Add(button34);
            buttons.Add(button35);
            buttons.Add(button36);
            buttons.Add(button37);
            buttons.Add(button38);
            buttons.Add(button39);
            buttons.Add(button40);
            buttons.Add(button41);
            buttons.Add(button42);
            buttons.Add(button43);
            buttons.Add(button44);
            buttons.Add(button45);
            buttons.Add(button46);
            buttons.Add(button47);
            buttons.Add(button48);
            buttons.Add(button49);
            buttons.Add(button50);
            buttons.Add(button51);
            buttons.Add(button52);
            buttons.Add(button53);
            buttons.Add(button54);
            buttons.Add(button55);
            buttons.Add(button56);
            buttons.Add(button57);
            buttons.Add(button58);
            buttons.Add(button59);
            buttons.Add(button60);
            buttons.Add(button61);
            buttons.Add(button62);
            buttons.Add(button63);
            buttons.Add(button64);
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
                                
                            } else if (!ai && state != State.End)
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

            if (move.row != -1) // pokud nebyl nalezen možný krok, je možnost tahu přeskočena
            {
                if (game.MakeMove(move.row, move.column))
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

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonClick(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ButtonClick(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ButtonClick(2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ButtonClick(3);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ButtonClick(4);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ButtonClick(5);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ButtonClick(6);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ButtonClick(7);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ButtonClick(8);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ButtonClick(9);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            ButtonClick(10);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ButtonClick(11);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            ButtonClick(12);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ButtonClick(13);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            ButtonClick(14);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            ButtonClick(15);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            ButtonClick(16);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            ButtonClick(17);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            ButtonClick(18);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            ButtonClick(19);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            ButtonClick(20);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            ButtonClick(21);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            ButtonClick(22);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            ButtonClick(23);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            ButtonClick(24);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            ButtonClick(25);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            ButtonClick(26);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            ButtonClick(27);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            ButtonClick(28);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            ButtonClick(29);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            ButtonClick(30);
        }

        private void button32_Click(object sender, EventArgs e)
        {
            ButtonClick(31);
        }

        private void button33_Click(object sender, EventArgs e)
        {
            ButtonClick(32);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            ButtonClick(33);
        }

        private void button35_Click(object sender, EventArgs e)
        {
            ButtonClick(34);
        }

        private void button36_Click(object sender, EventArgs e)
        {
            ButtonClick(35);
        }

        private void button37_Click(object sender, EventArgs e)
        {
            ButtonClick(36);
        }

        private void button38_Click(object sender, EventArgs e)
        {
            ButtonClick(37);
        }

        private void button39_Click(object sender, EventArgs e)
        {
            ButtonClick(38);
        }

        private void button40_Click(object sender, EventArgs e)
        {
            ButtonClick(39);
        }

        private void button41_Click(object sender, EventArgs e)
        {
            ButtonClick(40);
        }

        private void button42_Click(object sender, EventArgs e)
        {
            ButtonClick(41);
        }

        private void button43_Click(object sender, EventArgs e)
        {
            ButtonClick(42);
        }

        private void button44_Click(object sender, EventArgs e)
        {
            ButtonClick(43);
        }

        private void button45_Click(object sender, EventArgs e)
        {
            ButtonClick(44);
        }

        private void button46_Click(object sender, EventArgs e)
        {
            ButtonClick(45);
        }

        private void button47_Click(object sender, EventArgs e)
        {
            ButtonClick(46);
        }

        private void button48_Click(object sender, EventArgs e)
        {
            ButtonClick(47);
        }

        private void button49_Click(object sender, EventArgs e)
        {
            ButtonClick(48);
        }

        private void button50_Click(object sender, EventArgs e)
        {
            ButtonClick(49);
        }

        private void button51_Click(object sender, EventArgs e)
        {
            ButtonClick(50);
        }

        private void button52_Click(object sender, EventArgs e)
        {
            ButtonClick(51);
        }

        private void button53_Click(object sender, EventArgs e)
        {
            ButtonClick(52);
        }

        private void button54_Click(object sender, EventArgs e)
        {
            ButtonClick(53);
        }

        private void button55_Click(object sender, EventArgs e)
        {
            ButtonClick(54);
        }

        private void button56_Click(object sender, EventArgs e)
        {
            ButtonClick(55);
        }

        private void button57_Click(object sender, EventArgs e)
        {
            ButtonClick(56);
        }

        private void button58_Click(object sender, EventArgs e)
        {
            ButtonClick(57);
        }

        private void button59_Click(object sender, EventArgs e)
        {
            ButtonClick(58);
        }

        private void button60_Click(object sender, EventArgs e)
        {
            ButtonClick(59);
        }

        private void button61_Click(object sender, EventArgs e)
        {
            ButtonClick(60);
        }

        private void button62_Click(object sender, EventArgs e)
        {
            ButtonClick(61);
        }

        private void button63_Click(object sender, EventArgs e)
        {
            ButtonClick(62);
        }

        private void button64_Click(object sender, EventArgs e)
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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            BackToParent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            helpLabel.Visible = helpLabel.Visible ? false : true;
        }

        private void helpLabel_Click(object sender, EventArgs e)
        {
            if (helpLabel.Visible)
            {
                helpLabel.Visible = false;
            }
        }

        private void endBox_Click(object sender, EventArgs e)
        {
            if (state == State.End && endBox.Visible) // po konci hry a zobrazení ikony je možné kliknutím obnovit hru
            {
                NewStart();
            }
        }

        private void endRectangle_Click(object sender, EventArgs e)
        {
            if (state == State.End && endRectangle.Visible)
            {
                NewStart();
            }
        }
    }

}
