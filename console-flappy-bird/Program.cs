using console_flappy_bird.Interfaces;
using console_flappy_bird.Logic;

namespace console_flappy_bird
{
    class Program
    {
        static void Main(string[] args)
        {
            IGameEngine gameEngine = new GameEngineConsole();
            gameEngine.StartGame();
        }
    }
}
