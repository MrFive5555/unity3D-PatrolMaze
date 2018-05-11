using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolScript : MonoBehaviour {
    public delegate void CatchHero();
    public static event CatchHero CatchHeroEvent;

    public delegate void Escape();
    public static event Escape EscapeEvent;

    private const float speed = 5;

    /* 分别为巡逻兵巡逻范围左上角坐标的x, z，x方向的长，z方向的宽 */
    public Rect area;
    public Hero hero;
    private Vector3 target;
    private bool isChasing;
    private bool hasCatch;

    // Use this for initialization
    void Start () {
        target = getRandomTarget();
        gameObject.transform.position = getRandomPosition();
        hasCatch = false;
        GameControllor.RestartEvent += restart;
    }
	
	// Update is called once per frame
	void Update () {
        if (shouldChase() && !hasCatch) {
            target = hero.gameobject.transform.position;
            isChasing = true;
        } else if(isChasing) { // 停止抓捕
            isChasing = false;
            EscapeEvent();
            target = getRandomPosition();
        } else if((target - gameObject.transform.position).magnitude < 3) {
            target = getRandomPosition();
        }
        move();
    }

    void OnCollisionStay(Collision e) {
        string name = e.gameObject.name;
        if(name.Contains("Hero")) {
            CatchHeroEvent();
            hasCatch = true;
            target = getRandomTarget();
        } else if(name.Contains("wall") || name.Contains("Patrol")) {
            target = getRandomTarget();
        }
    }

    private Vector3 getRandomTarget() {
        float height = gameObject.transform.lossyScale.y;
        //float height = 0;
        switch (Random.Range(0, 4)) {
            case 0:
                return new Vector3(Random.Range(area.xMin, area.xMax), height / 2, area.yMax);
            case 1:
                return new Vector3(area.xMin, height / 2, Random.Range(area.yMin, area.yMax));
            case 2:
                return new Vector3(Random.Range(area.xMin, area.xMax), height / 2, area.yMin);
            default:
                return new Vector3(area.xMax, height / 2, Random.Range(area.yMin, area.yMax));
        }
    }
    private Vector3 getRandomPosition() {
        return new Vector3(
            Random.Range(area.xMin, area.xMax),
            gameObject.transform.lossyScale.y / 2,
            //0,
            Random.Range(area.yMin, area.yMax)
        );
    }

    private bool shouldChase() {
        Vector3 pos = hero.gameobject.transform.position;
        return area.xMin <= pos.x && pos.x <= area.xMax
            && area.yMin <= pos.z && pos.z <= area.yMax;
    }

    private void move() {
        //Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        //rigidbody.velocity = (target - gameObject.transform.position).normalized * speed;
        //rigidbody.velocity -= Vector3.up * rigidbody.velocity.y;

        gameObject.transform.position += (target - gameObject.transform.position).normalized * speed * Time.deltaTime;
        gameObject.transform.rotation = Quaternion.LookRotation(target - gameObject.transform.position);
    }

    private void restart() {
        isChasing = false;
        hasCatch = false;
        gameObject.transform.position = getRandomPosition();
    }
}
