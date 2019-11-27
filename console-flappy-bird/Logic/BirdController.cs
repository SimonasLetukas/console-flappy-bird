using console_flappy_bird.Models;
using System;

namespace console_flappy_bird.Logic
{
    class BirdController
    {
        private Bird bird;
        public BirdController (int position)
        {
            bird = new Bird
            {
                Score = 0,
                Direction = BirdDirection.Side,
                Position = position,
                Velocity = 0
            };
        }

        public void Update (bool jumpFlag, int deltaTime, int fallingConstant, int jumpConstant)
        {
            if (jumpFlag)
            {
                bird.Velocity = jumpConstant;
            }
            var deltaTimeSecond = deltaTime / 1000f;
            bird.Position += bird.Velocity * deltaTimeSecond;
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

        public char GetSymbol ()
        {
            switch (bird.Direction)
            {
                case BirdDirection.Up:
                    return '^';
                case BirdDirection.Side:
                    return '>';
                default:
                    return 'v';
            }
        }
    }
}
