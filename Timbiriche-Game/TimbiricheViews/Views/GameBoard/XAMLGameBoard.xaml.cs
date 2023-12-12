using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TimbiricheViews.Components.Match;
using TimbiricheViews.Player;
using TimbiricheViews.Server;
using TimbiricheViews.Utils;

namespace TimbiricheViews.Views
{
    public partial class XAMLGameBoard : Page
    {
        private const int BOARD_SIZE = 10;
        private const string HORIZONTAL_TYPE_LINE = "Horizontal";
        private const string VERTICAL_TYPE_LINE = "Vertical";

        private Server.Player _playerLoggedIn = PlayerSingleton.Player;
        private int[,] _horizontalLines;
        private int[,] _verticalLines;
        private int _row;
        private int _column;
        private string _username;
        private string _lobbyCode;
        private string _playerHexadecimalColor;
        private string _playerStylePath;
        private bool _itsMyTurn;

        public XAMLGameBoard(string lobbyCode, string playerHexadecimalColor, string playerStylePath)
        {
            InitializeComponent();
            InitializeGameBoard();
            
            _lobbyCode = lobbyCode;
            _playerHexadecimalColor = playerHexadecimalColor;
            _playerStylePath = playerStylePath;
            _username = _playerLoggedIn.Username;

            InitializeRegisterToTheMatch();
        }

