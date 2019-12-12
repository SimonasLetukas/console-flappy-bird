namespace console_flappy_bird.Models
{
    class GameEngineServiceModel
    {
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public char CloudSymbol { get; set; }
        public int CloudMaxDensity { get; set; }
        public int CloudSparsingRate { get; set; }
        public char BirdUpSymbol { get; set; }
        public char BirdSideSymbol { get; set; }
        public char BirdDownSymbol { get; set; }
        public int BirdHorizontalPosition { get; set; }
        public char PipeLeftTopSymbol { get; set; }
        public char PipeRightTopSymbol { get; set; }
        public char PipeLeftBottomSymbol { get; set; }
        public char PipeRightBottomSymbol { get; set; }
        public char PipeHoleSymbol { get; set; }
        public char PipeBodySymbol { get; set; }
        public int PipeThickness { get; set; }
        public int PipeGapSize { get; set; }
    }
}
