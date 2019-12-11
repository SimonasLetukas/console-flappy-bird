using console_flappy_bird.Interfaces;
using console_flappy_bird.Models;
using Pastel;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Timers;
using Kažkas.gero;

namespace console_flappy_bird.Logic
{
    class GameEngineConsole : IGameEngine
    {
        // Config
        const int maxUsernameChars = 20;
        const int screenWidth = 80;
        const int screenHeight = 15;
        const int refreshInterval = 100;

        const int pipeGapSize = 7;
        const int pipePeriodLength = 15;
        const int pipeThickness = 6;
        const char pipeLeftTopSymbol = '└';
        const char pipeRightTopSymbol = '┘';
        const char pipeLeftBottomSymbol = '┌';
        const char pipeRightBottomSymbol = '┐';
        const char pipeHoleSymbol = '=';
        const char pipeBodySymbol = '│';

        const int cloudSparsingRate = 20;
        const int cloudMaxDensity = 25;
        const char cloudSymbol = '~';

        const int gravityConstant = 8;
        const int jumpAmount = 4;
        const int birdHorizontalPosition = 5;
        const char birdUpSymbol = '^';
        const char birdSideSymbol = '>';
        const char birdDownSymbol = 'v';
        

        // Variables
        int oldHighscore;
        string oldHighscoreUser;
        DateTime oldHighscoreDate;
        string username;
        bool flyUpFlag;
        bool renderFlag;
        bool retryGame;
        Random random;
        Timer fixedUpdate;
        Timer acceleratedUpdate;
        object fixedUpdateLock;
        object acceleratedUpdateLock;
        Stopwatch gameTime;
        BirdController bird;
        EnvironmentController environment;
        GameEngineService service;

        public void StartGame ()
        {
            SetupStart();
            DisplayStartMenu();

            retry:
            while (UpdateGame())
            {
            }

            DisplayEndMenu();
            ExportResults();
            if (retryGame)
            {
                retryGame = false;
                InitiateGame();
                goto retry;
            }
        }

        private void SetupStart ()
        {
            Console.SetWindowSize(screenWidth, screenHeight);
            Console.CursorVisible = false;
            Console.Clear();
            flyUpFlag = false;
            renderFlag = true;
            random = new Random();
            gameTime = new Stopwatch();
            fixedUpdateLock = new object();
            acceleratedUpdateLock = new object();

            var serviceModel = PopulateServiceModel();
            service = new GameEngineService(serviceModel);

            // TODO import best score from file
            oldHighscore = 100;
            oldHighscoreUser = "Simonas";
            oldHighscoreDate = DateTime.Now;
        }

