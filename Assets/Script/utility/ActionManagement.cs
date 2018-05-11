using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 结束时需要调用callback，并把destory设置成true
public class Action : ScriptableObject {
    public GameObject gameobject { get; set; }
    public Callback callback;

    public bool enable = true;
    public bool destroy = false; // 当动作完成后，应令destroy为true

    public virtual void Start() {
        throw new System.NotImplementedException();
    }
    public virtual void Update() {
        throw new System.NotImplementedException();
    }
    public virtual void OnGUI() {
            
    }
    public virtual void FixedUpdate() {

    }
}

// 顺序动作
public class SequenceAction : Action, Callback {
    public List<Action> sequence;
    private int i = 0;

    public static Action getAction(List<Action> _seq) {
        SequenceAction action = ScriptableObject.CreateInstance<SequenceAction>();
        action.sequence = _seq;
        return action;
    }

    public override void Start() {
        foreach (Action ac in sequence) {
            ac.callback = this;
        }
        if (sequence.Count != 0) {
            sequence[0].Start();
        } else {
            if (callback != null) {
                callback.call();
            }
            destroy = true;
        }
    }
    public override void Update() {
        if(i < sequence.Count) {
            sequence[i].Update();
        } else {
            destroy = true;
            if(callback != null) {
                callback.call();
            }
        }
    }
    public override void OnGUI() {
        if (i < sequence.Count) {
            sequence[i].OnGUI();
        }
    }
    // 子动作的回调函数
    public void call() {
        sequence[i].destroy = true;
        if (++i < sequence.Count) {
            sequence[i].Start();
        }
    }
}

// 循环动作
public class CycleAction : Action, Callback {
    public List<Action> _begin;
    public List<Action> _cycle;
    private int i = 0;
    private bool inCycle = false;

    public static Action getAction(List<Action> begin, List<Action> cycle) {
        CycleAction action = ScriptableObject.CreateInstance<CycleAction>();
        action._begin = begin;
        action._cycle = cycle;
        return action;
    }

    public override void Start() {
        inCycle = false;
        foreach (Action ac in _cycle) {
            ac.callback = this;
        }
        foreach (Action ac in _begin) {
            ac.callback = this;
        }
        if (_begin.Count != 0) {
            _begin[0].Start();
        } else if (_cycle.Count != 0) {
            inCycle = true;
            _cycle[0].Start();
        } else {
            if (callback != null) {
                callback.call();
            }
            destroy = true;
        }
    }
    public override void Update() {
        if(inCycle) {
            if(_cycle.Count != 0) {
                _cycle[i].Update();
            } else {
                this.destroy = true;
            }
        } else {
            _begin[i].Update();
        }
    }
    public override void OnGUI() {
        if (inCycle) {
            _cycle[i].OnGUI();
        } else {
            _begin[i].OnGUI();
        }
    }
    // 子动作的回调函数
    public void call() {
        if(inCycle) {
            i = (i + 1) % _cycle.Count;
            _cycle[i].Start();
        } else {
            _begin[i].destroy = true;
            if (++i < _begin.Count) {
                // 开始操作还没结束
                _begin[i].Start();
            } else if (_cycle.Count != 0) {
                // 开始操作已结束，进入循环
                i = 0;
                _cycle[0].Start();
                inCycle = true;
            } else {
                // 循环队列无动作
                this.destroy = true;
            }
        }
    }
}

// 移动动作
public class MoveToAction : Action {
    private Vector3 from;
    private Vector3 Del;
    private Vector3 to;
    private float _during;

    private float time = 0;

    public static Action getAction(GameObject gameobject, Vector3 dist, float during) {
        MoveToAction ac = ScriptableObject.CreateInstance<MoveToAction>();
        ac.to = dist;
        ac._during = during;
        ac.gameobject = gameobject;
        return ac;
    }
    public static Action getActionByVelocity(GameObject gameobject, Vector3 dist, float velocity) {
        float during = (gameobject.transform.position - dist).magnitude / velocity;
        return getAction(gameobject, dist, during);
    }

