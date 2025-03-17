using ChessLogic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChessUI;

public partial class MainWindow : Window
{
    private readonly Image[,] pieceImages = new Image[8, 8];
    private readonly Rectangle[,] highlights = new Rectangle[8, 8];
    private readonly Dictionary<Position, Move> moveCache = new Dictionary<Position, Move>();

    private GameState gameState;
    private Position selectedPos = null;
    private Move lastMove = null;

    private ChessBot chessBot;

    public MainWindow()
    {
        InitializeComponent();
        InitializeBoard();
        gameState = new GameState(Player.White, Board.Initial());
        DrawBoard(gameState.Board);
        SetCursor(gameState.CurrentPlayer);
        chessBot = new ChessBot();
    }

    private void InitializeBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Image image = new Image();
                pieceImages[i, j] = image;
                PieceGrid.Children.Add(image);
                Rectangle highlight = new Rectangle();
                highlights[i, j] = highlight;
                HighlightGrid.Children.Add(highlight);
            }
        }
    }

    private void DrawBoard(Board board)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Piece piece = board[i, j];
                pieceImages[i, j].Source = Images.GetImage(piece);
            }
        }
    }

    private void BoardGrid_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (IsMenuOnScreen())
        {
            return;
        }

        Point point = e.GetPosition(BoardGrid);
        Position pos = ToSquarePosition(point);

        if (selectedPos == null)
        {
            OnFromPositionSelected(pos);
        }
        else
        {
            OnToPositionSelected(pos);
        }
    }

    private void OnFromPositionSelected(Position pos)
    {
        IEnumerable<Move> moves = gameState.LegalMovesForPiece(pos);

        if (moves.Any())
        {
            selectedPos = pos;
            CacheMoves(moves);
            ShowHighlights();
        }
    }

    private void OnToPositionSelected(Position pos)
    {
        selectedPos = null;
        HideHighlights();
        if (moveCache.TryGetValue(pos, out Move move))
        {
            if (move.Type == MoveType.PawnPromotion)
            {
                HandlePromotion(move.FromPos, move.ToPos);
            }
            else
            {
                HandleMove(move);
            }
        }
    }

    private void HandlePromotion(Position fromPos, Position toPos)
    {
        pieceImages[toPos.Row, toPos.Column].Source = Images.GetImage(gameState.CurrentPlayer, PieceType.Pawn);
        pieceImages[fromPos.Row, fromPos.Column].Source = null;
        PromotionWindow promotionWindow = new PromotionWindow(gameState.CurrentPlayer);

        MenuContainer.Content = promotionWindow;

        promotionWindow.PieceSelected += type =>
        {
            MenuContainer.Content = null;
            Move promMove = new PawnPromotion(fromPos, toPos, type);
            HandleMove(promMove);
        };
    }

    private void HandleMove(Move move)
    {
        gameState.MakeMove(move);
        DrawBoard(gameState.Board);
        SetCursor(gameState.CurrentPlayer);

        HideLastMove();
        lastMove = move;
        ShowLastMove();
        if (gameState.IsGameOver())
        {
            ShowGameOver();
        }
        else
        {
            if (gameState.CurrentPlayer == Player.Black)
            {
                Move botMove = chessBot.GetBotMove(gameState);
                gameState.MakeMove(botMove);
                DrawBoard(gameState.Board);
                SetCursor(gameState.CurrentPlayer);

                HideLastMove();
                lastMove = botMove;
                ShowLastMove();

                if (gameState.IsGameOver())
                {
                    ShowGameOver();
                }
            }
        }
    }

    private Position ToSquarePosition(Point point)
    {
        double squareSize = BoardGrid.ActualWidth / 8;
        int row = (int)(point.Y / squareSize);
        int column = (int)(point.X / squareSize);
        return new Position(row, column);
    }

    private void CacheMoves(IEnumerable<Move> moves)
    {
        moveCache.Clear();

        foreach (Move move in moves)
        {
            moveCache[move.ToPos] = move;
        }
    }

    private void ShowHighlights()
    {
        Color color = Color.FromArgb(150, 125, 255, 125);

        foreach (Position to in moveCache.Keys)
        {
            highlights[to.Row, to.Column].Fill = new SolidColorBrush(color);
        }
    }

    private void HideHighlights()
    {
        foreach (Position to in moveCache.Keys)
        {
            highlights[to.Row, to.Column].Fill = Brushes.Transparent;
        }
    }

    private void SetCursor(Player player)
    {
        if (player == Player.White)
        {
            Cursor = ChessCursors.WhiteCursor;
        }
        else
        {
            Cursor = ChessCursors.BlackCursor;
        }
    }

    private bool IsMenuOnScreen()
    {
        return MenuContainer.Content != null;
    }

    private void ShowGameOver()
    {
        GameOverMenu gameOverMenu = new GameOverMenu(gameState);
        MenuContainer.Content = gameOverMenu;

        gameOverMenu.OptionSelected += option =>
        {
            if (option == Option.Restart)
            {
                MenuContainer.Content = null;
                RestartGame();
            }
            else
            {
                Application.Current.Shutdown();
            }
        };
    }

    private void RestartGame()
    {
        selectedPos = null;
        HideHighlights();
        moveCache.Clear();
        gameState = new GameState(Player.White, Board.Initial());
        DrawBoard(gameState.Board);
        SetCursor(gameState.CurrentPlayer);
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (!IsMenuOnScreen() && e.Key == Key.Escape)
        {
            ShowPauseMenu();
        }
    }

    private void ShowPauseMenu()
    {
        PauseMenu pauseMenu = new PauseMenu();
        MenuContainer.Content = pauseMenu;

        pauseMenu.OptionSelected += option =>
        {
            MenuContainer.Content = null;
            if (option == Option.Restart)
            {
                RestartGame();
            }
        };
    }

    private void BoardGrid_MouseMove(object sender, MouseEventArgs e)
    {
        if (IsMenuOnScreen())
        {
            return;
        }

        Point point = e.GetPosition(BoardGrid);
        Position pos = ToSquarePosition(point);

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (selectedPos == null)
            {
                OnFromPositionSelected(pos);
            }
            DragDrop.DoDragDrop(pieceImages[pos.Row, pos.Column], pieceImages[pos.Row, pos.Column], DragDropEffects.Move);
        }
    }

    private void BoardGrid_Drop(object sender, DragEventArgs e)
    {
        Point point = e.GetPosition(BoardGrid);
        Position pos = ToSquarePosition(point);

        if (selectedPos != null)
        {
            OnToPositionSelected(pos);
        }
    }

    private void BoardGrid_DragOver(object sender, DragEventArgs e)
    {
        if (selectedPos == null)
        {
            return;
        }

        Point point = e.GetPosition(BoardGrid);
    }

    private void ShowLastMove()
    {
        if(lastMove != null)
        {
            highlights[lastMove.FromPos.Row, lastMove.FromPos.Column].Fill = new SolidColorBrush(Color.FromArgb(150, 255, 255, 125));
            highlights[lastMove.ToPos.Row, lastMove.ToPos.Column].Fill = new SolidColorBrush(Color.FromArgb(150, 255, 255, 125));
        }
    }

    private void HideLastMove()
    {
        if (lastMove != null)
        {
            highlights[lastMove.FromPos.Row, lastMove.FromPos.Column].Fill = Brushes.Transparent;
            highlights[lastMove.ToPos.Row, lastMove.ToPos.Column].Fill = Brushes.Transparent;
        }
    }
}