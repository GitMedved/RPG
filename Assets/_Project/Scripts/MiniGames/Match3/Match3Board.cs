using System;
using System.Collections.Generic;

namespace MiaMax.Domain.MiniGames
{
    /// <summary>Portable 8x8 match-three rules engine; no Unity dependencies.</summary>
    public sealed class Match3Board
    {
        public const int DefaultWidth = 8;
        public const int DefaultHeight = 8;
        public const int DefaultGemKinds = 5;

        private readonly Random random;
        private readonly int gemKinds;
        private readonly int[,] cells;

        public int Width { get; }
        public int Height { get; }
        public int Score { get; private set; }
        public int MovesRemaining { get; private set; }
        public int[,] Snapshot
        {
            get
            {
                var copy = new int[Width, Height];
                Array.Copy(cells, copy, cells.Length);
                return copy;
            }
        }

        public Match3Board(int moves = 20, int seed = 0, int width = DefaultWidth,
            int height = DefaultHeight, int kinds = DefaultGemKinds)
        {
            if (width < 3 || height < 3 || kinds < 3 || moves < 1)
                throw new ArgumentOutOfRangeException("Board dimensions, gem kinds and moves must be valid.");
            Width = width;
            Height = height;
            gemKinds = kinds;
            MovesRemaining = moves;
            random = seed == 0 ? new Random() : new Random(seed);
            cells = new int[Width, Height];
            FillWithoutInitialMatches();
        }

        public int GetGem(int x, int y)
        {
            ValidateCell(x, y);
            return cells[x, y];
        }

        public bool TrySwap(int x1, int y1, int x2, int y2, out int pointsEarned)
        {
            pointsEarned = 0;
            if (MovesRemaining <= 0 || !Inside(x1, y1) || !Inside(x2, y2) ||
                Math.Abs(x1 - x2) + Math.Abs(y1 - y2) != 1)
                return false;

            Swap(x1, y1, x2, y2);
            List<int> matches = FindMatches();
            if (matches.Count == 0)
            {
                Swap(x1, y1, x2, y2);
                return false;
            }

            MovesRemaining--;
            int cascadeGuard = Width * Height;
            while (matches.Count > 0 && cascadeGuard-- > 0)
            {
                pointsEarned += matches.Count * 10;
                Score += matches.Count * 10;
                Clear(matches);
                CollapseAndRefill();
                matches = FindMatches();
            }
            if (MovesRemaining > 0 && !HasPossibleMove()) Shuffle();
            return true;
        }

        public bool HasPossibleMove()
        {
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                if (x + 1 < Width)
                {
                    Swap(x, y, x + 1, y);
                    bool match = FindMatches().Count > 0;
                    Swap(x, y, x + 1, y);
                    if (match) return true;
                }
                if (y + 1 < Height)
                {
                    Swap(x, y, x, y + 1);
                    bool match = FindMatches().Count > 0;
                    Swap(x, y, x, y + 1);
                    if (match) return true;
                }
            }
            return false;
        }

        private void FillWithoutInitialMatches()
        {
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                int gem;
                int attempts = 0;
                do
                {
                    gem = random.Next(gemKinds);
                    attempts++;
                }
                while (attempts < 100 && WouldMatchAt(x, y, gem));
                cells[x, y] = gem;
            }
            if (!HasPossibleMove()) Shuffle();
        }

        private bool WouldMatchAt(int x, int y, int gem)
        {
            return (x >= 2 && cells[x - 1, y] == gem && cells[x - 2, y] == gem) ||
                   (y >= 2 && cells[x, y - 1] == gem && cells[x, y - 2] == gem);
        }

        private List<int> FindMatches()
        {
            var indices = new HashSet<int>();
            for (int y = 0; y < Height; y++)
            {
                int start = 0;
                while (start < Width)
                {
                    int end = start + 1;
                    while (end < Width && cells[end, y] == cells[start, y]) end++;
                    if (cells[start, y] >= 0 && end - start >= 3)
                        for (int x = start; x < end; x++) indices.Add(y * Width + x);
                    start = end;
                }
            }
            for (int x = 0; x < Width; x++)
            {
                int start = 0;
                while (start < Height)
                {
                    int end = start + 1;
                    while (end < Height && cells[x, end] == cells[x, start]) end++;
                    if (cells[x, start] >= 0 && end - start >= 3)
                        for (int y = start; y < end; y++) indices.Add(y * Width + x);
                    start = end;
                }
            }
            return new List<int>(indices);
        }

        private void Clear(List<int> indices)
        {
            foreach (int index in indices) cells[index % Width, index / Width] = -1;
        }

        private void CollapseAndRefill()
        {
            for (int x = 0; x < Width; x++)
            {
                int writeY = 0;
                for (int y = 0; y < Height; y++)
                {
                    if (cells[x, y] < 0) continue;
                    cells[x, writeY++] = cells[x, y];
                }
                while (writeY < Height) cells[x, writeY++] = random.Next(gemKinds);
            }
        }

        private void Shuffle()
        {
            var values = new List<int>(Width * Height);
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++) values.Add(cells[x, y]);
            for (int i = values.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int temp = values[i]; values[i] = values[j]; values[j] = temp;
            }
            int index = 0;
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++) cells[x, y] = values[index++];
            if (FindMatches().Count > 0 || !HasPossibleMove())
            {
                for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++) cells[x, y] = random.Next(gemKinds);
                // Rebuild deterministically until a playable, match-free board is found.
                int guard = 0;
                while ((FindMatches().Count > 0 || !HasPossibleMove()) && guard++ < 1000)
                    for (int y = 0; y < Height; y++)
                    for (int x = 0; x < Width; x++) cells[x, y] = random.Next(gemKinds);
            }
        }

        private void Swap(int x1, int y1, int x2, int y2)
        {
            int value = cells[x1, y1]; cells[x1, y1] = cells[x2, y2]; cells[x2, y2] = value;
        }

        private bool Inside(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;
        private void ValidateCell(int x, int y)
        {
            if (!Inside(x, y)) throw new ArgumentOutOfRangeException();
        }
    }
}
