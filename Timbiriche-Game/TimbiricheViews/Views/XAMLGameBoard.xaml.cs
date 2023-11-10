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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TimbiricheViews.Components.Match;
using TimbiricheViews.Player;
using TimbiricheViews.Server;

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
            InitializeComponent();
            InitializeGameBoard();
            _lobbyCode = lobbyCode;

            InstanceContext context = new InstanceContext(this);
            MatchManagerClient client = new MatchManagerClient(context);
            client.RegisterToTheMatch(_lobbyCode, PlayerSingleton.Player.Username);
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
            string colorPlayer = "#FFAC4C4C";

            Button btnLine = (Button)sender;
            string[] tagParts = btnLine.Tag.ToString().Split(SPLIT_SYMBOL);
            string typeLine = tagParts[INDEX_TYPE_LINE];
            _row = int.Parse(tagParts[INDEX_ROW]);
            _column = int.Parse(tagParts[INDEX_COLUMN]);

            if (_itsMyTurn)
            {
                int points = SetMovement(btnLine, colorPlayer, _row, _column, typeLine);

                InstanceContext context = new InstanceContext(this);
                Server.MatchManagerClient client = new Server.MatchManagerClient(context);
                client.EndTurn(_lobbyCode, typeLine, _row, _column, points);
            }
        }

        private int SetMovement(Button btnLine, String colorPlayer, int row, int column, string typeLine)
        {
            UpdateButtonAppearance(btnLine, colorPlayer);
            MarkAsDrawed(row, column, typeLine);
            int points = ValidateSquares(row, column, typeLine);

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
        
        private int ValidateSquares(int row, int column, string typeLine)
        {
            bool isHorizontalLine = typeLine.Equals(HORIZONTAL_TYPE_LINE);
            bool isVerticalLine = typeLine.Equals(VERTICAL_TYPE_LINE);
            int points = 0;

            if (isHorizontalLine)
            {
                points += ValidateSquareAbove(row, column);
                points += ValidateSquareBelow(row, column);
            }
            else if(isVerticalLine)
            {
                points += ValidateSquareLeft(row, column);
                points += ValidateSquareRight(row, column);
            }

            return points;
        }

        private int ValidateSquareAbove(int row, int column)
        {
            int points = 0;
            if (row > 0 && _horizontalLines[row - 1, column] != 0 && _verticalLines[row - 1, column] != 0 && _verticalLines[row - 1, column + 1] != 0)
            {
                points++;
                SetImageBoard(row - 1, column);
            }

            return points;
        }

        private int ValidateSquareBelow(int row, int column)
        {
            int points = 0;
            if (row < BOARD_SIZE - 1 && _horizontalLines[row + 1, column] != 0 && _verticalLines[row, column] != 0 && _verticalLines[row, column + 1] != 0)
            {
                points++;
                SetImageBoard(row, column);
            }

            return points;
        }

        private int ValidateSquareLeft(int row, int column)
        {
            int points = 0;
            if (column > 0 && _verticalLines[row, column - 1] != 0 && _horizontalLines[row, column - 1] != 0 && _horizontalLines[row + 1, column - 1] != 0)
            {
                points++;
                SetImageBoard(row, column - 1);
            }

            return points;
        }

        private int ValidateSquareRight(int row, int column)
        {
            int points = 0;
            if (column < BOARD_SIZE - 1 && _verticalLines[row, column + 1] != 0 && _horizontalLines[row, column] != 0 && _horizontalLines[row + 1, column] != 0)
            {
                points++;
                SetImageBoard(row, column);
            }
            return points;
        }

        private void SetImageBoard(int row, int column)
        {
            Image imageWhoScore = new Image();
            // TODO: Utilities for transform Images
            BitmapImage bitmapImage = new BitmapImage(new Uri("../Resources/Skins/basicBox.png", UriKind.RelativeOrAbsolute));
            imageWhoScore.Source = bitmapImage;
            Grid.SetRow(imageWhoScore, row);
            Grid.SetColumn(imageWhoScore, column);
            gridGameBoard.Children.Add(imageWhoScore);
        }
    }

    public partial class XAMLGameBoard : IMatchManagerCallback
    {
        private Match.Timer _matchTimer;
        private Match.Timer _turnTimer;

        public void NotifyMovement(string typeLine, int row, int column)
        {
            Button btnLine = FindButtonByName(typeLine + "Q" + row + "Q" + column);
            SetMovement(btnLine, "#FFAC4C4C", row, column, typeLine);
        }

        public void NotifyFirstTurn(int matchDurationInMinutes, int turnDurationInMinutes, string username)
        {
            _matchTimer = new Match.Timer(matchDurationInMinutes * 60);
            _turnTimer = new Match.Timer(turnDurationInMinutes * 60);

            DispatcherTimer dispatchTimer = new DispatcherTimer();
            TimeSpan.FromSeconds(1);

            dispatchTimer.Tick += UpdateTimeLabels;
            dispatchTimer.Start();

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

        public void NotifyNewMessage(string senderUsername, string message)
        {
            XAMLMessageItemComponent messageComponent = new XAMLMessageItemComponent(senderUsername, message);
            messageComponent.HorizontalAlignment = HorizontalAlignment.Left;

            stackPanelMessages.Children.Add(messageComponent);
        }

        public void NotifyNewScoreboard(KeyValuePair<string, int>[] scoreboard)
        {
            Storyboard animationFadeIn = (Storyboard)FindResource("fadeAnimation");
            animationFadeIn.Begin();

            int numPlayers = scoreboard.Count();

            tbxFirstPlaceUsername.Text = scoreboard[0].Key;
            tbxFirstPlacePoints.Text = scoreboard[0].Value.ToString();

            tbxSecondPlaceUsername.Text = scoreboard[1].Key;
            tbxSecondPlacePoints.Text = scoreboard[1].Value.ToString();


            if (numPlayers  > 2)
            {
                gridThirdPlace.Visibility = Visibility.Visible;
                tbxThirdPlaceUsername.Text = scoreboard[2].Key;
                tbxThirdPlacePoints.Text = scoreboard[2].Value.ToString();
            }

            if (numPlayers > 3)
            {
                gridFourthPlace.Visibility = Visibility.Visible;
                tbxFourthPlaceUsername.Text = scoreboard[3].Key;
                tbxFourthPlacePoints.Text = scoreboard[3].Value.ToString();
            }
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
            _itsMyTurn = (PlayerSingleton.Player.Username == username) ? true : false;

            _turnTimer.Reset();
        }

        private void OnCountDownTurnFinished(object sender, EventArgs e)
        {
            InstanceContext context = new InstanceContext(this);
            Server.MatchManagerClient client = new Server.MatchManagerClient(context);
            client.EndTurnWithoutMovement(_lobbyCode);
        }

        private void OnCountDownMatchFinished(object sender, EventArgs e)
        {

        }

        private void BtnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if(tbxMessage.Text != null && tbxMessage.Text.Length != 0)
            {
                string senderUsername = PlayerSingleton.Player.Username;
                string message = tbxMessage.Text;

                XAMLMessageItemComponent messageComponent = new XAMLMessageItemComponent(senderUsername, message);
                messageComponent.HorizontalAlignment = HorizontalAlignment.Right;

                stackPanelMessages.Children.Add(messageComponent);
                tbxMessage.Text = "";

                InstanceContext context = new InstanceContext(this);
                Server.MatchManagerClient client = new Server.MatchManagerClient(context);
                client.SendMessageToLobby(_lobbyCode, senderUsername, message);
            }
        }
    }
}
