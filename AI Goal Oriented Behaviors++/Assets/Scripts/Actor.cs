using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;
using System;
using TMPro;

public class Actor : MonoBehaviour
{
    public StatsManager statsManager;
    List<Goal> myGoals;
    List<Action> myActions = new();
    NavMeshAgent m_navMeshAgent;
    Action m_currentAction;
    bool startedAction = false;
    public TextMeshProUGUI text;
    public void Add(Action action)
    {
        myActions.Add(action);
    }

    private void Awake()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();

        myGoals = new List<Goal>
        {
            new Goal("Hunger", 5f, 2f),
            new Goal("Thirst", 4f, 2f),
            new Goal("Sleep", 4f, 2f),
            new Goal("Bathroom", 4f, 1f)
        };

        statsManager.UpdateStats(myGoals);

        InvokeRepeating(nameof(Decay), 0f, 5f);
    }

    private void Update()
    {
        if (m_currentAction == null) {
            m_currentAction = ChooseAction(myActions, myGoals);
            m_navMeshAgent.SetDestination(m_currentAction.actionPoint.position);
            text.text = m_currentAction.name;
        }
        if (m_currentAction.GetDistance(transform) < 1.1f && !startedAction)
        {
            TakeAction();
            startedAction = true;
        }
    }

    private void ClearAction ()
    {
        m_currentAction = null;
        startedAction = false;
        text.text = "";
    }

    private void Decay()
    {
        string goalsStatus = "";
        foreach (Goal goal in myGoals)
        {
            goalsStatus += goal.name + " " + goal.value + "\n";
            goal.value += goal.changeRate;
        }
        Debug.Log(goalsStatus);
        statsManager.UpdateStats(myGoals);
    }
    private void TakeAction () {
        string goalsStatus = "";
        foreach (Goal goal in myGoals)
        {
            goal.value += m_currentAction.GetGoalChange(goal);
            goal.value = Mathf.Clamp(goal.value, 0f, goal.value);
            goalsStatus += goal.name + " " + goal.value + "\n";
        }
        Debug.Log("I will: " + m_currentAction.name + "\n" + goalsStatus);
        Invoke("ClearAction", m_currentAction.duration);
        statsManager.UpdateStats(myGoals);
    }

    Action ChooseAction (List<Action> actions, List<Goal> goals)
    {
        Action bestAction = actions[0];
        float bestValue = float.MaxValue;

        foreach (var action in actions) {
            float thisValue = GetDiscontentment(action, goals);
            if (thisValue < bestValue)
            {
                bestValue = thisValue;
                bestAction = action;
            }
        }

        return bestAction;
    }

    private float GetDiscontentment(Action action, List<Goal> goals)
    {
        float discontentment = 0f;

        foreach (var goal in goals)
        {
            float newValue = goal.value + action.GetGoalChange(goal);

            newValue += action.GetDistance(transform) / 10f;

            // This won't stop making them only drink or go to the bathroom
            //newValue += action.duration * goal.changeRate;

            discontentment += goal.GetDiscontentment(newValue);
        }

        return discontentment;
    }
}

[Serializable]
public class Action {
    public string name;
    public List<Goal> effectedGoals;
    public float duration;
    [HideInInspector] public Transform actionPoint;
    public Action (string name)
    {
        this.name = name;
    }

    public float GetGoalChange(Goal goal)
    {
        Goal effectedGoal = effectedGoals.FirstOrDefault(g => g.name == goal.name);
        if (effectedGoal != null) { 
            return effectedGoal.value;
        }
        return 0f;
    }

    public float GetDistance (Transform actorTransform)
    {
        return Vector3.Distance(actionPoint.position, actorTransform.position);
    }
}
[Serializable]
public class Goal {
    public string name;
    public float value;
    public float changeRate;

    public Goal (string name, float value, float changeRate)
    {
        this.name = name;
        this.value = value;
        this.changeRate = changeRate;
    }

    public float GetDiscontentment(float newValue)
    {
        return newValue * newValue;
    }
}