using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero {
    const int SPEED = 8;
    public GameObject gameobject { get; set; }
    private Vector3 orientPosition;
    public Animator animator;

    public Hero() {
        gameobject = GameObject.Instantiate(Resources.Load("Hero_Toony", typeof(GameObject))) as GameObject;
        orientPosition = new Vector3(25, gameobject.transform.lossyScale.y / 2, 25);
        gameobject.transform.position = orientPosition;
        GameControllor.RestartEvent += restart;
        PatrolScript.CatchHeroEvent += death;
        animator = gameobject.GetComponent<Animator>();
    }
    public enum Direction {
        UP, DOWN, LEFT, RIGHT
    };
    public void move(Direction dir) {
        Vector3 delPosition = Vector3.zero;
        switch(dir) {
            case Direction.UP:
                delPosition = Vector3.forward * SPEED * Time.deltaTime;
                gameobject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case Direction.DOWN:
                delPosition = Vector3.back * SPEED * Time.deltaTime;
                gameobject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                break;
            case Direction.LEFT:
                delPosition = Vector3.left * SPEED * Time.deltaTime;
                gameobject.transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                break;
            case Direction.RIGHT:
                delPosition = Vector3.right * SPEED * Time.deltaTime;
                gameobject.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                break;
            default:
                break;
        }
        gameobject.transform.position += delPosition;
    }

    void death() {
        animator.SetBool("death", true);
    }
    void restart() {
        gameobject.transform.position = orientPosition;
        animator.SetBool("death", false);
    }
}
