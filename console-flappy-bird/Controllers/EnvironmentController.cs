using console_flappy_bird.Interfaces;
using console_flappy_bird.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace console_flappy_bird.Controllers
{
    class EnvironmentController : IEnvironmentController
    {
        private Queue<PipeColumn> pipes;
        private readonly Random random;
        private readonly int gapSize;
        private readonly int thickness;
        private readonly int periodLength;
        private readonly int screenWidth;
        private readonly int screenHeight;
        private readonly int maxPipes;

        public EnvironmentController (EnvironmentControllerModel model)
        {
            pipes = new Queue<PipeColumn>();
            random = new Random();
            gapSize = model.PipeGapSize;
            thickness = model.PipeThickness;
            periodLength = model.PipePeriodLength;
            screenWidth = model.ScreenWidth - 1;
            screenHeight = model.ScreenHeight;
            maxPipes = screenWidth % periodLength == 0 ? 
                (screenWidth / periodLength) : (screenWidth / periodLength) + 1;
            GenerateNewPipe();
        }

        public void Update(bool flag = true)
        {
            foreach (var pipe in pipes.ToList())
            {
                pipe.HorizontalPosition--;
                if (pipe.HorizontalPosition == screenWidth - periodLength)
                {
                    GenerateNewPipe();
                }
            }
        }

        public IEnumerable<PipeColumn> GetCurrentPipes()
        {
            return pipes.ToList();
        }

        private void GenerateNewPipe()
        {
            var pipe = new PipeColumn();
            
            if (pipes.Count > maxPipes && 
                pipes.Peek().HorizontalPosition < 0 - thickness)
            {
                pipe = pipes.Dequeue();
            }

            ConfigureNewPipe(pipe);
            pipes.Enqueue(pipe);
        }

        private void ConfigureNewPipe(PipeColumn pipe)
        {
            pipe.HorizontalPosition = screenWidth;
            pipe.GapStart = random.Next(screenHeight - gapSize);
            pipe.IsScored = false;

            while (pipes.Count > 0 && pipes.Peek().GapStart == pipe.GapStart)
            {
                pipe.GapStart = random.Next(screenHeight - gapSize);
            }
        }

        public object Clone()
        {
            var copy = new EnvironmentController(pipes, maxPipes, screenWidth, screenHeight, gapSize, periodLength, thickness);
            return copy;
        }

        private EnvironmentController(Queue<PipeColumn> pipes, int maxPipes, int screenWidth, int screenHeight, int gapSize, int periodLength, int thickness)
        {
            this.pipes = new Queue<PipeColumn>();
            foreach (var pipe in pipes.ToList())
            {
                this.pipes.Enqueue(pipe);
            }
            random = new Random();
            this.gapSize = gapSize;
            this.thickness = thickness;
            this.periodLength = periodLength;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.maxPipes = maxPipes;
        }
    }
}
