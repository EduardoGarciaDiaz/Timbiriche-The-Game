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
        private int[,] _horizontalLines;
        private int[,] _verticalLines;
        private int _row;
        private int _column;
        private const int BOARD_SIZE = 10;
        const string HORIZONTAL_TYPE_LINE = "Horizontal";
        const string VERTICAL_TYPE_LINE = "Vertical";

        private Server.Player _playerLoggedIn = PlayerSingleton.Player;
        private string _username;
        private bool _itsMyTurn;
        private string _lobbyCode;
        private string _playerHexadecimalColor;
        private string _playerStylePath;



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

    public partial class XAMLGameBoard : Page, IMatchManagerCallback
    {
        private Match.Timer _matchTimer;
        private Match.Timer _turnTimer;
        private DispatcherTimer _dispatchTimer;

        public void NotifyMovement(Movement movement)
        {
            Button btnLine = FindButtonByName(movement.TypeLine + "_" + movement.Row + "_" + movement.Column);
            string stylePlayer = ChooseCorrectStyle(movement.StylePath, movement.Username);
            SetMovement(btnLine, movement.HexadecimalColor, stylePlayer, movement.Row, movement.Column, movement.TypeLine);
        }

        public void NotifyFirstTurn(float matchDurationInMinutes, float turnDurationInMinutes, string username)
        {
            _matchTimer = new Match.Timer((int)(matchDurationInMinutes * 60));
            _turnTimer = new Match.Timer((int)(turnDurationInMinutes * 60));

            _dispatchTimer = new DispatcherTimer();
            TimeSpan.FromSeconds(1);

            _dispatchTimer.Tick += UpdateTimeLabels;
            _dispatchTimer.Start();

            _matchTimer.CountDownFinished += OnCountDownMatchFinished;
            _turnTimer.CountDownFinished += OnCountDownTurnFinished;

            _matchTimer.Start();
            _turnTimer.Start();

            UpdateTurn(username);
        }

        public void NotifyNewTurn(string username)
        {
            UpdateTurn(username);
        }

        public void NotifyNewScoreboard(KeyValuePair<Server.LobbyPlayer, int>[] scoreboard)
        {
            if (stackPanelScoreboard.Visibility == Visibility.Collapsed)
            {
                stackPanelScoreboard.Visibility = Visibility.Visible;
            }

            Storyboard animationFadeIn = (Storyboard)FindResource("fadeAnimation");
            animationFadeIn.Begin();

            int numPlayers = scoreboard.Count();

            SolidColorBrush brushFirstPlace = Utilities.CreateColorFromHexadecimal(scoreboard[0].Key.HexadecimalColor);
            tbxFirstPlaceUsername.Text = scoreboard[0].Key.Username;
            tbxFirstPlacePoints.Text = scoreboard[0].Value.ToString();
            borderFirstPlace.Background = brushFirstPlace;

            SolidColorBrush brushSecondPlace = Utilities.CreateColorFromHexadecimal(scoreboard[1].Key.HexadecimalColor);
            tbxSecondPlaceUsername.Text = scoreboard[1].Key.Username;
            tbxSecondPlacePoints.Text = scoreboard[1].Value.ToString();
            borderSecondPlace.Background = brushSecondPlace;

            if (numPlayers  > 2)
            {
                SolidColorBrush brushThirdPlace = Utilities.CreateColorFromHexadecimal(scoreboard[2].Key.HexadecimalColor);
                gridThirdPlace.Visibility = Visibility.Visible;
                tbxThirdPlaceUsername.Text = scoreboard[2].Key.Username;
                tbxThirdPlacePoints.Text = scoreboard[2].Value.ToString();
                borderThirdPlace.Background = brushThirdPlace;
            }

            if (numPlayers > 3)
            {
                SolidColorBrush brushFourthPlace = Utilities.CreateColorFromHexadecimal(scoreboard[3].Key.HexadecimalColor);
                gridFourthPlace.Visibility = Visibility.Visible;
                tbxFourthPlaceUsername.Text = scoreboard[3].Key.Username;
                tbxFourthPlacePoints.Text = scoreboard[3].Value.ToString();
                borderFourthPlace.Background = brushFourthPlace;
            }
        }

        public void NotifyEndOfTheMatch(KeyValuePair<Server.LobbyPlayer, int>[] scoreboard, int coinsEarned)
        {
            _turnTimer.Stop();
            _matchTimer.Stop();
            _dispatchTimer.Stop();

            XAMLMainWindow parentWindow = Window.GetWindow(this) as XAMLMainWindow;

            if (parentWindow != null)
            {
                parentWindow.frameNavigation.NavigationService.Navigate(new XAMLVictory(_lobbyCode, scoreboard, coinsEarned));
            }
        }

        public void NotifyNewMessage(string senderUsername, string message, int idSenderPlayer)
        {
            bool isMessageReceived = true;

            XAMLMessageItemComponent messageComponent = new XAMLMessageItemComponent(senderUsername, message, isMessageReceived, idSenderPlayer);
            messageComponent.HorizontalAlignment = HorizontalAlignment.Left;

            stackPanelMessages.Children.Add(messageComponent);
        }

        public void NotifyPlayerLeftMatch()
        {
            gridThirdPlace.Visibility = Visibility.Collapsed;
            gridFourthPlace.Visibility = Visibility.Collapsed;
        }

        public void NotifyOnlyPlayerInMatch()
        {
            Utils.EmergentWindows.CreateEmergentWindow("Unico Jugador", "Eres el unico jugador en la partida. Regresaras al lobby.");
            LeftMatch();
        }

        private Button FindButtonByName(string name)
        {
            return (Button)LogicalTreeHelper.FindLogicalNode(this, name);
        }

        private void UpdateTimeLabels(object sender, EventArgs e)
        {
            lbMatchTime.Content = _matchTimer.GetTime();
            lbTurnTime.Content = _turnTimer.GetTime();
        }

        private void UpdateTurn(string username)
        {
            lbTurnOfUsername.Content = username;
            _itsMyTurn = (_username == username) ? true : false;

            _turnTimer.Reset();
        }

        private void OnCountDownTurnFinished(object sender, EventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            Server.MatchManagerClient client = new Server.MatchManagerClient(context);

            try
            {
                client.EndTurnWithoutMovement(_lobbyCode);
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
            }
        }

        private void OnCountDownMatchFinished(object sender, EventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            MatchManagerClient client = new MatchManagerClient(context);

            _itsMyTurn = false;

            try
            {
                client.EndMatch(_lobbyCode);
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

        private void LeftMatch()
        {
            _turnTimer.Stop();
            _matchTimer.Stop();
            _dispatchTimer.Stop();

            XAMLMainWindow parentWindow = Window.GetWindow(this) as XAMLMainWindow;

            if (parentWindow != null)
            {
                parentWindow.frameNavigation.NavigationService.Navigate(new XAMLLobby());
            }
        }

        private void SendMessageToLobby(string senderUsername, string message, int idSenderPlayer)
        {
            InstanceContext context = new InstanceContext(this);
            Server.MatchManagerClient client = new Server.MatchManagerClient(context);

            try
            {
                client.SendMessageToLobby(_lobbyCode, senderUsername, message, idSenderPlayer);
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
            }
        }

        private void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if(tbxMessage.Text != null && tbxMessage.Text.Length != 0)
            {
                string senderUsername = _username;
                string message = tbxMessage.Text;
                bool isMessageReceived = false;

                int idSenderPlayer = PlayerSingleton.Player.IdPlayer;

                XAMLMessageItemComponent messageComponent = new XAMLMessageItemComponent(senderUsername, message, isMessageReceived, idSenderPlayer);
                messageComponent.HorizontalAlignment = HorizontalAlignment.Right;

                stackPanelMessages.Children.Add(messageComponent);
                tbxMessage.Text = "";

                SendMessageToLobby(senderUsername, message, idSenderPlayer);
            }
        }

        private void BtnExitMatch_Click(object sender, RoutedEventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            Server.MatchManagerClient client = new Server.MatchManagerClient(context);

            try
            {
                client.LeftMatch(_lobbyCode, PlayerSingleton.Player.Username);
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
            }
            finally
            {
                _turnTimer.Stop();
                _matchTimer.Stop();
                _dispatchTimer.Stop();
            }

            LeftMatch();
        }

        private void ImageExitMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            gridExitMenu.Visibility = Visibility.Visible;
        }

        private void GridExitMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            gridExitMenu.Visibility = Visibility.Collapsed;
        }
    }
}
