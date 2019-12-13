using console_flappy_bird.Interfaces;
using console_flappy_bird.Services;
using System;
using System.Timers;

namespace console_flappy_bird.Controllers
{
    sealed class ConsoleController
    {
        private static readonly object singletonLock = new object();
        private static ConsoleController instance = null;
        public static ConsoleController Instance
        {
            get
            {
                lock (singletonLock)
                {
                    if (instance is null)
                    {
                        instance = new ConsoleController();
                    }
                }
                return instance;
            }
        }

        private bool flyUpFlag;
        private bool renderFlag;
        private bool retryGame;
        private Timer fixedUpdate;
        private Timer acceleratedUpdate;
        private object fixedUpdateLock;
        private object acceleratedUpdateLock;
        private IBirdController bird;
        private IEnvironmentController environment;
        private IConsoleService service;

        public void StartGame()
        {
            SetupStart();
            HandleStartMenu();

        retry:
            while (UpdateGame())
            {
            }

            HandleEndMenu();
            if (retryGame)
            {
                retryGame = false;
                InitiateGame();
                goto retry;
            }
        }

        private void SetupStart()
        {
            flyUpFlag = false;
            renderFlag = true;
            fixedUpdateLock = new object();
            acceleratedUpdateLock = new object();

            service = new ConsoleService();
        }

        private void HandleStartMenu()
        {
            service.DisplayStartMenu();

            while (true)
            {
                var keypress = Console.ReadKey(true);

                if (keypress.Key == ConsoleKey.Spacebar)
                {
                    InitiateGame();
                    break;
                }

                if (keypress.Key == ConsoleKey.Backspace)
                {
                    if (service.TryBackspaceUsername())
                        continue;
                }

                if (keypress.Key == ConsoleKey.Delete)
                {
                    if (service.TryDeleteUsername())
                        continue;
                }

                service.TryAddCharToUsername(keypress.KeyChar);
            }
        }

        private void InitiateGame()
        {
            service.InitiateNewGame();

            bird = new BirdController(service.GetBirdControllerModel());
            environment = new EnvironmentController(service.GetEnvironmentControllerModel());

            if (fixedUpdate != null)
            {
                fixedUpdate.Stop();
                fixedUpdate.Dispose();
            }
            if (acceleratedUpdate != null)
            {
                acceleratedUpdate.Stop();
                acceleratedUpdate.Dispose();
            }

            fixedUpdate = new Timer(service.GetRefreshInterval());
            acceleratedUpdate = new Timer(service.GetNewFrameLength());

            fixedUpdate.Elapsed += OnFixedUpdate;
            acceleratedUpdate.Elapsed += OnUpdate;

            fixedUpdate.AutoReset = true;
            acceleratedUpdate.AutoReset = true;

            fixedUpdate.Enabled = true;
            acceleratedUpdate.Enabled = true;
        }

        private bool UpdateGame()
        {
            if (renderFlag)
            {
                UpdateScore();

                var environmentInstance = GetEnvironmentControllerInstance();
                var birdInstance = (IBirdController)bird.Clone();

                service.Render(environmentInstance, birdInstance);
                service.DisplayScore(birdInstance);

                if (service.HasCollided(environmentInstance, birdInstance))
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

        private void UpdateScore()
        {
            if (System.Threading.Monitor.TryEnter(acceleratedUpdateLock))
            {
                try
                {
                    service.UpdateScore(environment, bird);
                    return;
                }
                finally
                {
                    System.Threading.Monitor.Exit(acceleratedUpdateLock);
                }
            }
            UpdateScore();
        }

        private IEnvironmentController GetEnvironmentControllerInstance()
        {
            if (System.Threading.Monitor.TryEnter(acceleratedUpdateLock))
            {
                try
                {
                    return (IEnvironmentController)environment.Clone();
                }
                finally
                {
                    System.Threading.Monitor.Exit(acceleratedUpdateLock);
                }
            }
            return GetEnvironmentControllerInstance();
        }

        private void HandleInput()
        {
            var keypress = Console.ReadKey(true);
            if (keypress.Key != ConsoleKey.Spacebar)
            {
                return;
            }

            flyUpFlag = true;
        }

        private void OnFixedUpdate(object sender, EventArgs args)
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

        private void OnUpdate(object sender, EventArgs args)
        {
            if (System.Threading.Monitor.TryEnter(acceleratedUpdateLock))
            {
                try
                {
                    acceleratedUpdate.Interval = service.GetNewFrameLength();
                    environment.Update(true);
                }
                finally
                {
                    System.Threading.Monitor.Exit(acceleratedUpdateLock);
                }
            }
        }

        private void HandleEndMenu()
        {
            service.DisplayEndMenu(bird);

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
    }
}
