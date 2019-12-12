using console_flappy_bird.Models;

namespace console_flappy_bird.Interfaces
{
    public interface IConsoleService
    {
        void InitiateNewGame();
        void Render(IEnvironmentController environment, IBirdController bird);
        void UpdateScore(IEnvironmentController environment, IBirdController bird);
        bool HasCollided(IEnvironmentController environment, IBirdController bird);

        void DisplayStartMenu();
        void DisplayEndMenu(IBirdController bird);
        void DisplayScore(IBirdController bird);

        bool TryBackspaceUsername();
        bool TryDeleteUsername();
        bool TryAddCharToUsername(char keyChar);

        BirdControllerModel GetBirdControllerModel();
        EnvironmentControllerModel GetEnvironmentControllerModel();
        int GetRefreshInterval();
        int GetNewFrameLength();
    }
}
