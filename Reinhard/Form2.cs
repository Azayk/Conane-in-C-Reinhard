using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace Reinhard
{
    public partial class Form2 : Form
    {
        private bool isWhitePieceRemovedFromCenter = false;
        private bool isFirstPlayerPieceRemovedFromEdge = false;
        private bool isFirstPlayerPieceRemovedFromEdgeUP = false;
        

        private List<Point> capturedPieces = new List<Point>();
        private List<Point> capturedPiecesList = new List<Point>();

        private int deletedRow = -1;
        private int deletedCol = -1;

        int pressedLokation;
        int startdeleted = 0;

        const int mapSize = 6;
        const int cellSize = 50;

        int currentPlayer;

        List<Button> simpleSteps = new List<Button>();

        int countEatSteps = 0;
        Button prevButton;
        Button pressedButton;
        bool isContinue = false;

        bool isMoving;

        int[,] map = new int[mapSize, mapSize];

        Button[,] buttons = new Button[mapSize, mapSize];

        Image whiteFigure;
        Image blackFigure;

        private static Random random = new Random();
        private int aiKill;

        private int playerside = 1;
        private int noPlayerside = 2;
        public Form2(int playerside, int noPlayerside)
        {
            InitializeComponent();
            this.playerside = playerside;
            this.noPlayerside = noPlayerside;


            whiteFigure = new Bitmap(new Bitmap(@"E:\Program Code\CheckersGame-master\CheckersGame\Sprites\w.png"), new Size(cellSize - 10, cellSize - 10));
            blackFigure = new Bitmap(new Bitmap(@"E:\Program Code\CheckersGame-master\CheckersGame\Sprites\b.png"), new Size(cellSize - 10, cellSize - 10));

            this.Text = "Reinhard";
            
            playerside = 1;
            noPlayerside = 2;
            Init();
        }

        public void Init()
        {
            currentPlayer = 1;
            isMoving = false;
            prevButton = null;

            map = new int[mapSize, mapSize]{
            { 2,1,2,1,2,1 },
            { 1,2,1,2,1,2 },
            { 2,1,2,1,2,1 },
            { 1,2,1,2,1,2 },
            { 2,1,2,1,2,1 },
            { 1,2,1,2,1,2 }
        };

            CreateMap();
        }

        public void ResetGame()
        {
            bool player1 = false;
            bool player2 = false;

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == 1)
                        player1 = true;
                    if (map[i, j] == 2)
                        player2 = true;
                }
            }
            if (!player1 || !player2)
            {
                this.Controls.Clear();
                Init();
            }
        }

        public void CreateMap()
        {
            this.Width = (mapSize + 1) * cellSize + 100;
            this.Height = (mapSize + 1) * cellSize;

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(j * cellSize, i * cellSize);
                    button.Size = new Size(cellSize, cellSize);
                    button.Click += new EventHandler(OnFigurePress);
                    if (map[i, j] == 1)
                        button.Image = whiteFigure;
                    else if (map[i, j] == 2) button.Image = blackFigure;

                    button.BackColor = GetPrevButtonColor(button);
                    button.ForeColor = Color.Red;

                    buttons[i, j] = button;

                    this.Controls.Add(button);
                }
            }
        }

        private void UpdateAvailableMovesLabel()
        {
            int availableMoves = CountAvailableMoves(currentPlayer);
            label2.Text = $" {availableMoves}";
        }

        public void SwitchPlayer()
        {
            currentPlayer = currentPlayer == 1 ? 2 : 1;
            ResetGame();
            UpdateAvailableMovesLabel();

            if (currentPlayer == 2)
            {
                pictureBox1.Image = Image.FromFile(@"E:\Program Code\Reinhard\Sprite\b.png");
            }

            if (currentPlayer == 1)
            {
                pictureBox1.Image = Image.FromFile(@"E:\Program Code\Reinhard\Sprite\w.png");
            }

            // Добавляем проверку на победителя после смены игрока
            if (CountAvailableMoves(currentPlayer) == 0 && startdeleted == 2)
            {
                int winner = currentPlayer == 1 ? 2 : 1;
                MessageBox.Show($"Игрок {winner} победил!");
            }
        }

        private int CountAvailableMoves(int player)
        {
            int moveCount = 0;
            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == player)
                    {
                        List<Point> possibleMoves = GetAllPossibleMoves(i, j, player);
                        moveCount += possibleMoves.Count;
                    }
                }
            }
            return moveCount;
        }


        private void AddDirectionalMoves(int i, int j, int di, int dj, List<Point> possibleMoves, int player)
        {
            int nextRow = i + di;
            int nextCol = j + dj;

            if (IsInsideBorders(nextRow, nextCol))
            {
                if (map[nextRow, nextCol] != 0 && map[nextRow, nextCol] != player)
                {
                    int jumpRow = nextRow + di;
                    int jumpCol = nextCol + dj;

                    if (IsInsideBorders(jumpRow, jumpCol) && map[jumpRow, jumpCol] == 0)
                    {
                        possibleMoves.Add(new Point(jumpRow, jumpCol));
                    }
                }
            }
        }

        private bool IsInsideBorders(int i, int j)
        {
            return i >= 0 && i < mapSize && j >= 0 && j < mapSize;
        }



        private Color GetPrevButtonColor(Button button)
        {
            int i = button.Location.Y / cellSize;
            int j = button.Location.X / cellSize;
            return (i + j) % 2 == 0 ? Color.White : Color.Gray;
        }


        public async void OnFigurePress(object sender, EventArgs e)
        {
            if (prevButton != null && prevButton != sender as Button && playerside == currentPlayer)
                prevButton.BackColor = GetPrevButtonColor(prevButton);

            pressedButton = sender as Button;
            int i = pressedButton.Location.Y / cellSize;
            int j = pressedButton.Location.X / cellSize;

            pressedLokation = map[i, j];

            if (startdeleted == 0 && playerside == 1)
            {
                HandleInitialPieceRemoval(i, j);
            }
            else if (startdeleted == 1 && playerside == 2)
            {
                HandleSecondPieceRemoval(i, j);
            }
            else if (startdeleted == 2 && playerside == currentPlayer)
            {
                HandleNormalTurn(i, j);
            }

            prevButton = pressedButton;

            if ((startdeleted > 0 && playerside != currentPlayer) || (startdeleted == 0 && playerside != 1))
            {
                await Task.Delay(1000); // Задержка 1 секунда перед ходом бота
                BotMove(); // Вызываем ход бота после задержки
            }
        }
        private List<Tuple<int, int, int, int>> GetAllPossibleCaptureMoves(int player)
        {
            List<Tuple<int, int, int, int>> possibleMoves = new List<Tuple<int, int, int, int>>();

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == player)
                    {
                        List<Point> captures = GetAllPossibleMoves(i, j, player);
                        foreach (var capture in captures)
                        {
                            possibleMoves.Add(new Tuple<int, int, int, int>(i, j, capture.X, capture.Y));
                        }
                    }
                }
            }

            return possibleMoves;
        }

        private void ExecuteBotMove(int startI, int startJ, int endI, int endJ)
        {
            // Move the piece on the board
            map[endI, endJ] = map[startI, startJ];
            map[startI, startJ] = 0;

            buttons[endI, endJ].Image = buttons[startI, startJ].Image;
            buttons[startI, startJ].Image = null;

            // Remove the captured piece
            int capturedI = (startI + endI) / 2;
            int capturedJ = (startJ + endJ) / 2;
            map[capturedI, capturedJ] = 0;
            buttons[capturedI, capturedJ].Image = null;
        }

        private List<Point> GetAllPossibleMoves(int i, int j, int player)
        {
            List<Point> possibleMoves = new List<Point>();

            AddDirectionalCaptures(i, j, -1, 0, possibleMoves, player); // Up
            AddDirectionalCaptures(i, j, 1, 0, possibleMoves, player);  // Down
            AddDirectionalCaptures(i, j, 0, -1, possibleMoves, player); // Left
            AddDirectionalCaptures(i, j, 0, 1, possibleMoves, player);  // Right

            return possibleMoves;
        }

        private void AddDirectionalCaptures(int startRow, int startCol, int di, int dj, List<Point> possibleCaptures, int player)
        {
            int nextRow = startRow + di;
            int nextCol = startCol + dj;

            if (IsInsideBorders(nextRow, nextCol))
            {
                if (map[nextRow, nextCol] != 0 && map[nextRow, nextCol] != player)
                {
                    int jumpRow = nextRow + di;
                    int jumpCol = nextCol + dj;

                    if (IsInsideBorders(jumpRow, jumpCol) && map[jumpRow, jumpCol] == 0)
                    {
                        possibleCaptures.Add(new Point(jumpRow, jumpCol));
                    }
                }
            }
        }
        private void BotMove()
        {
            if (startdeleted < 2)
            {
                if (startdeleted == 1 && playerside == 1)
                {
                    HandleSecondPieceRemoval2();
                }
                else if (startdeleted == 0 && playerside == 2)
                {
                    HandleInitialPieceRemoval2();
                }
                return;
            }

            List<Tuple<int, int, int, int>> possibleMoves = GetAllPossibleCaptureMoves(noPlayerside);

            if (possibleMoves.Count > 0)
            {
                Random random = new Random();
                int moveIndex = random.Next(0, possibleMoves.Count);
                var move = possibleMoves[moveIndex];
                ExecuteBotMove(move.Item1, move.Item2, move.Item3, move.Item4);

                SwitchPlayer();
                UpdateAvailableMovesLabel();

                if (CountAvailableMoves(currentPlayer) == 0 && startdeleted == 2)
                {
                    int winner = currentPlayer == 1 ? 2 : 1;
                    MessageBox.Show($"Игрок {winner} победил!");
                }
            }
        }


        private void HandleInitialPieceRemoval2()
        {
            aiKill = random.Next(1, 4); // aiKill будет либо 1, либо 2

            if (aiKill == 1)
            {
                isWhitePieceRemovedFromCenter = true;

                aiKill = random.Next(1, 3);

                if (aiKill == 1)
                {
                    buttons[2, 3].Image = null;
                    map[2, 3] = 0;
                    startdeleted++;
                    SwitchPlayer();
                    UpdateAvailableMovesLabel();
                }

                if (aiKill == 2)
                {
                    buttons[3, 2].Image = null;
                    map[3, 2] = 0;
                    startdeleted++;
                    SwitchPlayer();
                    UpdateAvailableMovesLabel();
                }
            }

            else if (aiKill == 2)
            {
                isFirstPlayerPieceRemovedFromEdge = true;

                buttons[5, 0].Image = null;
                map[5, 0] = 0;
                startdeleted++;
                SwitchPlayer();
                UpdateAvailableMovesLabel();
            }

            else if (aiKill == 3)
            {
                isFirstPlayerPieceRemovedFromEdgeUP = true;

                buttons[0, 5].Image = null;
                map[0, 5] = 0;
                startdeleted++;
                SwitchPlayer();
                UpdateAvailableMovesLabel();
            }
        }

        private void HandleSecondPieceRemoval2()
        {
            if (isWhitePieceRemovedFromCenter)
            {
                aiKill = random.Next(1, 3);

                if (aiKill == 1)
                {
                    buttons[2, 2].Image = null;
                    map[2, 2] = 0;
                    startdeleted++;
                    SwitchPlayer();
                    UpdateAvailableMovesLabel();
                }

                if (aiKill == 2)
                {
                    buttons[3, 3].Image = null;
                    map[3, 3] = 0;
                    startdeleted++;
                    SwitchPlayer();
                    UpdateAvailableMovesLabel();
                }
            }

            if (isFirstPlayerPieceRemovedFromEdge)
            {
                aiKill = random.Next(1, 3);

                if (aiKill == 1)
                {
                    buttons[4, 0].Image = null;
                    map[4, 0] = 0;
                    startdeleted++;
                    SwitchPlayer();
                    UpdateAvailableMovesLabel();
                }

                if (aiKill == 2)
                {
                    buttons[5, 1].Image = null;
                    map[5, 1] = 0;
                    startdeleted++;
                    SwitchPlayer();
                    UpdateAvailableMovesLabel();
                }
            }

            if (isFirstPlayerPieceRemovedFromEdgeUP)
            {
                aiKill = random.Next(1, 3);

                if (aiKill == 1)
                {
                    buttons[0, 4].Image = null;
                    map[0, 4] = 0;
                    startdeleted++;
                    SwitchPlayer();
                    UpdateAvailableMovesLabel();
                }

                if (aiKill == 2)
                {
                    buttons[1, 5].Image = null;
                    map[1, 5] = 0;
                    startdeleted++;
                    SwitchPlayer();
                    UpdateAvailableMovesLabel();
                }

            }
        }

        private void HandleInitialPieceRemoval(int i, int j)
        {
            if (currentPlayer == 1)
            {
                if ((i == 5 && j == 0) || (i == 2 && j == 3) || (i == 3 && j == 2) || (i == 0 && j == 5))
                {
                    pressedButton.Image = null;
                    map[i, j] = 0;
                    deletedRow = i;
                    deletedCol = j;
                    startdeleted++;
                    SwitchPlayer();
                    UpdateAvailableMovesLabel();

                    if ((i == 2 && j == 3) || (i == 3 && j == 2))
                    {
                        isWhitePieceRemovedFromCenter = true;
                    }
                    if (i == 5 && j == 0)
                    {
                        isFirstPlayerPieceRemovedFromEdge = true;
                    }
                    if (i == 0 && j == 5)
                    {
                        isFirstPlayerPieceRemovedFromEdgeUP = true;
                    }
                }
                else
                {
                    MessageBox.Show("Выберите корректную фигуру для удаления!");
                }
            }
        }

        private void HandleSecondPieceRemoval(int i, int j)
        {
            if (currentPlayer == 2)
            {
                bool pieceRemoved = false;
                if (CanRemovePieceFromCenter(i, j))
                {
                    RemovePiece(i, j);
                    pieceRemoved = true;
                }
                else if (CanRemovePieceFromEdge(i, j))
                {
                    RemovePiece(i, j);
                    pieceRemoved = true;
                }
                else if (CanRemovePieceFromEdgeUP(i, j))
                {
                    RemovePiece(i, j);
                    pieceRemoved = true;
                }

                if (!pieceRemoved)
                {
                    MessageBox.Show("Выберите корректную фигуру для удаления!");
                }
                else
                {
                    startdeleted++;
                    SwitchPlayer();
                    UpdateAvailableMovesLabel();
                }
            }
        }

        private bool CanRemovePieceFromCenter(int i, int j)
        {
            return isWhitePieceRemovedFromCenter && ((i == 2 && j == 2) || (i == 3 && j == 3));
        }

        private bool CanRemovePieceFromEdge(int i, int j)
        {
            return isFirstPlayerPieceRemovedFromEdge && ((i == 4 && j == 0) || (i == 5 && j == 1));
        }

        private bool CanRemovePieceFromEdgeUP(int i, int j)
        {
            return isFirstPlayerPieceRemovedFromEdgeUP && ((i == 0 && j == 4) || (i == 1 && j == 5));
        }

        private void RemovePiece(int i, int j)
        {
            pressedButton.Image = null;
            map[i, j] = 0;
        }

        private void HandleNormalTurn(int i, int j)
        {
            if (currentPlayer == 2 && pressedLokation == 1 || (playerside == 1 && currentPlayer == 2))
                return;

            if (currentPlayer == 1 && pressedLokation == 2 || ( playerside == 2 && currentPlayer == 1))
                return;


            if (pressedButton == prevButton && isMoving)
            {
                CloseSteps();
                ActivateAllButtons();
                isMoving = false;
                prevButton = null;
                capturedPiecesList.Clear();
                return;
            }

            if (pressedLokation == currentPlayer)
            {
                CloseSteps();
                pressedButton.BackColor = Color.Red;
                DeactivateAllButtons();
                pressedButton.Enabled = true;
                capturedPieces.Clear();
                ShowPossibleSteps(i, j);
                isMoving = true;
            }
            else if (isMoving)
            {
                int prevI = prevButton.Location.Y / cellSize;
                int prevJ = prevButton.Location.X / cellSize;

                if (buttons[i, j].BackColor == Color.Yellow)
                {
                    map[i, j] = map[prevI, prevJ];
                    map[prevI, prevJ] = 0;
                    pressedButton.Image = prevButton.Image;
                    prevButton.Image = null;
                    pressedButton.Text = prevButton.Text;
                    prevButton.Text = "";

                    if (possibleCaptures.ContainsKey(pressedButton))
                    {
                        foreach (var point in possibleCaptures[pressedButton])
                        {
                            map[point.X, point.Y] = 0;
                            buttons[point.X, point.Y].Image = null;
                        }
                        possibleCaptures.Remove(pressedButton);
                    }

                    CaptureAdjacentEnemy(prevI, prevJ, i, j);

                    SwitchPlayer();
                    UpdateAvailableMovesLabel();
                    isMoving = false;
                    CloseSteps();
                    ActivateAllButtons();
                }
            }
        }

        private void CaptureAdjacentEnemy(int prevI, int prevJ, int newI, int newJ)
        {
            int di = newI - prevI;
            int dj = newJ - prevJ;

            int directionI = di == 0 ? 0 : di / Math.Abs(di);
            int directionJ = dj == 0 ? 0 : dj / Math.Abs(dj);

            int enemyI = prevI + directionI;
            int enemyJ = prevJ + directionJ;

            if (IsInsideBorders(enemyI, enemyJ) && map[enemyI, enemyJ] != 0 && map[enemyI, enemyJ] != currentPlayer)
            {
                map[enemyI, enemyJ] = 0;
                buttons[enemyI, enemyJ].Image = null;
            }
        }


        private Dictionary<Button, List<Point>> possibleCaptures = new Dictionary<Button, List<Point>>();

        private void ShowPossibleSteps(int i, int j)
        {
            capturedPieces.Clear();
            possibleCaptures.Clear();
            capturedPiecesList.Clear();

            ShowDirectionalSteps(i, j, -1, 0); // Up
            ShowDirectionalSteps(i, j, 1, 0);  // Down
            ShowDirectionalSteps(i, j, 0, -1); // Left
            ShowDirectionalSteps(i, j, 0, 1);  // Right
        }


        private void ShowDirectionalSteps(int i, int j, int di, int dj)
        {
            int currentPlayer = map[i, j];
            int nextRow = i + di;
            int nextCol = j + dj;

            if (IsInsideBorders(nextRow, nextCol))
            {
                if (map[nextRow, nextCol] != 0 && map[nextRow, nextCol] != currentPlayer)
                {
                    int jumpRow = nextRow + di;
                    int jumpCol = nextCol + dj;

                    if (IsInsideBorders(jumpRow, jumpCol) && map[jumpRow, jumpCol] == 0)
                    {
                        buttons[jumpRow, jumpCol].BackColor = Color.Yellow;
                        buttons[jumpRow, jumpCol].Enabled = true;

                        List<Point> capturedDirection = new List<Point>(capturedPieces);
                        capturedDirection.Add(new Point(nextRow, nextCol));

                        if (!possibleCaptures.ContainsKey(buttons[jumpRow, jumpCol]))
                        {
                            possibleCaptures[buttons[jumpRow, jumpCol]] = new List<Point>(capturedDirection);
                        }
                        else
                        {
                            foreach (var point in capturedDirection)
                            {
                                if (!possibleCaptures[buttons[jumpRow, jumpCol]].Contains(point))
                                {
                                    possibleCaptures[buttons[jumpRow, jumpCol]].Add(point);
                                }
                            }
                        }

                        ShowDirectionalSteps(jumpRow, jumpCol, di, dj);
                    }
                }
            }
        }

        public void CheckDirectionMove(int i, int j, int currentPlayer, int di, int dj)
        {
            int nextRow = i + di;
            int nextCol = j + dj;

            // Check if the next cell is within the borders
            if (!IsInsideBorders(nextRow, nextCol))
                return;

            // Check if the next cell is occupied by an opponent's piece
            if (map[nextRow, nextCol] != currentPlayer && map[nextRow, nextCol] != 0)
            {
                // Check if the cell after the opponent's piece is empty
                int afterEnemyRow = nextRow + di;
                int afterEnemyCol = nextCol + dj;
                if (IsInsideBorders(afterEnemyRow, afterEnemyCol) && map[afterEnemyRow, afterEnemyCol] == 0)
                {
                    // Highlight the cell where the player can move
                    buttons[afterEnemyRow, afterEnemyCol].BackColor = Color.Yellow;
                    buttons[afterEnemyRow, afterEnemyCol].Enabled = true;

                    // Add the captured piece's coordinates to the list
                    capturedPieces.Add(new Point(nextRow, nextCol));

                }
            }
        }
        public bool CanCapture(int i, int j, int nextRow, int nextCol, int currentPlayer)
        {
            int enemyPlayer = currentPlayer == 1 ? 2 : 1;

            // Check if the next cell contains an opponent's piece
            if (map[nextRow, nextCol] == enemyPlayer)
            {
                // Check if it's possible to capture the opponent's piece
                if (Math.Abs(nextRow - i) == 1 || Math.Abs(nextCol - j) == 1)
                {
                    return true;
                }
            }

            return false;
        }


        public void CloseSimpleSteps(List<Button> simpleSteps)
        {
            if (simpleSteps.Count > 0)
            {
                for (int i = 0; i < simpleSteps.Count; i++)
                {
                    simpleSteps[i].BackColor = GetPrevButtonColor(simpleSteps[i]);
                    simpleSteps[i].Enabled = false;
                }
            }
        }




        public bool IsButtonHasEatStep(int IcurrFigure, int JcurrFigure, bool isOneStep, int[] dir)
        {
            bool eatStep = false;
            int i = IcurrFigure + dir[0];
            int j = JcurrFigure + dir[1];

            // Check if the next cell is adjacent and within the board borders
            if (IsInsideBorders(i, j))
            {
                // If the next cell contains an opponent's piece
                if (map[i, j] != 0 && map[i, j] != currentPlayer)
                {
                    // Check if the cell after the opponent's piece is available for the move
                    if (IsInsideBorders(i + dir[0], j + dir[1]) && map[i + dir[0], j + dir[1]] == 0)
                        eatStep = true;
                }
            }

            return eatStep;
        }

        private void CloseSteps()
        {
            for (int i = 0; i < buttons.GetLength(0); i++)
            {
                for (int j = 0; j < buttons.GetLength(1); j++)
                {
                    if (buttons[i, j].BackColor == Color.Yellow)
                    {
                        buttons[i, j].BackColor = GetPrevButtonColor(buttons[i, j]);
                        buttons[i, j].Enabled = false;
                    }
                }
            }
        }


        private void ActivateAllButtons()
        {
            foreach (var button in buttons)
            {
                button.Enabled = true;
            }
        }

        private void DeactivateAllButtons()
        {
            foreach (var button in buttons)
            {
                button.Enabled = false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }


        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Создаем новый экземпляр формы Form2
            Form2 newForm = new Form2(playerside, noPlayerside);

            // Показываем новую форму
            newForm.Show();

            // Закрываем текущую форму
            this.Hide();

            // Устанавливаем событие закрытия новой формы, чтобы закрыть приложение при закрытии новой формы
            newForm.FormClosed += (s, args) => this.Close();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            // Скрываем текущую форму
            this.Hide();

            // Создаем новую форму Form1
            Menu form1 = new Menu();

            // Показываем новую форму
            form1.Show();
        }
    }
}
