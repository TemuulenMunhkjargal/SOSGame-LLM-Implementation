using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SOSGameLib;

namespace SOSGameUI
{
    public partial class GameWindow : Window
    {
        private SOSGameMain _game;
        private PlayerType _redPlayerType;
        private PlayerType _bluePlayerType;
        private IComputerPlayerStrategy _computerStrategy;

        public GameWindow(int boardSize, string gameMode,
            PlayerType redPlayerType, PlayerType bluePlayerType,
            string computerStrategyType = "Random")
        {
            InitializeComponent();

            // Set player types
            _redPlayerType = redPlayerType;
            _bluePlayerType = bluePlayerType;

            // Select computer strategy
            if (computerStrategyType == "Smart")
            {
                _computerStrategy = new SmartComputerStrategy();
            }
            else
            {
                _computerStrategy = new RandomComputerStrategy();
            }

            // Set game mode
            GameModeTextBlock.Text = gameMode;
            if (gameMode == "Simple")
            {
                _game = new SimpleGame(boardSize);
            }
            else
            {
                _game = new GeneralGame(boardSize);
            }

            InitializeBoard(boardSize);
            UpdateTurnDisplay();
            UpdateScoreDisplay();

            // Check if first move should be computer
            if (_game.CurrentPlayer == Player.Red && _redPlayerType == PlayerType.Computer ||
                _game.CurrentPlayer == Player.Blue && _bluePlayerType == PlayerType.Computer)
            {
                MakeComputerMove();
            }
        }
        private void MakeComputerMove()
        {
            if ((_game.CurrentPlayer == Player.Red && _redPlayerType == PlayerType.Computer) ||
                (_game.CurrentPlayer == Player.Blue && _bluePlayerType == PlayerType.Computer))
            {
                var move = _computerStrategy.GenerateMove(_game.Board, _game.Letters, _game.BoardSize);

                // Find the corresponding button
                Button targetButton = GetButtonAtPosition(move.row, move.col);

                if (targetButton != null)
                {
                    // Simulate button click with computer's move
                    SButton.IsChecked = move.letter == 'S';
                    OButton.IsChecked = move.letter == 'O';

                    // Trigger the click event
                    Cell_Click(targetButton, new RoutedEventArgs());
                }
            }
        }

        private Button GetButtonAtPosition(int row, int col)
        {
            foreach (UIElement child in GameGrid.Children)
            {
                if (child is Button button &&
                    Grid.GetRow(button) == row &&
                    Grid.GetColumn(button) == col)
                {
                    return button;
                }
            }
            return null;
        }


        private void InitializeBoard(int size)
        {
            GameGrid.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < size; i++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    Button button = new Button();
                    button.Click += Cell_Click;
                    GameGrid.Children.Add(button);
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                }
            }
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);

            char selectedLetter = SButton.IsChecked == true ? 'S' : 'O';

            if (_game.MakeMove(row, col, selectedLetter))
            {
                button.Content = selectedLetter;
                UpdateScoreDisplay();
                UpdateTurnDisplay();

                if (_game.GameOver)
                {
                    string winnerMessage = _game.Winner == Player.None ? "It's a draw!" : $"{_game.Winner} wins!";
                    MessageBox.Show(winnerMessage, "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Check for next computer move
                if ((_game.CurrentPlayer == Player.Red && _redPlayerType == PlayerType.Computer) ||
                    (_game.CurrentPlayer == Player.Blue && _bluePlayerType == PlayerType.Computer))
                {
                    // Use Dispatcher to ensure UI is updated before computer move
                    Dispatcher.Invoke(() =>
                    {
                        MakeComputerMove();
                    });
                }
            }
            else
            {
                MessageBox.Show("Invalid Move! Cell is occupied.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LetterButton_Click(object sender, RoutedEventArgs e)
        {
            var clickedButton = (ToggleButton)sender;
            
            // Ensure one button is always selected
            if (clickedButton == SButton)
            {
                OButton.IsChecked = false;
                SButton.IsChecked = true;
            }
            else
            {
                SButton.IsChecked = false;
                OButton.IsChecked = true;
            }
        }

        private void UpdateScoreDisplay()
        {
            if (_game is GeneralGame generalGame)
            {
                RedScoreTextBlock.Text = generalGame.RedScore.ToString();
                BlueScoreTextBlock.Text = generalGame.BlueScore.ToString();
            }
            else
            {
                RedScoreTextBlock.Text = "N/A";
                BlueScoreTextBlock.Text = "N/A";
            }
        }

        private void UpdateTurnDisplay()
        {
            CurrentTurnTextBlock.Text = _game.CurrentPlayer == Player.Red ? "Red's Turn" : "Blue's Turn";
        }

        private void EndGame_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Game Ended");
            this.Close();
        }
    }
}