using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol {
    public GameObject gameobject { get; set; }

    private const float rotateVelocity = 90;

    public Patrol(Hero hero, Rect area) {
        //gameobject = GameObject.Instantiate(Resources.Load("Patrol", typeof(GameObject))) as GameObject;
        gameobject = GameObject.Instantiate(Resources.Load("Patrol_Toony", typeof(GameObject))) as GameObject;
        PatrolScript script = gameobject.GetComponent<PatrolScript>();
        script.area = area;
        script.hero = hero;
    }
}
