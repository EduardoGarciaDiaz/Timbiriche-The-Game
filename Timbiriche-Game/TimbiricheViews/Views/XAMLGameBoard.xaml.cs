using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimbiricheViews.Player;

namespace TimbiricheViews.Views
{
    public partial class XAMLGameBoard : Page
    {
        private int[,] _horizontalLines;
        private int[,] _verticalLines;
        private int _row;
        private int _column;
        private const int BOARD_SIZE = 10;
        const string HORIZONTAL_TYPE_LINE = "Horizontal";
        const string VERTICAL_TYPE_LINE = "Vertical";

        private bool _itsMyTurn;
        private string _lobbyCode;


        public XAMLGameBoard(string lobbyCode)
        {
            _lobbyCode = lobbyCode;
            InitializeComponent();
            InitializeGameBoard();
        }

        private void InitializeGameBoard()
        {
            _horizontalLines = new int[BOARD_SIZE, BOARD_SIZE - 1];
            _verticalLines = new int[BOARD_SIZE - 1, BOARD_SIZE];
            CreateButtonLines();
            _itsMyTurn = true;
        }

        private void CreateButtonLines()
        {
            CreateLines(BOARD_SIZE, BOARD_SIZE - 1, btnHorizontalLineTemplate, HORIZONTAL_TYPE_LINE);
            CreateLines(BOARD_SIZE - 1, BOARD_SIZE, btnVerticalLineTemplate, VERTICAL_TYPE_LINE);
        }

        private void CreateLines(int rows, int cols, Button buttonTemplate, string typeLine)
        {
            for (int currentRow = 0 ; currentRow < rows ; currentRow++)
            {
                for (int currentColumn = 0 ; currentColumn < cols ; currentColumn++)
                {
                    Button btnLine = CreateButton(buttonTemplate, typeLine, currentRow, currentColumn);
                    gridGameBoard.Children.Add(btnLine);
                }
            }
        }

        private Button CreateButton(Button buttonTemplate, string typeLine, int row, int column)
        {
            Button btnLine = XamlReader.Parse(XamlWriter.Save(buttonTemplate)) as Button;
            btnLine.Click += BtnLine_Click;
            btnLine.Name = typeLine + "Q" + row + "Q" + column;
            btnLine.Tag = typeLine + "," + row + "," + column;
            btnLine.IsEnabled = true;
            btnLine.Visibility = Visibility.Visible;
            Grid.SetRow(btnLine, row);
            Grid.SetColumn(btnLine, column);
            return btnLine;
        }

        private void BtnLine_Click(object sender, RoutedEventArgs e)
        {
            const char SPLIT_SYMBOL = ',';
            const int INDEX_TYPE_LINE = 0;
            const int INDEX_ROW = 1;
            const int INDEX_COLUMN = 2;
            string colorPlayer = "#FFAC4C4C";

            Button btnLine = (Button)sender;
            string[] tagParts = btnLine.Tag.ToString().Split(SPLIT_SYMBOL);
            string typeLine = tagParts[INDEX_TYPE_LINE];
            _row = int.Parse(tagParts[INDEX_ROW]);
            _column = int.Parse(tagParts[INDEX_COLUMN]);

            if (_itsMyTurn)
            {
                SetMovement(btnLine, colorPlayer, _row, _column, typeLine);

                InstanceContext context = new InstanceContext(this);
                Server.MatchManagerClient client = new Server.MatchManagerClient(context);
                client.EndTurn(_lobbyCode, typeLine, _row, _column);
            }
        }

        private void SetMovement(Button btnLine, String colorPlayer, int row, int column, string typeLine)
        {
            UpdateButtonAppearance(btnLine, colorPlayer);
            MarkAsDrawed(row, column, typeLine);
            ValidateSquares(row, column, typeLine);
        }

        private void UpdateButtonAppearance(Button btnLine, string color)
        {
            btnLine.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            btnLine.Style = (Style)FindResource("NoHoverStyle");
            btnLine.IsEnabled = false;
        }

