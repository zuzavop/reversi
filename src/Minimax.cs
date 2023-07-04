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
    public enum StatePlace { empty = 0, white = 1, black = -1 } // possible states of each stones

    public struct MinimaxMove
    {
        public int Row { get; }
        public int Column { get; }
        public int Value { set;  get; }

        public MinimaxMove(int row, int col, int val)
        {
            Row = row;
            Column = col;
            Value = val;
        }

        public MinimaxMove(int row, int col)
        {
            Row = row;
            Column = col;
            Value = 0;
        }

        public MinimaxMove(int val)
        {
            Row = Column = -1;
            Value = val;
        }
    }

    public abstract class Minimax
    {
        private int maxDepth;

        public abstract int MaxDepth { get; }

        public MinimaxMove Search(StatePlace[,] state, bool isWhite)
        {
            maxDepth = MaxDepth == -1 ? int.MaxValue : MaxDepth; // if max depth is set to -1 search throught whole tree
            _ = new MinimaxMove(0);

            var nextMove = RealSearch(state, isWhite, int.MinValue, int.MaxValue, 0);

            if (nextMove.Row == -1) // if ideal move wasnt found - choose randomly
            {
                foreach (var move in GetMoves(state, isWhite))
                {
                    nextMove = move;
                    nextMove.Value = isWhite ? int.MinValue : int.MaxValue;
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
                return new MinimaxMove(EvaluateHeuristic(state)); // use of heuristic
            }

            var bestMove = new MinimaxMove(isWhite ? int.MinValue : int.MaxValue);

            var validMoves = GetMoves(state, isWhite); // found of possible moves

            bool availabilityOfMoves = false;
            foreach (var move in validMoves)
            {
                availabilityOfMoves = true;

                if (depth == 0 && IsInCorner(move)) // if current move in corner, choose that as best
                {
                    return move;
                }

                var currentMove = move;
                // count values of possible moves
                currentMove.Value = RealSearch(GetCurrentBoardState(state, currentMove, isWhite), !isWhite, alpha, beta, depth + 1).Value;

                if (isWhite)
                {
                    if (currentMove.Value > bestMove.Value) bestMove = currentMove;

                    if (bestMove.Value >= beta) break; // because of alpha-beta trimming

                    alpha = Math.Max(alpha, bestMove.Value); // set alpha
                }
                else
                {
                    if (currentMove.Value < bestMove.Value) bestMove = currentMove;

                    if (bestMove.Value <= alpha) break; // because of alpha-beta trimming

                    beta = Math.Min(beta, bestMove.Value); // set beta
                }
            }

            if (!availabilityOfMoves) // if none possible moves in this layer, search throught next layer
            {
                bestMove.Value = RealSearch(state, !isWhite, alpha, beta, depth + 1).Value;
            }

            return bestMove;
        }

    }


}
