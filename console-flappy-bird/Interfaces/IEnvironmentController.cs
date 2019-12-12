using console_flappy_bird.Models;
using System;
using System.Collections.Generic;

namespace console_flappy_bird.Interfaces
{
    public interface IEnvironmentController : IUpdateable, ICloneable
    {
        /// <summary>
        /// Gets current PipeColumn objects.
        /// </summary>
        IEnumerable<PipeColumn> GetCurrentPipes();
    }
}
