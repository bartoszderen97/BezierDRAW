using System;
using System.Collections.Generic;
using System.Windows;

namespace BezierDRAW
{
    public class MyBezierCurve
    {
        private static List<Point> points;
        public static List<Point> GetMyBezierPoints() { return points; }
        private static double calculateXcoords(double t, double x0, double x1, double x2, double x3)
        {
            return (
                x0 * Math.Pow((1 - t), 3) +
                x1 * 3 * t * Math.Pow((1 - t), 2) +
                x2 * 3 * Math.Pow(t, 2) * (1 - t) +
                x3 * Math.Pow(t, 3)
            );
        }
        private static double calculateYcoords(double t, double y0, double y1, double y2, double y3)
        {
            return (
                y0 * Math.Pow((1 - t), 3) +
                y1 * 3 * t * Math.Pow((1 - t), 2) +
                y2 * 3 * Math.Pow(t, 2) * (1 - t) +
                y3 * Math.Pow(t, 3)
            );
        }       
        public static void CalculateMyBezier(double dt, Point pt0, Point pt1, Point pt2, Point pt3)
        {
            points = new List<Point>();
            for (double t = 0.0; t <= 1.0; t += dt)
            {
                points.Add(new Point(
                    calculateXcoords(t, pt0.X, pt1.X, pt2.X, pt3.X),
                    calculateYcoords(t, pt0.Y, pt1.Y, pt2.Y, pt3.Y)));
            }
        }
    }
}