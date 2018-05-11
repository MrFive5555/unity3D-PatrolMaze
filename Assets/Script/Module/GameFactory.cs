using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFactory {
    private static GameFactory _instance;
    private GameFactory() { }
    
    static public GameFactory getInstance() {
        if (_instance == null) {
            _instance = new GameFactory();
        }
        return _instance;
    }

    Hero _hero;
    public Hero getHero() {
        if(_hero == null) {
            _hero = new Hero();
        }
        return _hero;
    }

    public Patrol getPatrol(Rect area) {
        return new Patrol(getHero(), area);
    }
}
