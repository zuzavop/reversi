/*
Reversi

Zuzana Vopálková, 1. ročník

Programování 2 (NPRG031)
letní semestr 2020/21
*/

using System.Collections.Generic;

namespace Reversi
{
    class ReversiGame : Minimax
    {
        private readonly int numberRows, numberCols;
        private int depth;

        public ReversiGame(int numRows, int numCols, bool isWhiteStart)
        {
            numberRows = numRows;
            numberCols = numCols;
            Board = new StatePlace[numRows, numCols];

            InitializeBoard();

            IsWhite = isWhiteStart; // if true start player has white stones
        }

        public StatePlace[,] Board { get; }
        public bool IsWhite { get; private set; }

        public override int MaxDepth => depth;

        private void InitializeBoard()
        {
            for (int i = 0; i < numberRows; i++)
            {
                for (int j = 0; j < numberCols; j++)
                {
                    Board[i, j] = StatePlace.empty;
                }
            }

            Board[3, 3] = StatePlace.white;
            Board[4, 4] = StatePlace.white;
            Board[4, 3] = StatePlace.black;
            Board[3, 4] = StatePlace.black;
        }

        public void SetMinimaxDepth(int maxDepth)
        {
            depth = maxDepth;
        }

        protected override int EvaluateHeuristic(StatePlace[,] state)
        {
            int boardValue = 0;

            // +1 for light stones and -1 for dark ones
            for (int r = 0; r < numberRows; r++)
            {
                for (int c = 0; c < numberCols; c++)
                {
                    if (state[r, c] == StatePlace.white)
                    {
                        boardValue++;
                    }
                    if (state[r, c] == StatePlace.black)
                    {
                        boardValue--;
                    }
                }
            }
            return boardValue;
        }

        protected override StatePlace[,] GetCurrentBoardState(StatePlace[,] state, MinimaxMove move, bool isWhite)
        {
            var newState = (StatePlace[,])state.Clone();

            newState[move.Row, move.Column] = isWhite ? StatePlace.white : StatePlace.black;

            return newState;
        }

        protected override IEnumerable<MinimaxMove> GetMoves(StatePlace[,] state, bool isWhite)
        {
            // find all possible turns of current player
            for (int r = 0; r < numberRows; r++)
            {
                for (int c = 0; c < numberCols; c++)
                {
                    if (IsValid(state, isWhite, r, c))
                    {
                        yield return new MinimaxMove(r, c);
                    }
                }
            }
        }

        protected override bool IsInCorner(MinimaxMove move)
        {
            return ((move.Column == 0 && move.Row == 0) || (move.Column == 0 && move.Row == (numberRows - 1)) || 
                (move.Column == (numberCols - 1) && move.Row == 0) || (move.Column == (numberCols - 1) && move.Row == (numberRows - 1)));
        }

        private bool ValidMove(StatePlace[,] state, int row, int col, StatePlace player, StatePlace enemy, int dirRow, int dirCol)
        {
            // check validity of move - in that direction is player stone
            if (row + dirRow < numberRows && row + dirRow >= 0 && col + dirCol < numberCols && col + dirCol >= 0 && state[row + dirRow, col + dirCol] == enemy)
            {
                for (int currentRow = row + 2 * dirRow, currentCol = col + 2 * dirCol;
                    currentRow < numberRows && currentRow >= 0 && currentCol < numberCols && currentCol >= 0;
                    currentRow += dirRow, currentCol += dirCol)
                {
                    if (state[currentRow, currentCol] == player)
                    {
                        return true;
                    }
                    if (state[currentRow, currentCol] == StatePlace.empty)
                    {
                        break;
                    }
                }
            }

            return false;
        }

