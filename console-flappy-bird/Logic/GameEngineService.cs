using console_flappy_bird.Models;
using System;

namespace console_flappy_bird.Logic
{
    class GameEngineService
    {
        private readonly GameEngineServiceModel model;
        private readonly char[,] baseRender;
        private readonly Random random;

        //  |||||
        //  |||||
        //  |||||
        //  ⌊===⌋

        //  ^ > v

        //  ⌈===⌉
        //  |||||
        //  |||||
        //  |||||

        public GameEngineService (GameEngineServiceModel model)
        {
            this.model = model;
            random = new Random();

            baseRender = new char[model.ScreenWidth, model.ScreenHeight];
            PlaceClouds(baseRender);
        }

        public bool HasCollided (EnvironmentController environment, BirdController bird)
        {
            var pipes = environment.GetCurrentPipes();
            var birdVerticalPosition = bird.GetPosition();
            foreach (var pipe in pipes)
            {
                var pipeHorizontalEndLine = pipe.HorizontalPosition + model.PipeThickness;

                // Check horizontal collision
                if (model.BirdHorizontalPosition >= pipe.HorizontalPosition && model.BirdHorizontalPosition < pipeHorizontalEndLine)
                {
                    var pipeVerticalEndLine = pipe.GapStart + model.PipeGapSize;

                    // Check vertical collision
                    if (birdVerticalPosition < pipe.GapStart && birdVerticalPosition >= pipeVerticalEndLine)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public char[,] GetRender (EnvironmentController environment, BirdController bird)
        {
            var render = baseRender.Clone() as char[,];

            AddEnvironment(render, environment);
            AddBird(render, bird);

            return render;
        }

        private void AddEnvironment (char[,] render, EnvironmentController environment)
        {
            var pipes = environment.GetCurrentPipes();
            foreach (var pipe in pipes)
            {
                PlacePipe(render, pipe);
            }
        }

        private void PlacePipe (char[,] render, PipeColumn pipe)
        {
            var horizontalOrigin = pipe.HorizontalPosition;
            var horizontalEnd = pipe.HorizontalPosition + model.PipeThickness;
            var pipeGapTop = pipe.GapStart - 1;
            var pipeGapBottom = pipe.GapStart + model.PipeGapSize;

            for (var y = 0; y < model.ScreenHeight; y++)
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

        private char GetPipeSymbol (bool onTopGap, bool onBottomGap, bool onLeftSide, bool onRightSide)
        {
            if (onTopGap)
            {
                if (onLeftSide)
                    return model.PipeLeftTopSymbol;

                if (onRightSide)
                    return model.PipeRightTopSymbol;

                return model.PipeHoleSymbol;
            }

            if (onBottomGap)
            {
                if (onLeftSide)
                    return model.PipeLeftBottomSymbol;

                if (onRightSide)
                    return model.PipeRightBottomSymbol;

                return model.PipeHoleSymbol;
            }

            return model.PipeBodySymbol;
        }

        private bool IsWithinHorizontalBounds (int x)
        {
            if (x >= model.ScreenWidth || x < 0)
            {
                return false;
            }
            return true;
        }

        private bool IsWithinGap (int y, int pipeGapTop, int pipeGapBottom)
        {
            if (y > pipeGapTop && y < pipeGapBottom)
            {
                return true;
            }
            return false;
        }

        private void AddBird (char[,] render, BirdController bird)
        {
            var x = model.BirdHorizontalPosition;
            var y = bird.GetPosition();
            render[x, y] = GetBirdSymbol(bird.GetDirection());
        }

        private char GetBirdSymbol (BirdDirection direction)
        {
            switch (direction)
            {
                case BirdDirection.Up:
                    return model.BirdUpSymbol;
                case BirdDirection.Side:
                    return model.BirdSideSymbol;
                default:
                    return model.BirdDownSymbol;
            }
        }

        private void PlaceClouds(char[,] render)
        {
            var currentDensity = model.CloudMaxDensity;
            for (var y = 0; y < model.ScreenHeight; y++)
            {
                var x = random.Next(currentDensity);
                while (x < model.ScreenWidth)
                {
                    render[x, y] = model.CloudSymbol;
                    x += random.Next(currentDensity);
                }
                currentDensity += model.CloudSparsingRate;
            }
        }
    }
}
