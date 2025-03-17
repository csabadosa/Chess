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
        private readonly int Depth = 3;
        private int iterationCount = 0;

        float[,] squareValues = new float[8, 8]
        {
            { 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f},
            { 0.00f, 0.00f, 0.10f, 0.10f, 0.10f, 0.10f, 0.00f, 0.00f},
            { -0.05f, 0.00f, 0.15f, 0.25f, 0.25f, 0.15f, 0.00f, -0.05f},
            { 0.00f, 0.00f, 0.25f, 0.50f, 0.50f, 0.25f, 0.00f, 0.00f},
            { 0.00f, 0.00f, 0.25f, 0.50f, 0.50f, 0.25f, 0.00f, 0.00f},
            { -0.05f, 0.00f, 0.15f, 0.25f, 0.25f, 0.15f, 0.00f, -0.05f},
            { 0.00f, 0.00f, 0.15f, 0.15f, 0.15f, 0.15f, 0.00f, 0.00f},
            { 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f, 0.00f},
        };
        public Move GetBotMove(GameState gameState)
        {
            Move finalMove = null;
            Player player = gameState.CurrentPlayer;
            float finalVal = player == Player.White ? -10000 : 10000;
            GameState gameCopy = CloneGameState(gameState);
            IEnumerable<Move> moves = gameState.AllLegalMovesFor(gameState.CurrentPlayer);
            foreach (Move move in moves)
            {
                gameCopy.MakeMove(move);
                float val = 0;
                if (gameCopy.IsGameOver())
                {
                    if (gameCopy.Result.Reason == EndReason.Checkmate)
                    {
                        return move;
                    }
                    else
                    {
                        val = 0;
                    }
                }
                else
                {
                    val = SearchForMove(gameCopy, 0, gameState.CurrentPlayer == Player.White, -10000, 10000);
                }
                System.Diagnostics.Debug.WriteLine("Val: ");
                System.Diagnostics.Debug.WriteLine(val);
                System.Diagnostics.Debug.WriteLine("Row: " + move.FromPos.Row);
                System.Diagnostics.Debug.WriteLine("Col: " + move.FromPos.Column);
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
            System.Diagnostics.Debug.WriteLine("Final val: " + finalVal);
            
            return finalMove;
        }

        private float SearchForMove(GameState gameState, int currentDepth, bool minLevel, float alpha, float beta)
        {
            //System.Diagnostics.Debug.WriteLine(iterationCount++);
            if (iterationCount == 341)
            {
                //System.Diagnostics.Debug.WriteLine(iterationCount);
            }
            //System.Diagnostics.Debug.WriteLine(gameState.CurrentPlayer);
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
                    val = SearchForMove(gameCopy, currentDepth + 1, !minLevel, alpha, beta);
                }

                gameCopy = CloneGameState(gameState);

                if (moveCandidate.FromPos.Row == 1 && moveCandidate.FromPos.Column == 4)
                {
                    System.Diagnostics.Debug.WriteLine("Pawn val: ");
                }

                if (minLevel)
                {
                    min = Math.Min(min, val);
                    beta = Math.Min(beta, val);
                }
                else
                {
                    max = Math.Max(max, val);
                    alpha = Math.Max(alpha, val);   
                }

                if (beta <= alpha)
                {
                    break;
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
                            if(!piece.HasMoved)
                            {
                                white -= 0.25f;
                            }
                            white += 3;
                        }
                        else if (piece.Type == PieceType.Knight)
                        {
                            if (!piece.HasMoved)
                            {
                                white -= 0.25f;
                            }
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
                        black += squareValues[i, j];
                        if (piece.Type == PieceType.Pawn)
                        {
                            black += 1;
                        }
                        else if (piece.Type == PieceType.Bishop)
                        {
                            if (!piece.HasMoved)
                            {
                                black -= 0.25f;
                            }
                            black += 3;
                        }
                        else if (piece.Type == PieceType.Knight)
                        {
                            if (!piece.HasMoved)
                            {
                                black -= 0.25f;
                            }
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
