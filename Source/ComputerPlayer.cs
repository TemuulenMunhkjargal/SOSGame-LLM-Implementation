using System;
using System.Collections.Generic;

namespace SOSGameLib
{
    public interface IComputerPlayerStrategy
    {
        (int row, int col, char letter) GenerateMove(Player[,] board, char[,] letters, int boardSize);
    }

    public class RandomComputerStrategy : IComputerPlayerStrategy
    {
        private Random _random = new Random();

        public (int row, int col, char letter) GenerateMove(Player[,] board, char[,] letters, int boardSize)
        {
            var availableCells = new List<(int row, int col)>();

            // Find all unoccupied cells
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (board[row, col] == Player.None)
                    {
                        availableCells.Add((row, col));
                    }
                }
            }

            if (availableCells.Count == 0)
                throw new InvalidOperationException("No moves available");

            // Select random cell
            var selectedCell = availableCells[_random.Next(availableCells.Count)];

            // Randomly choose letter
            char[] possibleLetters = { 'S', 'O' };
            char selectedLetter = possibleLetters[_random.Next(2)];

            return (selectedCell.row, selectedCell.col, selectedLetter);
        }
    }

    public class SmartComputerStrategy : IComputerPlayerStrategy
    {
        private Random _random = new Random();

        public (int row, int col, char letter) GenerateMove(Player[,] board, char[,] letters, int boardSize)
        {
            // Prioritize creating or blocking SOS sequences
            var potentialMoves = FindBestMove(board, letters, boardSize);

            // If no strategic move found, fall back to random
            if (potentialMoves.Count == 0)
            {
                var randomStrategy = new RandomComputerStrategy();
                return randomStrategy.GenerateMove(board, letters, boardSize);
            }

            // Choose a random move from potential strategic moves
            return potentialMoves[_random.Next(potentialMoves.Count)];
        }

        private List<(int row, int col, char letter)> FindBestMove(Player[,] board, char[,] letters, int boardSize)
        {
            var potentialMoves = new List<(int row, int col, char letter)>();

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (board[row, col] == Player.None)
                    {
                        // Check potential SOS formations
                        if (WouldCreateSOS(row, col, 'S', board, letters, boardSize))
                        {
                            potentialMoves.Add((row, col, 'S'));
                        }
                        if (WouldCreateSOS(row, col, 'O', board, letters, boardSize))
                        {
                            potentialMoves.Add((row, col, 'O'));
                        }
                    }
                }
            }

            return potentialMoves;
        }

        private bool WouldCreateSOS(int row, int col, char letter, Player[,] board, char[,] letters, int boardSize)
        {
            // Temporary placement
            char originalLetter = letters[row, col];
            letters[row, col] = letter;

            bool sosDetected = false;
            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

            if (letter == 'S')
            {
                for (int dir = 0; dir < 8; dir++)
                {
                    int r1 = row + dx[dir];
                    int c1 = col + dy[dir];
                    int r2 = r1 + dx[dir];
                    int c2 = c1 + dy[dir];

                    if (IsInBounds(r1, c1, boardSize) && IsInBounds(r2, c2, boardSize))
                    {
                        if (letters[r1, c1] == 'O' && letters[r2, c2] == 'S')
                        {
                            sosDetected = true;
                            break;
                        }
                    }
                }
            }
            else if (letter == 'O')
            {
                for (int dir = 0; dir < 8; dir++)
                {
                    int rPrev = row - dx[dir];
                    int cPrev = col - dy[dir];
                    int rNext = row + dx[dir];
                    int cNext = col + dy[dir];

                    if (IsInBounds(rPrev, cPrev, boardSize) && IsInBounds(rNext, cNext, boardSize))
                    {
                        if (letters[rPrev, cPrev] == 'S' && letters[rNext, cNext] == 'S')
                        {
                            sosDetected = true;
                            break;
                        }
                    }
                }
            }

            // Restore original state
            letters[row, col] = originalLetter;

            return sosDetected;
        }

        private bool IsInBounds(int row, int col, int boardSize)
        {
            return row >= 0 && row < boardSize && col >= 0 && col < boardSize;
        }
    }
}