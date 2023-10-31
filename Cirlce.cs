using System;

namespace Cable;

public class Cirlce
{
    public Double PositionX { get; set; }
    public Double PositionY { get; set; }
    public Double Radius { get; set; }

    public bool IsCollision(double secondX, double secondY, double secondRadius)
    {
        var test = Math.Sqrt(Math.Pow(PositionX - secondX, 2) + Math.Pow(PositionY - secondY, 2));

        if (test < secondRadius + Radius)
        {
            return true;
        }

        return false;
    }
}