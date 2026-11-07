using AurOS;
using UnityEngine;
using Canvas = AurOS.Canvas;


namespace NumbersGoUp
{
    internal readonly struct Grid
    {
        public readonly int Columns;
        public readonly int Rows;
        public readonly int Cell;
        public readonly int Left;
        public readonly int Top;

        public int Width => Columns * Cell;
        public int Height => Rows * Cell;


        public Grid(int columns, int rows, int top = 22, int bottom = 6)
        {
            Columns = columns;
            Rows = rows;
            Cell = Math.Min((Canvas.Width - 16) / columns, (Canvas.Height - top - bottom) / rows);
            Left = (Canvas.Width - columns * Cell) / 2;
            Top = top + (Canvas.Height - top - bottom - rows * Cell) / 2;
        }


        public int X(int column) => Left + column * Cell;
        public int Y(int row) => Top + row * Cell;


        public void FillCell(Canvas canvas, int column, int row, Color32 color, int inset = 1)
        {
            canvas.Fill(X(column) + inset, Y(row) + inset, Cell - 2 * inset, Cell - 2 * inset, color);
        }


        public void TextCell(Canvas canvas, int column, int row, string text, Color32 color)
        {
            int size = Math.Max(6, Cell * 7 / 10);
            canvas.Text(text, X(column) + Cell / 2, Y(row) + (Cell - size) / 2, size, color, TextAlign.Center);
        }
    }
}
