using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllor : ActionManager {
    public delegate void Restart();
    public static event Restart RestartEvent;

    private Hero hero;
    private Scorer scorer;
    private List<Patrol> patrols;
    private GameFactory factory;
    private bool gameover;

    // Use this for initialization
    void Start () {
        scorer = Scorer.getInstance();
        factory = GameFactory.getInstance();
        PatrolScript.CatchHeroEvent += patrolCatchHero;
        patrols = new List<Patrol>();
        foreach (Rect area in Maze.getArea()) {
            Patrol p = factory.getPatrol(area);
            patrols.Add(p);
        }
        hero = factory.getHero();
        gameover = false;
        PatrolScript.CatchHeroEvent += patrolCatchHero;
        PatrolScript.EscapeEvent += heroEscape;
    }

    private void patrolCatchHero() {
        gameover = true;
    }
    private void heroEscape() {
        scorer.addScore(1);
    }

    // Update is called once per frame
    new void Update () {
        if(!gameover) {
            getInput();
        }
        base.Update();
        Camera.main.transform.position = hero.gameobject.transform.position + new Vector3(0, 6, -6);
    }

    new
    void OnGUI() {
        base.OnGUI();
        if (GUI.Button(View.buttonPos, "重新开始", View.ButtonStyle())) {
            gameover = false;
            RestartEvent();
            scorer.clear();
            Clear();
        }
        GUI.Label(View.scorePos, "Score : " + scorer.getScore(), View.scoreStyle());
        if(gameover) {
            GUI.Label(View.LabelPos, "GAME OVER", View.LabelStyle());
        }
    }

    private void getInput() {
        hero.animator.SetBool("moving", true);
        if (Input.GetKey(KeyCode.UpArrow)) {
            hero.move(Hero.Direction.UP);
        } else if (Input.GetKey(KeyCode.DownArrow)) {
            hero.move(Hero.Direction.DOWN);
        } else if (Input.GetKey(KeyCode.LeftArrow)) {
            hero.move(Hero.Direction.LEFT);
        } else if (Input.GetKey(KeyCode.RightArrow)) {
            hero.move(Hero.Direction.RIGHT);
        } else {
            hero.animator.SetBool("moving", false);
        }
    }
}
