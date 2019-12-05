using console_flappy_bird.Extensions;
using console_flappy_bird.Interfaces;
using console_flappy_bird.Models;
using System;
using System.Diagnostics;
using System.Timers;

namespace console_flappy_bird.Logic
{
    class GameEngineConsole : IGameEngine
    {
        // Config
        const int maxUsernameChars = 20;
        const int screenWidth = 80;
        const int screenHeight = 15;
        const int refreshInterval = 100;
        const int gravityConstant = 10;
        const int jumpAmount = 15;
        const int birdHorizontalPosition = 5;
        const int pipeGapSize = 5;
        const int pipePeriodLength = 10;
        const int pipeThickness = 2;

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
                acceleratedUpdate.Interval = GetFrameLength();
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
            service = new GameEngineService(birdHorizontalPosition - 1, pipeThickness, pipeGapSize);

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

            bird = new BirdController(screenHeight / 2, refreshInterval, gravityConstant, jumpAmount);
            environment = new EnvironmentController(screenWidth, screenHeight, pipeGapSize, pipePeriodLength, pipeThickness);

            gameTime.Start();

            fixedUpdate = new Timer(refreshInterval);
            acceleratedUpdate = new Timer(GetFrameLength());

            fixedUpdate.Elapsed += OnFixedUpdate;
            acceleratedUpdate.Elapsed += OnUpdate;

            fixedUpdate.AutoReset = true;
            acceleratedUpdate.AutoReset = true;
        }

        private bool UpdateGame ()
        {
            if (renderFlag)
            {
                Render();
                if (HasCollided())
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

        private void Render ()
        {
            // TODO: Call the game engine service, generate request model with environment and bird and all other necessary parameters
            // Then draw everything in this method from the returned array of chars. 
            Console.Clear();
            var screen = service.GetRender(environment, bird);
            for (var row = 0; row < screenHeight; row++)
            {
                var line = screen.GetRow(row);
                Console.WriteLine(line);
            }
        }

        private bool HasCollided ()
        {
            var hasCollided = service.HasCollided(environment, bird);
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
            bird.Update(flyUpFlag);
            renderFlag = true;
        }

        private void OnUpdate (object sender, EventArgs args)
        {
            environment.Update();
            renderFlag = true;
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
            var newFrameLength = (int)(500f / (timeSpan + 1)) + refreshInterval;
            return newFrameLength;
        }
    }
}
