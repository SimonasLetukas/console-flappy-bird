namespace console_flappy_bird.Logic
{
    class GameEngineService
    {
        private readonly int birdHorizontalPosition;
        private readonly int pipeThickness;
        private readonly int pipeGapSize;

        public GameEngineService (int birdHorizontalPosition, int pipeThickness, int pipeGapSize)
        {
            this.birdHorizontalPosition = birdHorizontalPosition;
            this.pipeThickness = pipeThickness;
            this.pipeGapSize = pipeGapSize;
        }

        public bool HasCollided (EnvironmentController environment, BirdController bird)
        {
            var pipes = environment.GetCurrentPipes();
            var birdVerticalPosition = bird.GetPosition();
            foreach (var pipe in pipes)
            {
                var pipeHorizontalEndLine = pipe.HorizontalPosition + pipeThickness;

                // Check horizontal collision
                if (birdHorizontalPosition >= pipe.HorizontalPosition && birdHorizontalPosition < pipeHorizontalEndLine)
                {
                    var pipeVerticalEndLine = pipe.GapStart + pipeGapSize;

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
            return new char[1,1];
        }
    }
}
