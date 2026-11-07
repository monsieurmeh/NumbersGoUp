using AurOS;
using UnityEngine;
using Canvas = AurOS.Canvas;


namespace NumbersGoUp
{
    internal sealed class Game2048 : Game
    {
        private const int Size = 4;

        private enum Direction
        {
            Up = 1,
            Down = 2,
            Left = 3,
            Right = 4,
        }

        private static readonly Color32[] Colors =
        {
            new Color32(30, 70, 110, 255),
            new Color32(30, 100, 50, 255),
            new Color32(30, 100, 100, 255),
            new Color32(120, 35, 35, 255),
            new Color32(100, 35, 100, 255),
            new Color32(130, 110, 30, 255),
            new Color32(50, 110, 220, 255),
            new Color32(220, 60, 60, 255),
            new Color32(200, 70, 200, 255),
        };

        private readonly Grid mGrid = new Grid(Size, Size);
        private int?[,] mBoard = new int?[Size, Size];
        private int mScore;
        private bool mGameOver;

        public override string Name => "Numbers Go Up";
        protected override Prompt[] Prompts => new[]
        {
            new Prompt(PromptActions.Move, "Slide"),
            new Prompt(PromptActions.Reload, "New Game"),
            new Prompt(PromptActions.Escape, "Exit"),
        };


        protected override void NewGame()
        {
            mBoard = new int?[Size, Size];
            mScore = 0;
            mGameOver = false;
            AddTile();
        }


        protected override void Play(Controls controls, float deltaTime)
        {
            if (mGameOver)
            {
                return;
            }
            Vector2Int step = controls.Step;
            Direction? direction = step.y > 0 ? Direction.Up : step.y < 0 ? Direction.Down : step.x < 0 ? Direction.Left : step.x > 0 ? Direction.Right : null;
            if (direction != null && TryUpdate(mBoard, ref mScore, direction.Value))
            {
                AddTile();
            }
        }


        public override void Draw(Canvas canvas)
        {
            canvas.Clear(Palette.Black);
            Title(canvas, Name, mGameOver ? $"Game Over. Score: {mScore}" : $"Score: {mScore}");
            for (int i = Size - 1; i >= 0; i--)
            {
                for (int j = 0; j < Size; j++)
                {
                    int row = Size - 1 - i;
                    int? value = mBoard[i, j];
                    mGrid.FillCell(canvas, j, row, value is null ? Palette.Faint : Colors[(int)(Math.Log2(value.Value) - 1) % Colors.Length], 3);
                    if (value is not null)
                    {
                        mGrid.TextCell(canvas, j, row, value.Value.ToString(), Palette.White);
                    }
                }
            }
        }


        private void AddTile()
        {
            List<(int X, int Y)> empty = new List<(int X, int Y)>();
            foreach (var (x, y) in BoardValues(mBoard))
            {
                if (mBoard[x, y] is null)
                {
                    empty.Add((x, y));
                }
            }
            if (empty.Count is 0)
            {
                mGameOver = true;
                return;
            }
            var (tx, ty) = empty[Random.Next(0, empty.Count)];
            mBoard[tx, ty] = Random.Next(10) < 9 ? 2 : 4;
            mScore += 2;
            int score = mScore;
            if (!TryUpdate((int?[,])mBoard.Clone(), ref score, Direction.Up) &&
                !TryUpdate((int?[,])mBoard.Clone(), ref score, Direction.Down) &&
                !TryUpdate((int?[,])mBoard.Clone(), ref score, Direction.Left) &&
                !TryUpdate((int?[,])mBoard.Clone(), ref score, Direction.Right))
            {
                mGameOver = true;
            }
        }


        private static bool TryUpdate(int?[,] board, ref int score, Direction direction)
        {
            (int X, int Y) Adjacent(int x, int y) =>
                direction switch
                {
                    Direction.Up => (x + 1, y),
                    Direction.Down => (x - 1, y),
                    Direction.Left => (x, y - 1),
                    Direction.Right => (x, y + 1),
                    _ => throw new NotImplementedException(),
                };

            (int X, int Y) Map(int x, int y) =>
                direction switch
                {
                    Direction.Up => (board.GetLength(0) - x - 1, y),
                    Direction.Down => (x, y),
                    Direction.Left => (x, y),
                    Direction.Right => (x, board.GetLength(1) - y - 1),
                    _ => throw new NotImplementedException(),
                };

            bool[,] locked = new bool[board.GetLength(0), board.GetLength(1)];
            bool update = false;
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    var (tempi, tempj) = Map(i, j);
                    if (board[tempi, tempj] is null)
                    {
                        continue;
                    }
                    while (true)
                    {
                        var (adji, adjj) = Adjacent(tempi, tempj);
                        if (adji < 0 || adji >= board.GetLength(0) ||
                            adjj < 0 || adjj >= board.GetLength(1) ||
                            locked[adji, adjj])
                        {
                            break;
                        }
                        if (board[adji, adjj] is null)
                        {
                            board[adji, adjj] = board[tempi, tempj];
                            board[tempi, tempj] = null;
                            update = true;
                            tempi = adji;
                            tempj = adjj;
                            continue;
                        }
                        if (board[adji, adjj] == board[tempi, tempj])
                        {
                            board[adji, adjj] += board[tempi, tempj];
                            score += board[adji, adjj]!.Value;
                            board[tempi, tempj] = null;
                            update = true;
                            locked[adji, adjj] = true;
                        }
                        break;
                    }
                }
            }
            return update;
        }


        private static IEnumerable<(int, int)> BoardValues(int?[,] board)
        {
            for (int i = board.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    yield return (i, j);
                }
            }
        }
    }
}
