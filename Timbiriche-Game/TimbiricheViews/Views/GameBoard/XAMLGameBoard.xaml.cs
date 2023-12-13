using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private const int GAMEBOARD_BOUNDARY = 0;
        private int[,] _horizontalLines;
        private int[,] _verticalLines;
        private int _row;
        private int _column;
        private string _username;
        private string _lobbyCode;
        private string _playerHexadecimalColor;
        private string _playerStylePath;
        private bool _itsMyTurn;
        private Server.Player _playerLoggedIn = PlayerSingleton.Player;

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
            catch (FaultException)
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
            int initialGrid = 0;

            for (int currentRow = initialGrid; currentRow < rows ; currentRow++)
            {
                for (int currentColumn = initialGrid; currentColumn < cols ; currentColumn++)
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
            const char splitSymbol = ',';
            const int indexTypeLine = 0;
            const int indexRow = 1;
            const int indexColumn = 2;

            Button btnLine = (Button)sender;
            string[] tagParts = btnLine.Tag.ToString().Split(splitSymbol);
            string typeLine = tagParts[indexTypeLine];
            _row = int.Parse(tagParts[indexRow]);
            _column = int.Parse(tagParts[indexColumn]);

            DrawLine(btnLine, typeLine);
        }

        private void DrawLine(Button btnLine, string typeLine)
        {
            if (_itsMyTurn)
            {
                SoundsUtilities.PlayButtonClicLineSound();
                string stylePlayer = ChooseCorrectStyle(_playerStylePath, _username);
                int points = SetMovement(btnLine, _playerHexadecimalColor, stylePlayer, _row, _column, typeLine);

                try
                {
                    InstanceContext context = new InstanceContext(this);
                    Server.MatchManagerClient client = new Server.MatchManagerClient(context);

                    Movement movement = new Movement
                    {
                        TypeLine = typeLine,
                        Row = _row,
                        Column = _column,
                        EarnedPoints = points,
                        HexadecimalColor = _playerHexadecimalColor,
                        StylePath = _playerStylePath,
                        Username = _username
                    };

                    client.EndTurn(_lobbyCode, movement);
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
                catch (FaultException)
                {
                    EmergentWindows.CreateServerErrorMessageWindow();
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
                }
            }
        }

        private string ChooseCorrectStyle(string stylePath, string username)
        {
            string defaultStyle = "";
            int indexFirstLetter = 0;

            string stylePlayer = stylePath;
            if (stylePath == defaultStyle)
            {
                stylePlayer = username[indexFirstLetter].ToString();
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
            string styleButtonClicked = "NoHoverStyle";
            btnLine.Background = Utilities.CreateColorFromHexadecimal(color);
            btnLine.Style = (Style)FindResource(styleButtonClicked);
            btnLine.IsEnabled = false;
        }

        private void MarkAsDrawed(int row, int column, string typeLine)
        {
            bool isHorizontalLine = typeLine.Equals(HORIZONTAL_TYPE_LINE);
            bool isVerticalLine = typeLine.Equals(VERTICAL_TYPE_LINE);
            int markedLine = 1;

            if (isHorizontalLine)
            {
                _horizontalLines[row, column] = markedLine;
            }
            else if (isVerticalLine)
            {
                _verticalLines[row, column] = markedLine;
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

            if (row > GAMEBOARD_BOUNDARY && _horizontalLines[row - 1, column] != GAMEBOARD_BOUNDARY 
                && _verticalLines[row - 1, column] != GAMEBOARD_BOUNDARY && _verticalLines[row - 1, column + 1] != GAMEBOARD_BOUNDARY)
            {
                points++;
                SetScoringPlayerOnBoard(row - 1, column, imageBoardPath, hexadecimalColor);
            }

            return points;
        }

        private int ValidateSquareBelow(int row, int column, string hexadecimalColor, string imageBoardPath)
        {
            int points = 0;

            if (row < BOARD_SIZE - 1 && _horizontalLines[row + 1, column] != GAMEBOARD_BOUNDARY 
                && _verticalLines[row, column] != GAMEBOARD_BOUNDARY && _verticalLines[row, column + 1] != GAMEBOARD_BOUNDARY)
            {
                points++;
                SetScoringPlayerOnBoard(row, column, imageBoardPath, hexadecimalColor);
            }

            return points;
        }

        private int ValidateSquareLeft(int row, int column, string hexadecimalColor, string imageBoardPath)
        {
            int points = 0;

            if (column > GAMEBOARD_BOUNDARY && _verticalLines[row, column - 1] != GAMEBOARD_BOUNDARY 
                && _horizontalLines[row, column - 1] != GAMEBOARD_BOUNDARY && _horizontalLines[row + 1, column - 1] != GAMEBOARD_BOUNDARY)
            {
                points++;
                SetScoringPlayerOnBoard(row, column - 1, imageBoardPath, hexadecimalColor);
            }

            return points;
        }

        private int ValidateSquareRight(int row, int column, string hexadecimalColor, string imageBoardPath)
        {
            int points = 0;

            if (column < BOARD_SIZE - 1 && _verticalLines[row, column + 1] != GAMEBOARD_BOUNDARY 
                && _horizontalLines[row, column] != GAMEBOARD_BOUNDARY && _horizontalLines[row + 1, column] != GAMEBOARD_BOUNDARY)
            {
                points++;
                SetScoringPlayerOnBoard(row, column, imageBoardPath, hexadecimalColor);
            }

            return points;
        }

        private void SetScoringPlayerOnBoard(int row, int column, string imageBoardPath, string hexadecimalColor)
        {
            SolidColorBrush colorBrush = Utilities.CreateColorFromHexadecimal(hexadecimalColor);

            Rectangle scoringPlayerColor = new Rectangle
            {
                Fill = colorBrush
            };

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
            int defaultStyleLength = 1;

            if (stylePath.Length == defaultStyleLength)
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