    public override void Start() {
        time = 0;
        from = gameobject.transform.position;
        Del = to - from;
    }
    public override void Update() {
        if (time < _during) {
            gameobject.transform.position += Del * Time.deltaTime / _during;
            time += Time.deltaTime;
        } else {
            gameobject.transform.position = to;
            destroy = true;
            if(callback != null) {
                callback.call();
            }
        }
    }
}

// 转体动作
public class RotateToAction : Action {
    private Vector3 targetPos;
    private Quaternion dist;
    private Vector3 Del;
    private float during;

    private float time = 0;

    public static Action getAction(GameObject gameobject, Vector3 dist, float during) {
        RotateToAction ac = ScriptableObject.CreateInstance<RotateToAction>();
        ac.targetPos = dist;
        ac.gameobject = gameobject;
        ac.during = during;
        return ac;
    }
    public static Action getActionByVelocity(GameObject gameobject, Vector3 dist, float velocity) {
        Vector3 e = Quaternion.LookRotation(dist).eulerAngles - gameobject.transform.rotation.eulerAngles;
        float during = e.magnitude / velocity;
        return getAction(gameobject, dist, during);
    }

    public override void Start() {
        time = 0;
        dist = Quaternion.LookRotation(targetPos - gameobject.transform.position);
        Del = dist.eulerAngles - gameobject.transform.rotation.eulerAngles;
    }
    public override void Update() {
        if (time < during) {
            gameobject.gameObject.transform.rotation *= Quaternion.Euler(Del * Time.deltaTime / during);
            time += Time.deltaTime;
        } else {
            gameobject.transform.rotation = dist;
            destroy = true;
            if (callback != null) {
                callback.call();
            }
        }
    }
}

public interface Callback {
    void call();
}

public class ActionManager : MonoBehaviour {
    private Dictionary<int, Action> actions = new Dictionary<int, Action>();
    private List<Action> waitingAdd = new List<Action>();
    private List<int> waitingDelete = new List<int>();

    public void Update() {
        foreach (Action ac in waitingAdd) {
            actions[ac.GetInstanceID()] = ac;
        }
        waitingAdd.Clear();
        foreach (KeyValuePair<int, Action> kv in actions) {
            Action ac = kv.Value;
            if (ac.destroy) {
                waitingDelete.Add(kv.Key);
            } else if (ac.enable) {
                ac.Update();
            }
        }
        foreach (int key in waitingDelete) {
            Action ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }

    public void FixedUpdate() {
        foreach (Action ac in waitingAdd) {
            actions[ac.GetInstanceID()] = ac;
        }
        waitingAdd.Clear();
        foreach (KeyValuePair<int, Action> kv in actions) {
            Action ac = kv.Value;
            if (ac.destroy) {
                waitingDelete.Add(kv.Key);
            } else if (ac.enable) {
                ac.FixedUpdate();
            }
        }
        foreach (int key in waitingDelete) {
            Action ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }

    public void OnGUI() {
        foreach (KeyValuePair<int, Action> kv in actions) {
            Action ac = kv.Value;
            ac.OnGUI();
        }
    }
    // 执行动作
    public void RunAction(Action action) {
        waitingAdd.Add(action);
        action.Start();
    }
    // 清空动作队列
    public void Clear() {
        foreach (Action ac in waitingAdd) {
            DestroyObject(ac);
        }
        foreach (KeyValuePair<int, Action> kv in actions) {
            DestroyObject(kv.Value);
        }
        actions.Clear();
        waitingAdd.Clear();
        waitingDelete.Clear();
    }
    // 结束指定动作id号的动作(id通过ac.GetInstanceID()方法得到)
    public void Stop(int id) {
        actions[id].destroy = true;
    }
}