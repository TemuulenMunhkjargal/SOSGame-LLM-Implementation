using System.Windows;
using System.Windows.Controls;
using SOSGameLib;

namespace SOSGameUI
{
    public partial class InitialMenuWindow : Window
    {
        public InitialMenuWindow()
        {
            InitializeComponent();
        }

        private void PlayGame_Click(object sender, RoutedEventArgs e)
        {
            // Validate input
            if (!int.TryParse(BoardSizeTextBox.Text, out int boardSize) || boardSize < 3)
            {
                MessageBox.Show("Please enter a valid board size (3 or greater).", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Get user selections
            string gameMode = (GameModeComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            PlayerType redPlayerType = (RedPlayerTypeComboBox.SelectedItem as ComboBoxItem).Content.ToString() == "Human"
                ? PlayerType.Human : PlayerType.Computer;
            PlayerType bluePlayerType = (BluePlayerTypeComboBox.SelectedItem as ComboBoxItem).Content.ToString() == "Human"
                ? PlayerType.Human : PlayerType.Computer;
            string computerStrategy = (ComputerStrategyComboBox.SelectedItem as ComboBoxItem).Content.ToString();

            // Open Game Window and pass values
            GameWindow gameWindow = new GameWindow(boardSize, gameMode, redPlayerType, bluePlayerType, computerStrategy);
            gameWindow.Show();
            this.Close();  // Close the initial menu window
        }
    }
}