        private bool IsValid(StatePlace[,] state, bool isWhite, int row, int col)
        {
            // check validity of move 
            if (row < 0 || row >= numberRows || col < 0 || col >= numberCols || state[row, col] != StatePlace.empty)
            {
                return false;
            }

            var player = isWhite ? StatePlace.white : StatePlace.black;
            var enemy = isWhite ? StatePlace.black : StatePlace.white;

            for (int dirRow = -1; dirRow <= 1; dirRow++)
            {
                for (int dirCol = -1; dirCol <= 1; dirCol++)
                {
                    if (dirRow == 0 && dirCol == 0)
                    {
                        continue;
                    }

                    if (ValidMove(state, row, col, player, enemy, dirRow, dirCol))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void ControlMove(int row, int col, StatePlace enemy, StatePlace player, int dirRow, int dirCol)
        {
            // change stones in given direction
            if (row + dirRow < numberRows && row + dirRow >= 0 && col + dirCol < numberCols && col + dirCol >= 0 && Board[row + dirRow, col + dirCol] == enemy)
            {
                bool find = false;
                for (int currentRow = row + 2 * dirRow, currentCol = col + 2 * dirCol;
                    currentRow < numberRows && currentRow >= 0 && currentCol < numberCols && currentCol >= 0;
                    currentRow += dirRow, currentCol += dirCol) // search for player stone - connection of stones
                {
                    if (Board[currentRow, currentCol] == StatePlace.empty) break; // if empty stone is not possible to connect
                    if (Board[currentRow, currentCol] == player) find = true;
                }
                if (find)
                {
                    for (int currentRow = row + dirRow, currentCol = col + dirCol;
                        currentRow < numberRows && currentRow >= 0 && currentCol < numberCols && currentCol >= 0;
                        currentRow += dirRow, currentCol += dirCol)
                    {
                        if (Board[currentRow, currentCol] == StatePlace.empty || Board[currentRow, currentCol] == player) break;
                        Board[currentRow, currentCol] = player; // change of stones
                    }
                }
            }
        }

        public bool MakeMove(int row, int col)
        {
            // do turn if valid
            if (!IsValid(Board, IsWhite, row, col))
            {
                return false;
            }

            var player = IsWhite ? StatePlace.white : StatePlace.black;
            var enemy = IsWhite ? StatePlace.black : StatePlace.white;

            Board[row, col] = player;

            for (int dirRow = -1; dirRow <= 1; dirRow++)
            {
                for (int dirCol = -1; dirCol <= 1; dirCol++)
                {
                    if (dirRow == 0 && dirCol == 0)
                    {
                        continue;
                    }

                    ControlMove(row, col, enemy, player, dirRow, dirCol);
                }
            }

            IsWhite = !IsWhite; // change of player

            return true;
        }

        public GameResult GetGameResult(bool isWhite)
        {
            var result = new GameResult();

            if (EndingTest(Board, isWhite)) // if current player cant have valid moves -> end of game
            {
                // compute occupied stones
                for (int i = 0; i < numberRows; i++)
                {
                    for (int j = 0; j < numberCols; j++)
                    {
                        if (Board[i, j] == StatePlace.black)
                        {
                            result.NumberBlack++;
                        }
                        if (Board[i, j] == StatePlace.white)
                        {
                            result.NumberWhite++;
                        }
                    }
                }

                if (result.NumberWhite > result.NumberBlack)
                {
                    result.GameState = GameState.WhiteWon;
                }
                else if (result.NumberWhite < result.NumberBlack)
                {
                    result.GameState = GameState.BlackWon;
                }
                else
                {
                    result.GameState = GameState.Draw;
                }
            }
            else
            {
                result.GameState = GameState.InProgress;
            }

            return result;
        }

        protected override bool EndingTest(StatePlace[,] board, bool isWhite)
        {
            // current player dont have valid moves -> end of game
            foreach (var _ in GetMoves(board, isWhite))
            {
                return false;
            }
            return true;
        }
    }

    public struct GameResult
    {
        public GameState GameState;
        public int NumberWhite, NumberBlack;
    }

    public enum GameState { InProgress = int.MaxValue, WhiteWon = 1, BlackWon = -1, Draw = 0 }

}
