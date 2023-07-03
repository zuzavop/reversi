/*
Reversi

Zuzana Vopálková, 1. ročník

Programování 2 (NPRG031)
letní semestr 2020/21
*/

using System;
using System.Collections.Generic;

namespace Reversi
{
    class ReversiGame : Minimax
    {
        public ReversiGame(int numRows, int numCols, bool isWhiteStart)
        {
            numberRows = numRows;
            numberCol = numCols;
            Board = new StatePlace[numRows, numCols];
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numCols; j++)
                {
                    Board[i, j] = StatePlace.empty;
                }
            }

            //nastavení počátečních pozic
            Board[3, 3] = StatePlace.white;
            Board[4, 4] = StatePlace.white;
            Board[4, 3] = StatePlace.black;
            Board[3, 4] = StatePlace.black;

            isWhite = isWhiteStart; //pokud true začíná hráč s bílými kameny
        }

        public StatePlace[,] Board { get; }
        public bool isWhite { get; private set; }

        private int numberRows, numberCol, depth;

        public override int MaxDepth => depth;

        public void SetMinimaxDepth(int maxDepth)
        {
            depth = maxDepth;
        }

        protected override int EvaluateHeuristic(StatePlace[,] state)
        {
            int boardValue = 0;

            // +1 pro světlé kameny a -1 pro tmavé
            for (int r = 0; r < numberRows; r++)
            {
                for (int c = 0; c < numberCol; c++)
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
            // vytvoření nové plochy s aktuálním tahem
            var states = new StatePlace[numberRows, numberCol];
            for (int r = 0; r < numberRows; r++)
            {
                for (int c = 0; c < numberCol; c++)
                {
                    states[r, c] = state[r, c];
                }
            }

            states[move.row, move.column] = isWhite ? StatePlace.white : StatePlace.black;

            return states;
        }

        protected override IEnumerable<MinimaxMove> GetMoves(StatePlace[,] state, bool isWhite)
        {
            // nalezení všech možných tahů aktuálního hráče
            for (int r = 0; r < numberRows; r++)
            {
                for (int c = 0; c < numberCol; c++)
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
            if ((move.column == 0 && move.row == 0) || (move.column == 0 && move.row == (numberRows - 1)) || (move.column == (numberCol - 1) && move.row == 0) || (move.column == (numberCol - 1) && move.row == (numberRows - 1))) return true;
            return false;
        }

        private bool ValidMove(StatePlace[,] state, int row, int col, StatePlace player, StatePlace enemy, int dirRow, int dirCol)
        {
            // prohledání platnosti tahu do jednoho směru - je tím směrem obarvené tlačítko hráče
            if (row + dirRow < numberRows && row + dirRow >= 0 && col + dirCol < numberCol && col + dirCol >= 0 && state[row + dirRow, col + dirCol] == enemy)
            {
                for (int currentRow = row + 2* dirRow, currentCol = col + 2* dirCol;
                    currentRow < numberRows && currentRow >= 0 && currentCol < numberCol && currentCol >= 0;
                    currentRow = currentRow + dirRow, currentCol = currentCol + dirCol)
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
            // oveření platnosti tahu
            if ((row >= 0 && row < 8) && (col >= 0 && col < 8)) 
            {
                if (state[row, col] != StatePlace.empty)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            var player = isWhite ? StatePlace.white : StatePlace.black;
            var enemy = isWhite ? StatePlace.black : StatePlace.white;

            // kontrola pod
            if(ValidMove(state, row, col, player, enemy, 1 , 0)) return true;

            // kontrola pod vpravo
            if (ValidMove(state, row, col, player, enemy, 1, 1)) return true;

            // kontrola pod vlevo
            if (ValidMove(state, row, col, player, enemy, 1, -1)) return true;

            // kontrola pravé
            if (ValidMove(state, row, col, player, enemy, 0, 1)) return true;

            // kontrola levé
            if (ValidMove(state, row, col, player, enemy, 0, -1)) return true;

            // kontrola nad
            if (ValidMove(state, row, col, player, enemy, -1, 0)) return true;

            // kontrola nad vpravo
            if (ValidMove(state, row, col, player, enemy, -1, 1)) return true;

            // kontrola nad vlevo
            if (ValidMove(state, row, col, player, enemy, -1, -1)) return true;

            return false;
        }

        private void ControlMove(int row, int col, StatePlace enemy, StatePlace player, int dirRow, int dirCol)
        {
            // změna tlačítek určitým směrem
            if (row + dirRow < numberRows && row + dirRow >= 0 && col + dirCol < numberCol && col + dirCol >=0 && Board[row + dirRow, col + dirCol] == enemy)
            {
                bool find = false;
                for (int currentRow = row + 2*dirRow, currentCol = col + 2*dirCol; 
                    currentRow < numberRows && currentRow >= 0 && currentCol < numberCol && currentCol >= 0;
                    currentRow = currentRow + dirRow, currentCol = currentCol + dirCol) //hledání jestli někde aktuálním směrem je hráčův kámen - kvůli spojení kamenů
                {
                    if (Board[currentRow, currentCol] == StatePlace.empty) break; //při prázdném políčku je zbytečné hledat dál (nemůže se spojit)
                    if (Board[currentRow, currentCol] == player) find = true;
                }
                if (find)
                {
                    for (int currentRow = row + dirRow, currentCol = col + dirCol;
                        currentRow < numberRows && currentRow >= 0 && currentCol < numberCol && currentCol >= 0;
                        currentRow = currentRow + dirRow, currentCol = currentCol + dirCol)
                    {
                        if (Board[currentRow, currentCol] == StatePlace.empty || Board[currentRow, currentCol] == player) break;
                        Board[currentRow, currentCol] = player; //změna políček
                    }
                }
            }
        }

        public bool MakeMove(int row, int col) 
        {
            // provedení aktuálního tahu pokud je platný
            if (!IsValid(Board, isWhite, row, col))
            {
                return false;
            }

            var player = isWhite ? StatePlace.white : StatePlace.black;
            var enemy = isWhite ? StatePlace.black : StatePlace.white;

            Board[row, col] = player;

            // změnit pod pokud je tam soupeř
            ControlMove(row, col, enemy, player, 1, 0);

            // změnit nad
            ControlMove(row, col, enemy, player, -1, 0);

            // změnit pravý
            ControlMove(row, col, enemy, player, 0, 1);

            // změnit levý
            ControlMove(row, col, enemy, player, 0, -1);

            // změnit pod pravý
            ControlMove(row, col, enemy, player, 1, 1);

            // změnit pod levý
            ControlMove(row, col, enemy, player, 1, -1);

            // změnit nad pravý
            ControlMove(row, col, enemy, player, -1, 1);

            // změnit pod levý
            ControlMove(row, col, enemy, player, -1, -1);

            isWhite = !isWhite; //změna hráče

            return true;
        }

        public GameResult GetGameResult(bool isWhite)
        {
            var result = new GameResult();

            if (EndingTest(Board, isWhite)) //pokud aktuální hráč nemůže táhnout -> konec hry
            {
                // spočítání obsazenosti plochy (pokud není konec hry není potřeba vědět početní rozložení barev)
                for (int i = 0; i < numberRows; i++)
                {
                    for (int j = 0; j < numberCol; j++)
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
            // neexistují tahy pro aktuálního hráče -> konec hry
            foreach (var move in GetMoves(board, isWhite))
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