        private void InitializeRegisterToTheMatch()
        {
            InstanceContext context = new InstanceContext(this);
            MatchManagerClient client = new MatchManagerClient(context);

            try
            {
                client.RegisterToTheMatch(_lobbyCode, _username, _playerHexadecimalColor);
            }
            catch (EndpointNotFoundException ex)
            {
                EmergentWindows.CreateConnectionFailedMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (TimeoutException ex)
            {
                EmergentWindows.CreateTimeOutMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (FaultException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                NavigationService.Navigate(new XAMLLogin());
            }
            catch (CommunicationException ex)
            {
                EmergentWindows.CreateServerErrorMessageWindow();
                HandlerException.HandleErrorException(ex, NavigationService);
            }
            catch (Exception ex)
            {
                EmergentWindows.CreateUnexpectedErrorMessageWindow();
                HandlerException.HandleFatalException(ex, NavigationService);

                NavigationService.Navigate(new XAMLLogin());
            }
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
            btnLine.Name = typeLine + "_" + row + "_" + column;
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

            Button btnLine = (Button)sender;
            string[] tagParts = btnLine.Tag.ToString().Split(SPLIT_SYMBOL);
            string typeLine = tagParts[INDEX_TYPE_LINE];
            _row = int.Parse(tagParts[INDEX_ROW]);
            _column = int.Parse(tagParts[INDEX_COLUMN]);

            if (_itsMyTurn)
            {
                SoundsUtilities.PlayButtonClicLineSound();
                string stylePlayer = ChooseCorrectStyle(_playerStylePath, _username);
                int points = SetMovement(btnLine, _playerHexadecimalColor, stylePlayer, _row, _column, typeLine);

                InstanceContext context = new InstanceContext(this);
                Server.MatchManagerClient client = new Server.MatchManagerClient(context);

                Movement movement = new Movement();
                movement.TypeLine = typeLine;
                movement.Row = _row;
                movement.Column = _column;
                movement.EarnedPoints = points;
                movement.HexadecimalColor = _playerHexadecimalColor;
                movement.StylePath = _playerStylePath;
                movement.Username = _username;

                client.EndTurn(_lobbyCode, movement);
            }
        }

        private string ChooseCorrectStyle(string stylePath, string username)
        {
            const string DEFAULT_STYLE = "";
            const int INDEX_FIRST_LETTER = 0;

            string stylePlayer = stylePath;
            if (stylePath == DEFAULT_STYLE)
            {
                stylePlayer = username[INDEX_FIRST_LETTER].ToString();
            }

            return stylePlayer;
        }

        private int SetMovement(Button btnLine, String colorPlayer, string stylePath, int row, int column, string typeLine)
        {
            UpdateButtonAppearance(btnLine, colorPlayer);
            MarkAsDrawed(row, column, typeLine);
            int points = ValidateSquares(row, column, typeLine, colorPlayer, stylePath);

            return points;
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
                _horizontalLines[row, column] = 1;
            }
            else if (isVerticalLine)
            {
                _verticalLines[row, column] = 1;
            }
        }
        
        private int ValidateSquares(int row, int column, string typeLine, string hexadecimalColor, string imageBoardPath)
        {
            bool isHorizontalLine = typeLine.Equals(HORIZONTAL_TYPE_LINE);
            bool isVerticalLine = typeLine.Equals(VERTICAL_TYPE_LINE);
            int points = 0;

            if (isHorizontalLine)
            {
                points += ValidateSquareAbove(row, column, hexadecimalColor, imageBoardPath);
                points += ValidateSquareBelow(row, column, hexadecimalColor, imageBoardPath);
            }
            else if(isVerticalLine)
            {
                points += ValidateSquareLeft(row, column, hexadecimalColor, imageBoardPath);
                points += ValidateSquareRight(row, column, hexadecimalColor, imageBoardPath);
            }

            return points;
        }

        private int ValidateSquareAbove(int row, int column, string hexadecimalColor, string imageBoardPath)
        {
            int points = 0;

            if (row > 0 && _horizontalLines[row - 1, column] != 0 && _verticalLines[row - 1, column] != 0 && _verticalLines[row - 1, column + 1] != 0)
            {
                points++;
                SetScoringPlayerOnBoard(row - 1, column, imageBoardPath, hexadecimalColor);

            }

            return points;
        }

        private int ValidateSquareBelow(int row, int column, string hexadecimalColor, string imageBoardPath)
        {
            int points = 0;

            if (row < BOARD_SIZE - 1 && _horizontalLines[row + 1, column] != 0 && _verticalLines[row, column] != 0 && _verticalLines[row, column + 1] != 0)
            {
                points++;
                SetScoringPlayerOnBoard(row, column, imageBoardPath, hexadecimalColor);
            }

            return points;
        }

        private int ValidateSquareLeft(int row, int column, string hexadecimalColor, string imageBoardPath)
        {
            int points = 0;

            if (column > 0 && _verticalLines[row, column - 1] != 0 && _horizontalLines[row, column - 1] != 0 && _horizontalLines[row + 1, column - 1] != 0)
            {
                points++;
                SetScoringPlayerOnBoard(row, column - 1, imageBoardPath, hexadecimalColor);
            }

            return points;
        }

        private int ValidateSquareRight(int row, int column, string hexadecimalColor, string imageBoardPath)
        {
            int points = 0;

            if (column < BOARD_SIZE - 1 && _verticalLines[row, column + 1] != 0 && _horizontalLines[row, column] != 0 && _horizontalLines[row + 1, column] != 0)
            {
                points++;
                SetScoringPlayerOnBoard(row, column, imageBoardPath, hexadecimalColor);
            }

            return points;
        }

        private void SetScoringPlayerOnBoard(int row, int column, string imageBoardPath, string hexadecimalColor)
        {
            Color color = (Color)ColorConverter.ConvertFromString(hexadecimalColor);
            SolidColorBrush colorBrush = new SolidColorBrush(color);

            Rectangle scoringPlayerColor = new Rectangle();
            scoringPlayerColor.Fill = colorBrush;

            Grid containerGrid = new Grid();
            containerGrid.Children.Add(scoringPlayerColor);
            SetFaceBoxOnSquareCompleted(imageBoardPath, containerGrid);

            Grid.SetRow(containerGrid, row);
            Grid.SetColumn(containerGrid, column);
        
            gridGameBoard.Children.Add(containerGrid);

            SoundsUtilities.PlaySquareCompleteSound();
        }

        private void SetFaceBoxOnSquareCompleted(string stylePath, Grid containerGrid)
        {
            const int DEFAULT_STYLE_LENGTH = 1;

            if (stylePath.Length == DEFAULT_STYLE_LENGTH)
            {
                Label lbFacebox = CreateFaceBoxLabel();
                lbFacebox.Content = stylePath;
                containerGrid.Children.Add(lbFacebox);
            }
            else
            {
                Image styleImage = Utilities.CreateImageByPath(stylePath);
                containerGrid.Children.Add(styleImage);
            }
        }

        private Label CreateFaceBoxLabel()
        {
            Label lbFacebox= XamlReader.Parse(XamlWriter.Save(lbFaceboxTemplate)) as Label;
            lbFacebox.IsEnabled = true;
            lbFacebox.Visibility = Visibility.Visible;

            return lbFacebox;
        }
    }
}
