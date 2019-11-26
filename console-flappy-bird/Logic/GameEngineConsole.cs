using console_flappy_bird.Interfaces;
using console_flappy_bird.Models;
using System;
using System.Threading;

namespace console_flappy_bird.Logic
{
    class GameEngineConsole : IGameEngine
    {
        // Config
        const int maxUsernameChars = 20;
        const int screenWidth = 80;
        const int screenHeight = 15;

        // Variables
        string username;
        int score;
        bool flyUpFlag;
        bool retryGame;
        Random random;

        public void StartGame ()
        {
            SetupStart();
            DisplayStartMenu();
            while (UpdateGame())
            {
                Thread.Sleep(GetFrameLength());
            }
            DisplayEndMenu();
        }

        private void SetupStart ()
        {
            Console.SetWindowSize(screenWidth, screenHeight);
            Console.CursorVisible = false;
            Console.Clear();
            score = 0;
            flyUpFlag = false;
            retryGame = false;
            random = new Random();
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
            Console.WriteLine(new string(' ', screenWidth / 2 - Texts.Line.Length / 2) + Texts.CurrentHighscore + ", "); // TODO: Add name and maybe date of the highscore.
            Console.WriteLine();
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
                    if (string.IsNullOrEmpty(username))
                    {
                        username = "User" + random.Next(10000, 99999).ToString() + "_" + DateTime.Now.ToString("yy/MM/dd");
                    }
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

        private bool UpdateGame ()
        {

            return false;
        }

        private void DisplayEndMenu ()
        {

            retryGame = false;
        }

        private int GetFrameLength ()
        {
            return 800;
        }
    }
}
