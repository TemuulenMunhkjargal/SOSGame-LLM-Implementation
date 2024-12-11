using System;

namespace SOSGameLib
{
    public enum GameMode { Simple, General }
    public enum Player { None, Red, Blue }
     // New enum for player types
     public enum PlayerType { Human, Computer }

    public abstract class SOSGameMain
    {
        public int BoardSize { get; private set; }
        public abstract int RedScore { get; set; }
        public abstract int BlueScore { get; set; }
        public Player[,] Board { get; private set; }
        public char[,] Letters { get; private set; }  // New field to store S/O letters
        public Player CurrentPlayer { get; set; }
        public bool GameOver { get; protected set; }
        public Player Winner { get; protected set; }

        public SOSGameMain(int boardSize)
        {
            if (boardSize < 3)
                throw new ArgumentException("Board size must be greater than 2.");

            BoardSize = boardSize;
            Board = new Player[BoardSize, BoardSize];
            Letters = new char[BoardSize, BoardSize];  // Initialize letters array
            CurrentPlayer = Player.Red;
            GameOver = false;
            Winner = Player.None;

            // Initialize Letters array with empty spaces
            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    Letters[i, j] = ' ';
        }

        public abstract bool MakeMove(int row, int col, char letter);

        protected bool CheckForSOS(int row, int col, char letter)
        {
            // Store the letter in the Letters array
            Letters[row, col] = letter;

            bool sosDetected = false;

            // Check in all 8 directions
            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

            if (letter == 'S')
            {
                // Check if this 'S' forms the start of an SOS
                for (int dir = 0; dir < 8; dir++)
                {
                    int r1 = row + dx[dir];
                    int c1 = col + dy[dir];
                    int r2 = r1 + dx[dir];
                    int c2 = c1 + dy[dir];

                    if (IsInBounds(r1, c1) && IsInBounds(r2, c2))
                    {
                        if (Letters[r1, c1] == 'O' && Letters[r2, c2] == 'S')
                        {
                            sosDetected = true;
                        }
                    }
                }

                // Check if this 'S' forms the end of an SOS
                for (int dir = 0; dir < 8; dir++)
                {
                    int r1 = row - dx[dir];
                    int c1 = col - dy[dir];
                    int r2 = r1 - dx[dir];
                    int c2 = c1 - dy[dir];

                    if (IsInBounds(r1, c1) && IsInBounds(r2, c2))
                    {
                        if (Letters[r1, c1] == 'O' && Letters[r2, c2] == 'S')
                        {
                            sosDetected = true;
                        }
                    }
                }
            }
            else if (letter == 'O')
            {
                // Check if this 'O' forms the middle of an SOS
                for (int dir = 0; dir < 8; dir++)
                {
                    int rPrev = row - dx[dir];
                    int cPrev = col - dy[dir];
                    int rNext = row + dx[dir];
                    int cNext = col + dy[dir];

                    if (IsInBounds(rPrev, cPrev) && IsInBounds(rNext, cNext))
                    {
                        if (Letters[rPrev, cPrev] == 'S' && Letters[rNext, cNext] == 'S')
                        {
                            sosDetected = true;
                        }
                    }
                }
            }

            return sosDetected;
        }

        private bool IsInBounds(int row, int col)
        {
            return row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;
        }
        public bool IsBoardFull()
        {
            for (int i = 0; i < BoardSize; i++)
                for (int j = 0; j < BoardSize; j++)
                    if (Board[i, j] == Player.None)
                        return false;
            return true;
        }
    }

    public class SimpleGame : SOSGameMain
    {
        public override int RedScore { get; set; }
        public override int BlueScore { get; set; }

        public SimpleGame(int boardSize) : base(boardSize) { }

        public override bool MakeMove(int row, int col, char letter)
        {
            if (GameOver || Board[row, col] != Player.None) return false;

            Board[row, col] = CurrentPlayer;

            if (CheckForSOS(row, col, letter))
            {
                Winner = CurrentPlayer;
                GameOver = true;
            }
            else
            {
                // Switch turns only if no SOS was formed
                CurrentPlayer = CurrentPlayer == Player.Red ? Player.Blue : Player.Red;
            }
            if (IsBoardFull())
            {
                GameOver = true;
                Winner = Player.None;  // Draw
            }

            return true;
        }
    }

    public class GeneralGame : SOSGameMain
    {
        public override int RedScore { get; set; }
        public override int BlueScore { get; set; }

        public GeneralGame(int boardSize) : base(boardSize)
        {
            RedScore = 0;
            BlueScore = 0;
        }

        public override bool MakeMove(int row, int col, char letter)
        {
            if (GameOver || Board[row, col] != Player.None) return false;

            Board[row, col] = CurrentPlayer;

            if (CheckForSOS(row, col, letter))
            {
                // Increment the score of the current player
                if (CurrentPlayer == Player.Red)
                    RedScore++;
                else
                    BlueScore++;
            }
            else
            {
                // Switch turns only if no SOS was formed
                CurrentPlayer = CurrentPlayer == Player.Red ? Player.Blue : Player.Red;
            }

            // Check if the board is full
            if (IsBoardFull())
            {
                GameOver = true;
                if (RedScore > BlueScore)
                    Winner = Player.Red;
                else if (BlueScore > RedScore)
                    Winner = Player.Blue;
                else
                    Winner = Player.None;  // Draw
            }

            return true;
        }

    }
}

