using System;

namespace Cable.Models;

/// <summary>
/// Cable class
/// </summary>
public class Circle
{
    public Double PositionX { get; set; }
    public Double PositionY { get; set; }
    public Double Radius { get; set; }

    /// <summary>
    /// Check if cable is in collision between two cable
    /// </summary>
    /// <param name="secondX">X-coord of second cable</param>
    /// <param name="secondY">Y-coord of second cable</param>
    /// <param name="secondRadius">radius of second cable</param>
    /// <returns></returns>
    public bool IsCollision(double secondX, double secondY, double secondRadius)
    {
        // calculate distance between cables
        var distance = Math.Sqrt(Math.Pow(PositionX - secondX, 2) + Math.Pow(PositionY - secondY, 2));

        return distance < secondRadius + Radius;
    }
}