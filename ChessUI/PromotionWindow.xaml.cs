using ChessLogic;
using System.Windows.Controls;
using System.Windows.Input;

namespace ChessUI
{
    /// <summary>
    /// Interaction logic for PromotionWindow.xaml
    /// </summary>
    public partial class PromotionWindow : UserControl
    {
        public event Action<PieceType> PieceSelected;
        public PromotionWindow(Player player)
        {
            InitializeComponent();

            QueenImg.Source = Images.GetImage(player, PieceType.Queen);
            RookImg.Source = Images.GetImage(player, PieceType.Rook);
            BishopImg.Source = Images.GetImage(player, PieceType.Bishop);
            KnightImg.Source = Images.GetImage(player, PieceType.Knight);
            
        }

        private void QueenImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PieceSelected?.Invoke(PieceType.Queen);
        }

        private void RookImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PieceSelected?.Invoke(PieceType.Rook);
        }

        private void BishopImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PieceSelected?.Invoke(PieceType.Bishop);
        }

        private void KnightImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PieceSelected?.Invoke(PieceType.Knight);
        }
    }
}