        private void MarkAsDrawed(int row, int column, string typeLine)
        {
            bool isHorizontalLine = typeLine.Equals(HORIZONTAL_TYPE_LINE);
            bool isVerticalLine = typeLine.Equals(VERTICAL_TYPE_LINE);

            if (isHorizontalLine)
            {
                _horizontalLines[row, column] = 1;  // TODO: Change the value according to the player
            }
            else if (isVerticalLine)
            {
                _verticalLines[row, column] = 1;    // TODO: Change the value according to the player
            }
        }
        
        private void ValidateSquares(int row, int column, string typeLine)
        {
            bool isHorizontalLine = typeLine.Equals(HORIZONTAL_TYPE_LINE);
            bool isVerticalLine = typeLine.Equals(VERTICAL_TYPE_LINE);

            if (isHorizontalLine)
            {
                ValidateSquareAbove(row, column);
                ValidateSquareBelow(row, column);
            }
            else if(isVerticalLine)
            {
                ValidateSquareLeft(row, column);
                ValidateSquareRight(row, column);
            }
        }

        private void ValidateSquareAbove(int row, int column)
        {
            if (row > 0 && _horizontalLines[row - 1, column] != 0 && _verticalLines[row - 1, column] != 0 && _verticalLines[row - 1, column + 1] != 0)
            {
                Console.WriteLine("CUADRADO COMPLETADO desde Horizontal hacia arriba");
                SetImageBoard(row - 1, column);
            }
        }

        private void ValidateSquareBelow(int row, int column)
        {
            if (row < BOARD_SIZE - 1 && _horizontalLines[row + 1, column] != 0 && _verticalLines[row, column] != 0 && _verticalLines[row, column + 1] != 0)
            {
                Console.WriteLine("CUADRADO COMPLETADO desde Horizontal hacia abajo");
                SetImageBoard(row, column);
            }
        }

        private void ValidateSquareLeft(int row, int column)
        {
            if (column > 0 && _verticalLines[row, column - 1] != 0 && _horizontalLines[row, column - 1] != 0 && _horizontalLines[row + 1, column - 1] != 0)
            {
                Console.WriteLine("CUADRADO COMPLETADO desde Vertical hacia izquierda");
                SetImageBoard(row, column - 1);
            }
        }

        private void ValidateSquareRight(int row, int column)
        {
            if (column < BOARD_SIZE - 1 && _verticalLines[row, column + 1] != 0 && _horizontalLines[row, column] != 0 && _horizontalLines[row + 1, column] != 0)
            {
                Console.WriteLine("CUADRADO COMPLETADO desde Vertical hacia derecha");
                SetImageBoard(row, column);
            }
        }

        private void SetImageBoard(int row, int column)
        {
            Image imageWhoScore = new Image();
            // TODO: Utilities for transform Images
            BitmapImage bitmapImage = new BitmapImage(new Uri("../Resources/close.png", UriKind.RelativeOrAbsolute));
            imageWhoScore.Source = bitmapImage;
            Grid.SetRow(imageWhoScore, row);
            Grid.SetColumn(imageWhoScore, column);
            gridGameBoard.Children.Add(imageWhoScore);
        }

    }

    public partial class XAMLGameBoard : Server.IMatchManagerCallback
    {
        public void NotifyMovement(string typeLine, int row, int column)
        {
            Button btnLine = FindButtonByName(typeLine + "Q" + row + "Q" + column);
            SetMovement(btnLine, "#FFAC4C4C", row, column, typeLine);
        }

        public void NotifyNewTurn(string username)
        {
            if(PlayerSingleton.Player.Username == username)
            {
                _itsMyTurn = true;
            }
            else
            {
                _itsMyTurn = false;
            }
        }

        private Button FindButtonByName(string name)
        {
            return (Button)LogicalTreeHelper.FindLogicalNode(this, name);
        }
    }
}
