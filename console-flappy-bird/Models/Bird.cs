namespace console_flappy_bird.Models
{
    class Bird
    {
        public int Score { get; set; }
        public BirdDirection Direction { get; set; }
        public float Position { get; set; }
        public float Velocity { get; set; }
    }
}
