using console_flappy_bird.Models;
using System;

namespace console_flappy_bird.Logic
{
    class BirdController : ICloneable
    {
        private Bird bird;
        private readonly int deltaTime;
        private readonly int fallingConstant;
        private readonly int jumpConstant;
        private readonly int screenHeight;

        public BirdController (int screenHeight, int deltaTime, int fallingConstant, int jumpConstant)
        {
            bird = new Bird
            {
                Score = 0,
                Direction = BirdDirection.Side,
                Position = screenHeight / 2,
                Velocity = 0
            };
            this.deltaTime = deltaTime;
            this.fallingConstant = fallingConstant;
            this.jumpConstant = jumpConstant;
            this.screenHeight = screenHeight;
        }

        public void Update (bool jumpFlag)
        {
            if (jumpFlag)
            {
                bird.Velocity = jumpConstant;
            }
            var deltaTimeSecond = deltaTime / 1000f;
            bird.Position -= bird.Velocity * deltaTimeSecond;
            bird.Position = Math.Clamp(bird.Position, 0f, screenHeight - 1);
            bird.Velocity -= fallingConstant * deltaTimeSecond;

            if (bird.Velocity > 0)
            {
                bird.Direction = BirdDirection.Up;
            }
            else if (Math.Abs(bird.Velocity) < fallingConstant)
            {
                bird.Direction = BirdDirection.Side;
            }
            else
            {
                bird.Direction = BirdDirection.Down;
            }
        }

        public void AddScore (int score)
        {
            bird.Score += score;
        }

        public int GetScore ()
        {
            return bird.Score;
        }

        public int GetPosition ()
        {
            return (int)bird.Position;
        }

        public BirdDirection GetDirection ()
        {
            return bird.Direction;
        }

        public object Clone()
        {
            var copy = new BirdController(bird, deltaTime, fallingConstant, jumpConstant);
            return copy;
        }

        private BirdController(Bird bird, int deltaTime, int fallingConstant, int jumpConstant)
        {
            this.bird = new Bird
            {
                Score = bird.Score,
                Direction = bird.Direction,
                Position = bird.Position,
                Velocity = bird.Velocity
            };
            this.deltaTime = deltaTime;
            this.fallingConstant = fallingConstant;
            this.jumpConstant = jumpConstant;
        }
    }
}
