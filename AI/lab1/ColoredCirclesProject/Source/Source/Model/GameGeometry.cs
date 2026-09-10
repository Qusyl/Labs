namespace Source.Model
{
    public class GameGeometry
{
    public  Ring[] Rings { get; set; } =
    [
           new()
            {
                Name = "Red",
                Positions = [0, 1, 2, 3, 4]
            },

            new()
            {
                Name = "Yellow",
                Positions = [3, 4, 5, 6, 7]
            },

            new()
            {
                Name = "White",
                Positions = [7, 8, 9, 10, 11]
            }
    ];
}
}