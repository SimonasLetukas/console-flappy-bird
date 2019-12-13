using console_flappy_bird.Models;

namespace console_flappy_bird.Interfaces
{
    public interface IConsoleService
    {
        /// <summary>
        /// Reset variables and set up class for a new game.
        /// </summary>
        void InitiateNewGame();

        /// <summary>
        /// Calculate and display game field depending on the state of Environment and Bird objects.
        /// </summary>
        void Render(IEnvironmentController environment, IBirdController bird);

        /// <summary>
        /// Calculate and update score inside the Environment and Bird objects.
        /// </summary>
        void UpdateScore(IEnvironmentController environment, IBirdController bird);

        /// <summary>
        /// Returns true if Bird has collided.
        /// </summary>
        bool HasCollided(IEnvironmentController environment, IBirdController bird);



        /// <summary>
        /// Generate and display start menu.
        /// </summary>
        void DisplayStartMenu();

        /// <summary>
        /// Generate and display end menu.
        /// </summary>
        void DisplayEndMenu(IBirdController bird);

        /// <summary>
        /// Generate and display score dialog in-game.
        /// </summary>
        void DisplayScore(IBirdController bird);



        /// <summary>
        /// Returns true if erasing one character from the username was successful.
        /// </summary>
        bool TryBackspaceUsername();

        /// <summary>
        /// Returns true if erasing the whole username was successful.
        /// </summary>
        bool TryDeleteUsername();

        /// <summary>
        /// Returns true if adding a new character to the username was successful.
        /// </summary>
        bool TryAddCharToUsername(char keyChar);



        /// <summary>
        /// Generate and get full BirdControllerModel.
        /// </summary>
        BirdControllerModel GetBirdControllerModel();

        /// <summary>
        /// Generate and get full EnvironmentControllerModel.
        /// </summary>
        EnvironmentControllerModel GetEnvironmentControllerModel();

        /// <summary>
        /// Get constant frame refresh interval in miliseconds.
        /// </summary>
        int GetRefreshInterval();

        /// <summary>
        /// Get new frame refresh interval in miliseconds.
        /// </summary>
        int GetNewFrameLength();
    }
}
