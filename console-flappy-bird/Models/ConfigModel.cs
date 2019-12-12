namespace console_flappy_bird.Models
{
    public class ConfigModel
    {
        public int MaxUsernameChars { get; set; }
        public string DefaultUsername { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public int RefreshInterval { get; set; }

        public int PipeGapSize { get; set; }
        public int PipePeriodLength { get; set; }
        public int PipeThickness { get; set; }
        public char PipeLeftTopSymbol { get; set; }
        public char PipeRightTopSymbol { get; set; }
        public char PipeLeftBottomSymbol { get; set; }
        public char PipeRightBottomSymbol { get; set; }
        public char PipeHoleSymbol { get; set; }
        public char PipeBodySymbol { get; set; }

        public int CloudSparsingRate { get; set; }
        public int CloudMaxDensity { get; set; }
        public char CloudSymbol { get; set; }

        public int GravityConstant { get; set; }
        public int JumpAmount { get; set; }
        public int BirdHorizontalPosition { get; set; }
        public char BirdUpSymbol { get; set; }
        public char BirdSideSymbol { get; set; }
        public char BirdDownSymbol { get; set; }
    }
}
