namespace console_flappy_bird.Models
{
    class Bird
    {
        public Bird ()
        {

        }
        public string Name { get; set; }
        public int Score { get; set; }
        public BirdDirection Direction { get; set; }
        public int Position { get; set; }
        public double Velocity { get; set; }
    }
}
