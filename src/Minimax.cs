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
    public enum StatePlace { empty = 0, white = 1, black = -1 } //možné stavy jednotlivých polí

    public struct MinimaxMove
    {
        public int row, column, value;

        public MinimaxMove(int row, int col, int val)
        {
            this.row = row;
            column = col;
            value = val;
        }

        public MinimaxMove(int row, int col)
        {
            this.row = row;
            this.column = col;
            value = 0;
        }

        public MinimaxMove(int val)
        {
            row = column = -1;
            value = val;
        }        
    }

    public abstract class Minimax
    {
        private int maxDepth;

        public abstract int MaxDepth { get; }

        public MinimaxMove Search(StatePlace[,] state, bool isWhite)
        {
            maxDepth = MaxDepth == -1 ? int.MaxValue : MaxDepth; //pokud je max hloubka nastavena na -1 prohledává se celý strom

            var nextMove = new MinimaxMove(0);

            nextMove = RealSearch(state, isWhite, int.MinValue, int.MaxValue, 0); //vnitřní prohledávání aktuální plochy

            if (nextMove.row == -1) // pokud nebyl nalezen ideální vybere se jakýkoliv z možných
            {
                foreach (var move in GetMoves(state, isWhite))
                {
                    nextMove = move;
                    nextMove.value = isWhite ? int.MinValue : int.MaxValue;
                    break;
                }
            }

            return nextMove;
        }
        protected abstract int EvaluateHeuristic(StatePlace[,] state);

        protected abstract StatePlace[,] GetCurrentBoardState(StatePlace[,] state, MinimaxMove move, bool isWhite);

        protected abstract IEnumerable<MinimaxMove> GetMoves(StatePlace[,] state, bool isWhite);

        protected abstract bool EndingTest(StatePlace[,] state, bool isWhite);

        protected abstract bool IsInCorner(MinimaxMove move);

        private MinimaxMove RealSearch(StatePlace[,] state, bool isWhite, int alpha, int beta, int depth)
        {
            if (depth >= maxDepth)
            {
                return new MinimaxMove(EvaluateHeuristic(state)); // využití heuristiky
            }

            var bestMove = new MinimaxMove(isWhite ? int.MinValue : int.MaxValue);

            var validMoves = GetMoves(state, isWhite); //nalezení všech proveditelných platných tahů

            bool availabilityOfMoves = false;
            foreach (var move in validMoves)
            {
                availabilityOfMoves = true;

                if (depth == 0) //pokud je možné v aktuálním tahu obsadit rohové pole, vybere se toto pole jako nejlepší
                {
                    if (IsInCorner(move))
                    {
                        return move;
                    }
                }

                var currentMove = move; 
                // spočtení hodnoty aktuálního možného kroku
                currentMove.value = RealSearch(GetCurrentBoardState(state, currentMove, isWhite), !isWhite, alpha, beta, depth + 1).value;

                if (isWhite)
                {
                    if (currentMove.value > bestMove.value) bestMove = currentMove; 
                    
                    if (bestMove.value >= beta) break; //kvůli alpha-beta ořezávání

                    alpha = Math.Max(alpha, bestMove.value); //nastavení alphy
                }
                else
                {
                    if (currentMove.value < bestMove.value) bestMove = currentMove;
                    
                    if (bestMove.value <= alpha) break; //kvůli alpha-beta ořezávání
                    
                    beta = Math.Min(beta, bestMove.value); //nastavení bety
                }
            }

            if (!availabilityOfMoves) //pokud nejsou v této vrstvě dostupné tahy, prohledává se další vrstva
            {
                bestMove.value = RealSearch(state, !isWhite, alpha, beta, depth + 1).value;
            }

            return bestMove;
        }
        
    }
    
    
}
