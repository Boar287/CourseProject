using System;

namespace GameOfLife
{
    [Serializable()]
    public class GameEngine
    {
        public uint CurrentGeneration { get; private set; }
        public uint Population { get; private set; }
        private bool[,] field;
        private readonly int COLS;
        private readonly int ROWS;
        public int Density { get; private set; }

        public GameEngine(int ROWS,int COLS,int Density)
        {
            Random random = new Random();
            this.ROWS = ROWS;
            this.COLS = COLS;
            this.Density = Density;
            field = new bool[COLS, ROWS];
            for(int x = 0; x < COLS; x++)
            {
                for(int y = 0; y < ROWS; y++)
                {
                    field[x, y] = random.Next(Density) == 0;
                }
            }
        }

        private int CountNeighbours(int x, int y)
        {
            int count = 0;

            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int col = (x + i + COLS) % COLS;
                    int row = (y + j + ROWS) % ROWS;
                    bool isCurrent = (col == x && row == y);
                    bool HasLife = field[col, row];

                    if (HasLife && !isCurrent)
                        count++;
                }
            }

            return count;
        }

        public void NextGeneration()
        {
            Population = 0;
            var NewField = new bool[COLS, ROWS];

            for (int x = 0; x < COLS; x++)
            {
                for (int y = 0; y < ROWS; y++)
                {
                    int NeighboursCount = CountNeighbours(x, y);
                    var HasLife = field[x, y];

                    if (!HasLife && NeighboursCount == 3)
                        NewField[x, y] = true;
                    else if (HasLife && (NeighboursCount < 2 || NeighboursCount > 3))
                        NewField[x, y] = false;
                    else
                        NewField[x, y] = field[x, y];

                    if (NewField[x, y])
                        Population++;
                }
            }
            field = NewField;
            CurrentGeneration++;
        }

        public bool[,] GetCurrentGeneration()
        {
            var result = new bool[COLS, ROWS];
            for(int x = 0; x < COLS; x++)
            {
                for(int y = 0; y < ROWS; y++)
                {
                    result[x, y] = field[x, y];
                }
            }
            return result;
        }
        private bool ValidateCellPosition(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < COLS && y < ROWS);
        }

        private void UpdateCell(int x,int y,bool state)
        {
            if (ValidateCellPosition(x, y))
                field[x, y] = state;
        }

        public void AddCell(int x, int y)
        {
            UpdateCell(x, y, state: true);
        }

        public void RemoveCell(int x,int y)
        {
            UpdateCell(x, y, state: false);
        }
    }
}
