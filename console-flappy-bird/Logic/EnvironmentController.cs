using console_flappy_bird.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace console_flappy_bird.Logic
{
    class EnvironmentController
    {
        private Queue<PipeColumn> pipes;
        private readonly Random random;
        private readonly int gapSize;
        private readonly int thickness;
        private readonly int periodLength;
        private readonly int screenWidth;
        private readonly int screenHeight;
        private readonly int maxPipes;

        public EnvironmentController (int screenWidth, int screenHeight, int gapSize, int periodLength, int thickness = 2)
        {
            pipes = new Queue<PipeColumn>();
            random = new Random();
            this.gapSize = gapSize;
            this.thickness = thickness;
            this.periodLength = periodLength;
            this.screenWidth = screenWidth - 1;
            this.screenHeight = screenHeight;
            maxPipes = screenWidth % periodLength == 0 ? 
                screenWidth / periodLength : screenWidth / periodLength + 1;
            GenerateNewPipe();
        }

        public void Update()
        {
            foreach (var pipe in pipes)
            {
                pipe.HorizontalPosition--;
                if (pipe.HorizontalPosition < 0)
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
            if (pipes.Count == maxPipes)
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
        }
    }
}
