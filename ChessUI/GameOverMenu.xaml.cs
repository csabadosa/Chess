using ChessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for GameOverMenu.xaml
    /// </summary>
    public partial class GameOverMenu : UserControl
    {
        public event Action<Option> OptionSelected;
        public GameOverMenu(GameState gameState)
        {
            InitializeComponent();
            Result result = gameState.Result;
            WinnerText.Text = GetWinnerText(result.Winner);
            ReasonText.Text = GetReasonText(result.Reason, gameState.CurrentPlayer);
        }

        private static string GetWinnerText(Player winner)
        {
            return winner switch
            {
                Player.White => "White Wins",
                Player.Black => "Black Wins",
                _ => "It's a Draw",
            };
        }

        private static string PlayerString(Player player)
        {
            return player switch
            {
                Player.White => "WHITE",
                Player.Black => "BLACK",
                _ => ""
            };
        }

        private static string GetReasonText(EndReason reason, Player currentPlayer)
        {
            return reason switch
            {
                EndReason.Stalemate => $"Stalemate - {PlayerString(currentPlayer)} can't move!",
                EndReason.Checkmate => $"Checkmate - {PlayerString(currentPlayer)} can't move!",
                EndReason.FiftyMoveRule => "Fifty-move rule",
                EndReason.ThreefoldRepetition => "Threefold repetition",
                EndReason.InsufficientMaterial => "Insufficient material",
                _ => ""
            };
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Restart);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected?.Invoke(Option.Exit);
        }

        private void GameOverBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(GameOverBox, GameOverBox, DragDropEffects.Move);
            }
        }
    }
}