        private void DisplayStartMenu ()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.Line); 
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.MainMenuWelcome.Length / 2) + Texts.MainMenuWelcome);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.Line);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            if (oldHighscore != 0)
            {
                Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.CurrentHighscore + oldHighscore + ", " + oldHighscoreUser);
                Console.WriteLine(new string(' ', (screenWidth / 2) - (Texts.Line.Length / 2) + Texts.CurrentHighscore.Length) + oldHighscoreDate.ToString("yyyy/MM/dd"));
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.WriteLine();
            var cursorPositionForUsername = Console.CursorTop;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.InputUsername + username);
            Console.WriteLine();
            Console.WriteLine();
            Console.Write(new string(' ', screenWidth / 2 - (Texts.Instructions.Length + Texts.Spacebar.Length) / 2) + Texts.Instructions);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(Texts.Spacebar);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.StartGame.Length / 2) + Texts.StartGame);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.Line);

            while(true)
            {
                var keypress = Console.ReadKey(true);
                if (keypress.Key == ConsoleKey.Spacebar)
                {
                    InitiateGame();
                    break;
                }
                else if (keypress.Key == ConsoleKey.Backspace)
                {
                    if (string.IsNullOrEmpty(username) && username.Length == 0)
                    {
                        continue;
                    }
                    username = username.Remove(username.Length - 1);
                    Console.SetCursorPosition(0, cursorPositionForUsername);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.InputUsername + username + "  ");
                }
                else if (keypress.Key == ConsoleKey.Delete)
                {
                    if (string.IsNullOrEmpty(username) && username.Length == 0)
                    {
                        continue;
                    }
                    username = string.Empty;
                    Console.SetCursorPosition(0, cursorPositionForUsername);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.InputUsername + new string(' ', maxUsernameChars));
                }
                else if (string.IsNullOrEmpty(username) || username.Length <= maxUsernameChars)
                {
                    username += keypress.KeyChar;
                    Console.SetCursorPosition(0, cursorPositionForUsername);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.InputUsername + username);
                }
            }
        }

        private void InitiateGame ()
        {
            if (string.IsNullOrEmpty(username))
            {
                username = "User" + random.Next(10000, 99999).ToString() + "_" + DateTime.Now.ToString("yy/MM/dd");
            }

            bird = new BirdController(screenHeight, refreshInterval, gravityConstant, jumpAmount);
            environment = new EnvironmentController(screenWidth, screenHeight, pipeGapSize, pipePeriodLength, pipeThickness);

            gameTime.Start();

            fixedUpdate = new Timer(refreshInterval);
            acceleratedUpdate = new Timer(GetFrameLength());

            fixedUpdate.Elapsed += OnFixedUpdate;
            acceleratedUpdate.Elapsed += OnUpdate;

            fixedUpdate.AutoReset = true;
            acceleratedUpdate.AutoReset = true;

            fixedUpdate.Enabled = true;
            acceleratedUpdate.Enabled = true;
        }

        private bool UpdateGame ()
        {
            if (renderFlag)
            {
                // Try without copying and see if errors appear
                //var environmentInstance = (EnvironmentController)environment.Clone();
                //var birdInstance = (BirdController)bird.Clone();

                Render(environment, bird);

                if (HasCollided(environment, bird))
                {
                    return false;
                }
                renderFlag = false;
            }

            if (Console.KeyAvailable)
            {
                HandleInput();
            }
            return true;
        }

        private void Render (EnvironmentController environmentInstance, BirdController birdInstance)
        {
            var render = service.GetRender(environmentInstance, birdInstance);
            var line = new (Color color, string value)[screenWidth];

            for (var y = 0; y < screenHeight; y++)
            {
                Console.SetCursorPosition(0, y);
                for (var x = 0; x < screenWidth; x++)
                {
                    line[x] = (GetColorForSymbol(render[x, y]), render[x, y].ToString());
                }
                Console.Write(string.Join(string.Empty, line.Select(s => s.value.Pastel(s.color))));
            }
        }

        private Color GetColorForSymbol (char value)
        {
            switch (value)
            {
                case pipeBodySymbol:
                case pipeHoleSymbol:
                case pipeLeftBottomSymbol:
                case pipeLeftTopSymbol:
                case pipeRightBottomSymbol:
                case pipeRightTopSymbol:
                    return Color.Green;

                case birdUpSymbol:
                case birdDownSymbol:
                case birdSideSymbol:
                    return Color.Red;

                default:
                    return Color.Teal;
            }
        }

        private bool HasCollided (EnvironmentController environmentInstance, BirdController birdInstance)
        {
            var hasCollided = service.HasCollided(environmentInstance, birdInstance);
            return hasCollided;
        }

        private void HandleInput ()
        {
            var keypress = Console.ReadKey(true);
            if (keypress.Key != ConsoleKey.Spacebar)
            {
                return;
            }

            flyUpFlag = true;
        }

        private void OnFixedUpdate (object sender, EventArgs args)
        {
            if (System.Threading.Monitor.TryEnter(fixedUpdateLock))
            {
                try
                {
                    bird.Update(flyUpFlag);
                    renderFlag = true;
                    flyUpFlag = false;
                }
                finally
                {
                    System.Threading.Monitor.Exit(fixedUpdateLock);
                }
            }
        }

        private void OnUpdate (object sender, EventArgs args)
        {
              if (System.Threading.Monitor.TryEnter(acceleratedUpdateLock))
            {
                try
                {
                    acceleratedUpdate.Interval = GetFrameLength();
                    environment.Update();
                }
                finally
                {
                    System.Threading.Monitor.Exit(acceleratedUpdateLock);
                }
            }
        }

        private void DisplayEndMenu ()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.Line);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.GameOver.Length / 2) + Texts.GameOver);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.Line);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.YourHighscore + bird.GetScore().ToString());
            if (bird.GetScore() > oldHighscore)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.NewHighscore + username + "!");
            }
            else
            {
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.IsRetryNeeded);
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.Line);
            while (true)
            {
                var keypress = Console.ReadKey(true);
                if (keypress.Key == ConsoleKey.Y)
                {
                    retryGame = true;
                    break;
                }
                if (keypress.Key == ConsoleKey.N)
                {
                    break;
                }
            }
        }

        private void ExportResults ()
        {
            // TODO: Export name and score and date into file IF it's better than before or before doesn't exist
        }

        private int GetFrameLength ()
        {
            var timeSpan = gameTime.ElapsedMilliseconds / 1000f;
            var newFrameLength = (int)(600f / (timeSpan + 1)) + refreshInterval;
            return newFrameLength;
        }

        private GameEngineServiceModel PopulateServiceModel ()
        {
            return new GameEngineServiceModel
            {
                ScreenWidth = screenWidth,
                ScreenHeight = screenHeight,
                CloudSymbol = cloudSymbol,
                CloudMaxDensity = cloudMaxDensity,
                CloudSparsingRate = cloudSparsingRate,
                BirdUpSymbol = birdUpSymbol,
                BirdSideSymbol = birdSideSymbol,
                BirdDownSymbol = birdDownSymbol,
                BirdHorizontalPosition = birdHorizontalPosition,
                PipeLeftTopSymbol = pipeLeftTopSymbol,
                PipeRightTopSymbol = pipeRightTopSymbol,
                PipeLeftBottomSymbol = pipeLeftBottomSymbol,
                PipeRightBottomSymbol = pipeRightBottomSymbol,
                PipeHoleSymbol = pipeHoleSymbol,
                PipeBodySymbol = pipeBodySymbol,
                PipeThickness = pipeThickness,
                PipeGapSize = pipeGapSize
            };
        }
    }
}
