using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

public class Block
{
    public Point[] Shape { get;  set; }  // Punkty definiujące kształt bloku
    public Color Color { get;  set; }    // Kolor bloku

    public Block(Point[] shape, Color color)
    {
        Shape = shape;
        Color = color;
    }

    // Metoda do obrotu kształtu
    public Point[] GetRotatedShape()
    {
        Point[] rotatedShape = new Point[Shape.Length];
        for (int i = 0; i < Shape.Length; i++)
        {
            var point = Shape[i];
            rotatedShape[i] = new Point(-point.Y, point.X); // Obrót o 90 stopni
        }
        return rotatedShape;
    }

    public void Rotate()
    {
        // Obrót bloku o 90 stopni
        for (int i = 0; i < Shape.Length; i++)
        {
            var x = Shape[i].X;
            Shape[i].X = -Shape[i].Y;
            Shape[i].Y = x;
        }
    }
}
