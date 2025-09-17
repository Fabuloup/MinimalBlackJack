using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinimBlackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BlackjackGame game = BlackjackGame.Instance;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = game;
        }

        #region Actions
        private void PlaceBetButton_Click(object sender, RoutedEventArgs e)
        {
            game.PlaceBet();
        }
        private void RemoveBetButton_Click(object sender, RoutedEventArgs e)
        {
            game.RemoveBet();
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            game.Reset();
            game.DealInitialCards();
            game.CheckBlackjack();
        }

        private void HitButton_Click(object sender, RoutedEventArgs e)
        {
            game.DealCardToPlayer();
            game.CheckBlackjack();
        }

        private async void StandButton_Click(object sender, RoutedEventArgs e)
        {

            await Task.Run(async () =>
            {
                await game.DealerTurn();
                await game.CheckWinner();
            });
        }

        // Événement du bouton Reset
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            game.FullReset();
        }
        #endregion
    }
}