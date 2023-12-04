using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp3
{
    public class Cell
    {
        public Cell()
        {

        }

        public Cell(Grid uIElement, int row, int column)
        {
            Name = uIElement;
            Row = row;
            Column = column;
        }
        public Grid Name { get; }
        public int Row { get; }
        public int Column { get; }
    }

    public class CellsList
    {
        public List<Cell> cells = new List<Cell>();

        public void Add(Cell item)
        {
            cells.Add(item);
        }

        public Cell Find(int r, int c)
        {
            return cells.Find(x => x.Row == r && x.Column == c);
        }

        public IEnumerator<Cell> GetEnumerator()
        {
            return cells.GetEnumerator();
        }
    }

    public partial class MainWindow : Window
    {
        public static CellsList cells = new CellsList();
        private List<UIElement> allFigures = new List<UIElement>();
        private List<UIElement> allBorders = new List<UIElement>();
        private UIElement figure;
        private FiguresAllowedMoves moves = new FiguresAllowedMoves();
        private int startRow;
        private int startColumn;
        private string player = "white";
        private char selectedFigureColor;
        public static Thickness thickActive = new Thickness(4, 4, 4, 4);
        public static Thickness thickDisable = new Thickness(0, 0, 0, 0);

        public MainWindow()
        {
            InitializeComponent();
            InitializeGameField();
        }

        public void InitializeGameField()
        {
            Grid cell;
            Grid cellForBorder;
            foreach (UIElement item in grid.Children)
            {
                try
                {
                    if (item.GetValue(TagProperty).ToString() == "cell")
                    {
                        Border border = new Border();
                        border.BorderBrush = Brushes.ForestGreen;
                        border.BorderThickness = thickActive;
                        border.Tag = "border";
                        border.Visibility = Visibility.Hidden;
                        cells.Add(new Cell((Grid)item, Grid.GetRow(item), Grid.GetColumn(item)));
                        cellForBorder = (Grid)item;
                        cellForBorder.Children.Add(border);
                        allBorders.Add(border);
                    }

                    if (item.GetValue(TagProperty).ToString().Split(' ')[0] == "figure")
                    {
                        allFigures.Add(item);
                    }
                }
                catch (NullReferenceException)
                {

                }
            }
            foreach (UIElement item in allFigures)
            {
                cell = ReturnCell(item);
                grid.Children.Remove(item);
                cell.Children.Add(item);
            }
        }

        private void Figure_MouseMove(object sender, MouseEventArgs e)
        {
            UIElement element = (UIElement)e.Source;
            lblCursorPosition.Text = $"Column: {startColumn}, Row {startRow}, {element}, {element.GetValue(NameProperty)}," +
                $" Parent {VisualTreeHelper.GetParent(element).GetValue(NameProperty)}";
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragDropEffects dragDropEffect = DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
                    if (dragDropEffect == DragDropEffects.None)
                    {
                        grid.Children.Remove(figure);
                        Grid cell = cells.Find(startRow, startColumn).Name;
                        cell.Children.Add(figure);
                        Grid.SetRow(figure, startRow);
                        Grid.SetColumn(figure, startColumn);
                        BorderDisable();
                        figure = null;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement element = (UIElement)e.Source;
            if (element is Image && player[0] == element.GetValue(NameProperty).ToString()[0])
            {
                figure = element;
                startRow = Grid.GetRow(figure);
                startColumn = Grid.GetColumn(figure);
                Grid cell = ReturnCell(figure);
                selectedFigureColor = figure.GetValue(NameProperty).ToString()[0];
                Panel.SetZIndex(figure, 2);
                cell.Children.Remove(figure);
                grid.Children.Add(figure);

                lblCursorPosition.Text = $"Column: {startColumn}, Row {startRow}, {element}, {element.GetValue(NameProperty)}," +
                    $" Parent {VisualTreeHelper.GetParent(element).GetValue(NameProperty)}";

                switch (figure.GetValue(TagProperty).ToString().Split(' ')[1])
                {
                    case "pawn":
                        moves.Pawn_Allowed_Moves(startRow, startColumn, selectedFigureColor);
                        break;
                    case "king":
                        moves.King_Allowed_Moves(startRow, startColumn, selectedFigureColor);
                        break;
                    case "queen":
                        moves.Queen_Allowed_Moves(startRow, startColumn, selectedFigureColor);
                        break;                    
                    case "bishop":
                        moves.Bishop_Allowed_Moves(startRow, startColumn, selectedFigureColor);
                        break;                    
                    case "knight":
                        moves.Knight_Allowed_Moves(startRow, startColumn, selectedFigureColor);
                        break;                  
                    case "rook":
                        moves.Rook_Allowed_Moves(startRow, startColumn, selectedFigureColor);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                figure = null;
            }
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Grid cell = cells.Find(startRow, startColumn).Name;
            if (figure != null)
            {
                Grid cell = ReturnCell(figure);
                //grid.Children.Remove(item);
                //cell.Children.Add(item);
                grid.Children.Remove(figure);
                cell.Children.Add(figure);
                //Grid.SetRow(figure, startRow);
                //Grid.SetColumn(figure, startColumn);
            }
            BorderDisable();
            figure = null;
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            var element = (UIElement)e.Source;
            int lastRow = Grid.GetRow(element);
            int lastColumn = Grid.GetColumn(element);
            Grid cell = cells.Find(lastRow, lastColumn).Name;
            try
            {
                if (allBorders.Find(x => VisualTreeHelper.GetParent(x) == cell).Visibility == Visibility.Visible || (lastRow == startRow && lastColumn == startColumn))
                {
                    Grid.SetRow(figure, lastRow);
                    Grid.SetColumn(figure, lastColumn);
                }
            }
            catch (ArgumentNullException)
            {

            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            try
            {
                Panel.SetZIndex(figure, 1);
                Grid cell = ReturnCell(figure);
                //allBorders.Find(x => VisualTreeHelper.GetParent(x) == cell)
                if (allBorders.Find(x => VisualTreeHelper.GetParent(x) == cell).Visibility == Visibility.Visible)
                {
                    grid.Children.Remove(figure);
                    foreach (UIElement item in cell.Children)
                    {
                        if (item is Image)
                        {
                            cell.Children.Remove(item);
                            break;
                        }
                    }
                    cell.Children.Add(figure);
                    ChangePlayer();
                }
                else
                {
                    cell = ReturnCell(figure);
                    grid.Children.Remove(figure);
                    //cell = cells.Find(startRow, startColumn).Name;
                    cell.Children.Add(figure);
                }
                BorderDisable();
                figure = null;
            }
            catch (ArgumentNullException)
            {

            }
        }

        private Grid ReturnCell(UIElement element)
        {
            int row = Grid.GetRow(element);
            int column = Grid.GetColumn(element);
            return cells.Find(row, column).Name;
        }

        private void BorderDisable()
        {
            foreach (Cell item in cells)
            {
                foreach (UIElement x in item.Name.Children)
                {
                    if (x.GetValue(TagProperty).ToString() == "border")
                    {
                        x.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void ChangePlayer()
        {
            player = (player == "white") ? "black" : "white";
        }
    }
}
