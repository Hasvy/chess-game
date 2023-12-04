using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp3;

namespace WpfApp3
{
    class FiguresAllowedMoves
    {
        CellsList cells = MainWindow.cells;

        public FiguresAllowedMoves()
        {
            
        }

        public void Pawn_Allowed_Moves(int row, int col, char color)                          //r, c is starting position, writed when MouseDown
        {
            int length = 1;
            if (row == 1 || row == 6)
            {
                length = 2;
            }
            for (int i = 0; i < length; i++)
            {
                row = (color == 'w') ? row - 1 : row + 1;
                if (row >= 0 && row <= 7)
                {
                    Grid cell;
                    if (col > 0 && i == 0)
                    {
                        cell = cells.Find(row, col - 1).Name;
                        if (CheckFigureInCell(cell) != 'n' && CheckFigureInCell(cell) != color)
                        {
                            LightingCell(cell);
                        }
                    }
                    if (col < 7 && i == 0)
                    {
                        cell = cells.Find(row, col + 1).Name;
                        if (CheckFigureInCell(cell) != 'n' && CheckFigureInCell(cell) != color)
                        {
                            LightingCell(cell);
                        }
                    }
                    cell = cells.Find(row, col).Name;
                    if (CheckFigureInCell(cell) != 'n')
                    {
                        break;
                    }
                    LightingCell(cell);
                }
            }
        }

        public void King_Allowed_Moves(int row, int col, char color)
        {
            Grid cell;
            int xStart, yStart, xEnd, yEnd;
            yStart = (row == 7) ? 0 : -1;
            yEnd = (row == 0) ? 0 : 1;
            xStart = (col == 0) ? 0 : -1;
            xEnd = (col == 7) ? 0 : 1;
            for (int y = yStart; y <= yEnd; y++)
            {
                for (int x = xStart; x <= xEnd; x++)
                {
                    cell = cells.Find(row - y, col + x).Name;
                    if (!((y == 0 && x == 0) || (CheckFigureInCell(cell) == color)))
                    {
                        LightingCell(cell);
                    }
                }
            }
        }

        public void Queen_Allowed_Moves(int row, int col, char color)
        {
            DirectMoves(row, col, color);
            DiagonalMoves(row, col, color);
        }

        internal void Bishop_Allowed_Moves(int row, int col, char color)
        {
            DiagonalMoves(row, col, color);
        }
        internal void Knight_Allowed_Moves(int row, int col, char color)
        {
            if (row < 6)
            {
                if (col > 0)
                {
                    LightIfNotSameColor(cells.Find(row + 2, col - 1).Name, color);
                }
                if (col < 7)
                {
                    LightIfNotSameColor(cells.Find(row + 2, col + 1).Name, color);
                }
            }
            if (col > 1)
            {
                if (row > 0)
                {
                    LightIfNotSameColor(cells.Find(row - 1, col - 2).Name, color);
                }
                if (row < 7)
                {
                    LightIfNotSameColor(cells.Find(row + 1, col - 2).Name, color);
                }
            }
            if (row > 1)
            {
                if (col > 0)
                {
                    LightIfNotSameColor(cells.Find(row - 2, col - 1).Name, color);
                }
                if (col < 7)
                {
                    LightIfNotSameColor(cells.Find(row - 2, col + 1).Name, color);
                }
            }
            if (col < 6)
            {
                if (row > 0)
                {
                    LightIfNotSameColor(cells.Find(row - 1, col + 2).Name, color);
                }
                if (row < 7)
                {
                    LightIfNotSameColor(cells.Find(row + 1, col + 2).Name, color);
                }
            }
        }

        internal void Rook_Allowed_Moves(int row, int col, char color)
        {
            DirectMoves(row, col, color);
        }

        private void DirectMoves(int row, int col, char color)
        {
            bool up, down, left, right;
            up = down = left = right = true;
            for (int i = 1; i <= 7; i++)
            {
                if (row + i <= 7 && up)
                {
                    up = CheckFigureColor(cells.Find(row + i, col).Name, color);
                }
                if (row - i >= 0 && down)
                {
                    down = CheckFigureColor(cells.Find(row - i, col).Name, color);
                }
                if (col + i <= 7 && right)
                {
                    right = CheckFigureColor(cells.Find(row, col + i).Name, color);
                }
                if (col - i >= 0 && left)
                {
                    left = CheckFigureColor(cells.Find(row, col - i).Name, color);
                }
            }
        }

        private void DiagonalMoves(int row, int col, char color)
        {
            bool leftUp, leftDown, rightUp, rightDown;
            leftUp = leftDown = rightUp = rightDown = true;
            for (int i = 1; i <= 7; i++)
            {
                if (row + i <= 7 && col - i >= 0 && leftUp)
                {
                    leftUp = CheckFigureColor(cells.Find(row + i, col - i).Name, color);
                }
                if (row + i <= 7 && col + i <= 7 && rightUp)
                {
                    rightUp = CheckFigureColor(cells.Find(row + i, col + i).Name, color);
                }
                if (row - i >= 0 && col + i <= 7 && rightDown)
                {
                    rightDown = CheckFigureColor(cells.Find(row - i, col + i).Name, color);
                }
                if (row - i >= 0 && col - i >= 0 && leftDown)
                {
                    leftDown = CheckFigureColor(cells.Find(row - i, col - i).Name, color);
                }
            }
        }

        private void LightIfNotSameColor(Grid cell, char color)
        {
            if (CheckFigureInCell(cell) != color)
            {
                LightingCell(cell);
            }
        }

        private bool CheckFigureColor(Grid cell, char color)
        {
            if (CheckFigureInCell(cell) == 'n')
            {
                LightingCell(cell);
                return true;
            }
            if (CheckFigureInCell(cell) != color)
            {
                LightingCell(cell);
                return false;
            }
            else
            {
                return false;
            }
        }

        private char CheckFigureInCell(Grid cell)
        {
            foreach (UIElement item in cell.Children)
            {
                if (item.GetValue(FrameworkElement.TagProperty).ToString().Split(' ')[0] == "figure")
                {
                    return item.GetValue(FrameworkElement.NameProperty).ToString()[0];
                }
            }
            return 'n';
        }

        private void LightingCell(Grid cell)
        {
            foreach (UIElement item in cell.Children)
            {
                if (item.GetValue(FrameworkElement.TagProperty).ToString() == "border")
                {
                    item.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
