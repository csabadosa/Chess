using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class ChessBot
    {
        private readonly int Depth = 2;
        private int iterationCount = 0;

        float[,] squareValues = new float[8, 8]
        {
            { 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f},
            { 0.00f, 0.00f, 0.10f, 0.10f, 0.10f, 0.10f, 0.00f, 0.00f},
            { -0.05f, 0.00f, 0.15f, 0.20f, 0.20f, 0.15f, 0.00f, -0.05f},
            { 0.00f, 0.00f, 0.20f, 0.25f, 0.25f, 0.15f, 0.00f, 0.00f},
            { 0.00f, 0.00f, 0.20f, 0.25f, 0.25f, 0.15f, 0.00f, 0.00f},
            { 0.00f, 0.00f, 0.15f, 0.20f, 0.20f, 0.15f, 0.00f, 0.00f},
            { 0.00f, 0.00f, 0.15f, 0.15f, 0.15f, 0.15f, 0.00f, 0.00f},
            { 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f},
        };
        public Move GetBotMove(GameState gameState)
        {
            int itC = 0;
            Move finalMove = null;
            Player player = gameState.CurrentPlayer;
            float finalVal = player == Player.White ? -1000 : 1000;
            GameState gameCopy = CloneGameState(gameState);
            IEnumerable<Move> moves = gameState.AllLegalMovesFor(gameState.CurrentPlayer);
            foreach (Move move in moves)
            {
                System.Diagnostics.Debug.WriteLine(itC++);
                gameCopy.MakeMove(move);
                float val = 0;
                if (gameCopy.IsGameOver())
                {
                    if(gameCopy.Result.Reason == EndReason.Checkmate)
                    {
                        return move;
                    } else
                    {
                        val = 0;
                    }
                }
                else
                {
                    val = SearchForMove(gameCopy, 0, gameState.CurrentPlayer == Player.White);
                }
                gameCopy = CloneGameState(gameState);

                switch (player)
                {
                    case Player.White:
                        if (val > finalVal)
                        {
                            finalVal = val;
                            finalMove = move;
                        }
                        break;
                    case Player.Black:
                        if (val < finalVal)
                        {
                            finalMove = move;
                            finalVal = val;
                        }
                        break;
                    default:
                        finalMove = null;
                        break;
                }
            }

            return finalMove;
        }

        private float SearchForMove(GameState gameState, int currentDepth, bool minLevel)
        {
            System.Diagnostics.Debug.WriteLine(iterationCount++);
            if (iterationCount == 341)
            {
                System.Diagnostics.Debug.WriteLine(iterationCount);
            }
            System.Diagnostics.Debug.WriteLine(gameState.CurrentPlayer);
            if (currentDepth == Depth)
            {
                return EvaluateBoard(gameState.Board);
            }

            float max = -1000;
            float min = 1000;

            GameState gameCopy = CloneGameState(gameState);

            IEnumerable<Move> moves = gameState.AllLegalMovesFor(gameState.CurrentPlayer);
            foreach (Move moveCandidate in moves)
            {
                gameCopy.MakeMove(moveCandidate);
                currentDepth++;
                float val = 0;
                if (gameCopy.IsGameOver())
                {
                    if (gameCopy.Result.Reason == EndReason.Checkmate)
                    {
                        if (minLevel)
                        {
                            val = -1000;
                        } 
                        else
                        {
                            val = 1000;
                        }
                    } 
                    else
                    {
                        val = 0;
                    }
                } 
                else
                {
                    val = SearchForMove(gameCopy, currentDepth, !minLevel);
                }

                gameCopy = CloneGameState(gameState);
                currentDepth--;

                if (minLevel)
                {
                    if (val < min)
                    {
                        min = val;
                    }
                }
                else
                {
                    if (val > max)
                    {
                        max = val;
                    }
                }
            }
            if (minLevel)
            {
                return min;
            }
            return max;
        }
        private float EvaluateBoard(Board board)
        {
            float white = 0;
            float black = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece piece = board[i, j];
                    if (piece == null)
                    {
                        continue;
                    }
                    if (piece.Color == Player.White)
                    {
                        white += squareValues[i, j];
                        if (piece.Type == PieceType.Pawn)
                        {
                            white += 1;
                        }
                        else if (piece.Type == PieceType.Bishop)
                        {
                            white += 3;
                        }
                        else if (piece.Type == PieceType.Knight)
                        {
                            white += 3;
                        }
                        else if (piece.Type == PieceType.Rook)
                        {
                            white += 5;
                        }
                        else if (piece.Type == PieceType.Queen)
                        {
                            white += 9;
                        }
                    }
                    else
                    {
                        white += squareValues[i, j];
                        if (piece.Type == PieceType.Pawn)
                        {
                            black += 1;
                        }
                        else if (piece.Type == PieceType.Bishop)
                        {
                            black += 3;
                        }
                        else if (piece.Type == PieceType.Knight)
                        {
                            black += 3;
                        }
                        else if (piece.Type == PieceType.Rook)
                        {
                            black += 5;
                        }
                        else if (piece.Type == PieceType.Queen)
                        {
                            black += 9;
                        }
                    }
                }
            }
            return white - black;
        }

        private GameState CloneGameState(GameState gameState)
        {
            Board board = gameState.Board.Copy();
            GameState gameCopy = new GameState(gameState.CurrentPlayer, board);
            return gameCopy;
        }
    }
}
