

namespace Source.Model
{
    public class Ring
    {
        public string Name { get; set; }

        public int[] Positions { get; set; }

        //Для юайки
        public string Color { get; set; } = "#808080";
        public double X { get; set; }
        public double Y { get; set; }
        public double Radius { get; set; } = 150;
        public bool Clockwise { get; set; } = true;
    }
}