using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze {
    public static List<Rect> getArea() {
        List<Rect> areas = new List<Rect>();
        areas.Add(new Rect(-50, -50, 30, 30));
        areas.Add(new Rect(-50, -50, 30, 30));

        areas.Add(new Rect(-20, -50, 40, 30));

        areas.Add(new Rect( 20, -50, 30, 30));

        areas.Add(new Rect(-50, -20, 30, 40));
        areas.Add(new Rect(-50, -20, 30, 40));
        areas.Add(new Rect(-50, -20, 30, 40));

        areas.Add(new Rect(-20,  -20, 40, 20));

        areas.Add(new Rect( 20, -20, 30, 40));
        areas.Add(new Rect( 20, -20, 30, 40));
        areas.Add(new Rect(20, -20, 30, 40));

        areas.Add(new Rect(-50, 20, 30, 30));

        areas.Add(new Rect(-20, 0, 40, 50));
        areas.Add(new Rect(-20, 0, 40, 50));
        areas.Add(new Rect(-20, 0, 40, 50));
        areas.Add(new Rect(-20, 0, 40, 50));
        return areas;
    }
}
