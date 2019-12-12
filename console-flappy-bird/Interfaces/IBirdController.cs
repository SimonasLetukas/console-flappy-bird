using console_flappy_bird.Models;
using System;

namespace console_flappy_bird.Interfaces
{
    public interface IBirdController : IUpdateable, ICloneable
    {
        /// <summary>
        /// Adds score to the current Bird.
        /// </summary>
        void AddScore();

        /// <summary>
        /// Gets the current score from Bird.
        /// </summary>
        int GetScore();

        /// <summary>
        /// Gets current Bird position in vertical axis.
        /// </summary>
        int GetPosition();

        /// <summary>
        /// Gets current Bird direction.
        /// </summary>
        BirdDirection GetDirection();
    }
}
