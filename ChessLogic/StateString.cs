using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLogic
{
    public class StateString
    {
        private readonly StringBuilder sb = new StringBuilder();

        public StateString(Player currentPlayer, Board board) 
        {
            AddPiecePlacement(board);
            sb.Append(' ');
            AddCurrentPlayer(currentPlayer);
            sb.Append(' ');
            AddCastlingRIghts(board);
            sb.Append(' ');
            AddEnPassant(board, currentPlayer);
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        private static char PieceChar(Piece piece)
        {
            char c = piece.Type switch
            {
                PieceType.Pawn => 'p',
                PieceType.Bishop => 'b',
                PieceType.Knight => 'n',
                PieceType.Rook => 'r',
                PieceType.Queen => 'q',
                PieceType.King => 'k',
                _ => ' '
            };

            if(piece.Color == Player.White)
            {
                return char.ToUpper(c);
            }

            return c;
        }

        private void AddRowData(Board board, int row)
        {
            int empty = 0;

            for (int i = 0; i < 8; i++)
            {
                if (board[row, i] == null)
                {
                    empty++;
                    continue;
                }

                if (empty > 0)
                {
                    sb.Append(empty);
                    empty = 0;
                }

                sb.Append(PieceChar(board[row, i]));
            }

            if(empty > 0)
            {
                sb.Append(empty);
            }
        }

        private void AddPiecePlacement(Board board)
        {
            for (int i = 0;i < 8;i++)
            {
                if (i != 0)
                {
                    sb.Append('/');
                }
                AddRowData(board,i);
            }
        }

        private void AddCurrentPlayer(Player currentPlayer)
        {
            if (currentPlayer == Player.White)
            {
                sb.Append('w');
            } 
            else
            {
                sb.Append('b');
            }
        }

        private void AddCastlingRIghts(Board board)
        {
            bool castleWShort = board.CastleRightShort(Player.White);
            bool castleWLong = board.CastleRightLong(Player.White);

            bool castleBShort = board.CastleRightShort(Player.Black);
            bool castleBLong = board.CastleRightLong(Player.Black);

            if(!(castleWShort || castleWLong || castleBShort || castleBLong))
            {
                sb.Append('-');
                return;
            }

            if(castleWShort)
            {
                sb.Append('K');
            }

            if (castleWLong)
            {
                sb.Append('Q');
            }

            if (castleBShort)
            {
                sb.Append('k');
            }

            if (castleBLong)
            {
                sb.Append('q');
            }
        }

        private void AddEnPassant(Board board, Player currentPlayer)
        {
            if(!board.CanCaptureEnPassant(currentPlayer))
            {
                sb.Append('-');
                return;
            }

            Position pos = board.GetPawnSkipPosition(currentPlayer.Opponent());

            char file = (char)('a' + pos.Column);
            int rank = 8 - pos.Row;
            sb.Append(file);
            sb.Append(rank);
        }
    }
}
