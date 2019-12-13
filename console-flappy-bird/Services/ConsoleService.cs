using console_flappy_bird.Interfaces;
using console_flappy_bird.Models;
using console_flappy_bird.Resources;
using Pastel;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace console_flappy_bird.Services
{
    public class ConsoleService : IConsoleService
    {
        private char[,] baseRender;
        private Random random;
        private Stopwatch gameTime;
        private ConfigModel config;
        private HighscoreModel highscore;

        private const string configPath = @"C:\Uni\console-flappy-bird\console-flappy-bird\Resources\Config.json";
        private const string highscorePath = @"C:\Uni\console-flappy-bird\console-flappy-bird\Resources\Highscore.json";

        private string username;
        private int cursorPositionForUsername;

        public ConsoleService()
        {
            SetupConsoleService();
        }

        public void DisplayStartMenu()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.Line);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.MainMenuWelcome.Length / 2) + Texts.MainMenuWelcome);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.Line);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            if (highscore.Highscore != 0)
            {
                Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.CurrentHighscore + highscore.Highscore + ", " + highscore.Username);
                Console.WriteLine(new string(' ', (config.ScreenWidth / 2) - (Texts.Line.Length / 2) + Texts.CurrentHighscore.Length) + highscore.Date.ToString("yyyy/MM/dd"));
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.WriteLine();
            cursorPositionForUsername = Console.CursorTop;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.InputUsername + username);
            Console.WriteLine();
            Console.WriteLine();
            Console.Write(new string(' ', config.ScreenWidth / 2 - (Texts.Instructions.Length + Texts.Spacebar.Length) / 2) + Texts.Instructions);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(Texts.Spacebar);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.StartGame.Length / 2) + Texts.StartGame);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.Line);
        }

        public bool TryBackspaceUsername()
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            username = username.Remove(username.Length - 1);
            Console.SetCursorPosition(0, cursorPositionForUsername);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.InputUsername + username + "  ");
            return true;
        }

        public bool TryDeleteUsername()
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            username = string.Empty;
            Console.SetCursorPosition(0, cursorPositionForUsername);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.InputUsername + new string(' ', config.MaxUsernameChars));
            return true;
        }

        public bool TryAddCharToUsername(char keyChar)
        {
            var isAllowedToAdd = string.IsNullOrEmpty(username) || username.Length <= config.MaxUsernameChars;
            if (!isAllowedToAdd)
            {
                return false;
            }
            username += keyChar;
            Console.SetCursorPosition(0, cursorPositionForUsername);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.InputUsername + username);
            return true;
        }

        public void InitiateNewGame()
        {
            if (string.IsNullOrEmpty(username))
            {
                username = config.DefaultUsername + random.Next(10000, 99999).ToString() + "_" + DateTime.Now.ToString("yy/MM/dd");
            }

            highscore = InputOutputService.Load<HighscoreModel>(highscorePath);

            gameTime = new Stopwatch();
            gameTime.Start();
        }

        public BirdControllerModel GetBirdControllerModel()
        {
            var model = new BirdControllerModel
            {
                ScreenHeight = config.ScreenHeight,
                RefreshInterval = config.RefreshInterval,
                GravityConstant = config.GravityConstant,
                JumpAmount = config.JumpAmount
            };
            return model;
        }

        public EnvironmentControllerModel GetEnvironmentControllerModel()
        {
            var model = new EnvironmentControllerModel
            {
                ScreenWidth = config.ScreenWidth,
                ScreenHeight = config.ScreenHeight,
                PipeGapSize = config.PipeGapSize,
                PipePeriodLength = config.PipePeriodLength,
                PipeThickness = config.PipeThickness
            };
            return model;
        }

        public int GetRefreshInterval()
        {
            return config.RefreshInterval;
        }

        public int GetNewFrameLength()
        {
            var timeSpan = gameTime.ElapsedMilliseconds / 1000f;
            var newFrameLength = (int)((500f / (timeSpan + 1)) + config.RefreshInterval);
            return newFrameLength;
        }

        public void Render(IEnvironmentController environmentInstance, IBirdController birdInstance)
        {
            var render = baseRender.Clone() as char[,];
            AddEnvironmentToRender(render, environmentInstance);
            AddBirdToRender(render, birdInstance);

            var line = new (Color color, string value)[config.ScreenWidth];

            for (var y = 0; y < config.ScreenHeight; y++)
            {
                Console.SetCursorPosition(0, y);
                for (var x = 0; x < config.ScreenWidth; x++)
                {
                    line[x] = (GetColorForSymbol(render[x, y]), render[x, y].ToString());
                }
                Console.Write(string.Join(string.Empty, line.Select(s => s.value.Pastel(s.color))));
            }
        }

        public bool HasCollided(IEnvironmentController environment, IBirdController bird)
        {
            var pipes = environment.GetCurrentPipes();
            var birdVerticalPosition = bird.GetPosition();

            if (birdVerticalPosition == 0 || birdVerticalPosition == config.ScreenHeight - 1)
            {
                return true;
            }

            foreach (var pipe in pipes)
            {
                var pipeHorizontalEndLine = pipe.HorizontalPosition + config.PipeThickness;

                // Check horizontal collision
                if (config.BirdHorizontalPosition >= pipe.HorizontalPosition && config.BirdHorizontalPosition < pipeHorizontalEndLine)
                {
                    var pipeVerticalEndLine = pipe.GapStart + config.PipeGapSize;

                    // Check vertical collision
                    if (birdVerticalPosition < pipe.GapStart || birdVerticalPosition >= pipeVerticalEndLine)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void UpdateScore(IEnvironmentController environment, IBirdController bird)
        {
            foreach (var pipe in environment.GetCurrentPipes())
            {
                var pipeHorizontalEndLine = pipe.HorizontalPosition + config.PipeThickness;

                if (!pipe.IsScored && config.BirdHorizontalPosition == pipeHorizontalEndLine)
                {
                    AddScore(pipe, bird);
                }
            }
        }

        public void DisplayEndMenu(IBirdController bird)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.Line);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.GameOver.Length / 2) + Texts.GameOver);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.Line);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.YourHighscore + bird.GetScore().ToString());
            if (bird.GetScore() > highscore.Highscore)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.NewHighscore + username + "!");
                ExportResults(bird.GetScore());
            }
            else
            {
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.IsRetryNeeded);
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(new string(' ', config.ScreenWidth / 2 - Texts.Line.Length / 2) + Texts.Line);
        }

        public void DisplayScore(IBirdController bird)
        {
            Console.SetCursorPosition((config.ScreenWidth / 2) - (Texts.YourHighscore.Length / 2), config.ScreenHeight - 1);
            Console.Write(Texts.YourHighscore + bird.GetScore());
        }

        private void AddScore(PipeColumn pipe, IBirdController bird)
        {
            bird.AddScore();
            pipe.IsScored = true;
        }

        private void ExportResults(int score)
        {
            highscore = new HighscoreModel
            {
                Username = username,
                Highscore = score,
                Date = DateTime.Now
            };
            InputOutputService.Save(highscore, highscorePath);
        }

        private Color GetColorForSymbol(char value)
        {
            if (value == config.PipeBodySymbol ||
                value == config.PipeHoleSymbol ||
                value == config.PipeLeftBottomSymbol ||
                value == config.PipeLeftTopSymbol ||
                value == config.PipeRightBottomSymbol ||
                value == config.PipeRightTopSymbol)
                return Color.Green;

            if (value == config.BirdUpSymbol ||
                value == config.BirdSideSymbol ||
                value == config.BirdDownSymbol)
                return Color.Red;

            return Color.Teal;
        }

        private void SetupConsoleService()
        {
            random = new Random();

            config = InputOutputService.Load<ConfigModel>(configPath);
            highscore = InputOutputService.Load<HighscoreModel>(highscorePath);

            baseRender = new char[config.ScreenWidth, config.ScreenHeight];
            PlaceClouds(baseRender);

            Console.SetWindowSize(config.ScreenWidth, config.ScreenHeight);
            Console.CursorVisible = false;
            Console.Clear();
        }

        private void AddEnvironmentToRender(char[,] render, IEnvironmentController environment)
        {
            var pipes = environment.GetCurrentPipes();
            foreach (var pipe in pipes)
            {
                PlacePipe(render, pipe);
            }
        }

        private void PlacePipe(char[,] render, PipeColumn pipe)
        {
            var horizontalOrigin = pipe.HorizontalPosition;
            var horizontalEnd = pipe.HorizontalPosition + config.PipeThickness;
            var pipeGapTop = pipe.GapStart - 1;
            var pipeGapBottom = pipe.GapStart + config.PipeGapSize;

            for (var y = 0; y < config.ScreenHeight; y++)
            {
                if (IsWithinGap(y, pipeGapTop, pipeGapBottom))
                {
                    continue;
                }
                for (var x = horizontalOrigin; x < horizontalEnd; x++)
                {
                    if (!IsWithinHorizontalBounds(x))
                    {
                        continue;
                    }
                    var onLeftSide = x == horizontalOrigin;
                    var onRightSide = x == horizontalEnd - 1;
                    var onTopGap = y == pipeGapTop;
                    var onBottomGap = y == pipeGapBottom;

                    render[x, y] = GetPipeSymbol(onTopGap, onBottomGap, onLeftSide, onRightSide);
                }
            }
        }

        private char GetPipeSymbol(bool onTopGap, bool onBottomGap, bool onLeftSide, bool onRightSide)
        {
            if (onTopGap)
            {
                if (onLeftSide)
                    return config.PipeLeftTopSymbol;

                if (onRightSide)
                    return config.PipeRightTopSymbol;

                return config.PipeHoleSymbol;
            }

            if (onBottomGap)
            {
                if (onLeftSide)
                    return config.PipeLeftBottomSymbol;

                if (onRightSide)
                    return config.PipeRightBottomSymbol;

                return config.PipeHoleSymbol;
            }

            return config.PipeBodySymbol;
        }

        private bool IsWithinHorizontalBounds(int x)
        {
            if (x >= config.ScreenWidth || x < 0)
            {
                return false;
            }
            return true;
        }

        private bool IsWithinGap(int y, int pipeGapTop, int pipeGapBottom)
        {
            if (y > pipeGapTop && y < pipeGapBottom)
            {
                return true;
            }
            return false;
        }

        private void AddBirdToRender(char[,] render, IBirdController bird)
        {
            var x = config.BirdHorizontalPosition;
            var y = bird.GetPosition();
            render[x, y] = GetBirdSymbol(bird.GetDirection());
        }

        private char GetBirdSymbol(BirdDirection direction)
        {
            switch (direction)
            {
                case BirdDirection.Up:
                    return config.BirdUpSymbol;
                case BirdDirection.Side:
                    return config.BirdSideSymbol;
                default:
                    return config.BirdDownSymbol;
            }
        }

        private void PlaceClouds(char[,] render)
        {
            var currentDensity = config.CloudMaxDensity;
            for (var y = 0; y < config.ScreenHeight; y++)
            {
                var x = random.Next(currentDensity);
                while (x < config.ScreenWidth)
                {
                    render[x, y] = config.CloudSymbol;
                    x += random.Next(currentDensity);
                }
                currentDensity += config.CloudSparsingRate;
            }
        }
    }
}
