using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public GameObject statBlockPrefab;
    public GameObject tallyPrefab;
    public Dictionary<string, StatBlock> statBlockPairs = new Dictionary<string, StatBlock>();

    public void UpdateStats(List<Goal> stats)
    {
        foreach (Goal goal in stats)
        {
            if (statBlockPairs.ContainsKey(goal.name))
            {
                statBlockPairs[goal.name].SetValue(Mathf.RoundToInt(goal.value));
            }
            else
            {
                StatBlock statBlock = Instantiate(statBlockPrefab, transform).GetComponent<StatBlock>();
                statBlock.Create(goal.name, Mathf.RoundToInt(goal.value), tallyPrefab);
                statBlockPairs.Add(goal.name, statBlock);
            }
        }
    }
 }
