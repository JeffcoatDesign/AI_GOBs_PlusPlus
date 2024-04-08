using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatBlock : MonoBehaviour
{
    public string statname;
    public GameObject tally;
    public int value;
    public Transform tallyArea;
    TextMeshProUGUI text;
    List<GameObject> tallies = new();
    public void Create(string name, int value, GameObject tally)
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        statname = name;
        text.text = statname;
        this.tally = tally;
        tallyArea = transform.Find("TallyArea");
        SetValue(value);
    }

    public void SetValue (int value)
    {
        this.value = value;
        while (tallies.Count > 0)
        {
            GameObject old = tallies[0];
            tallies.RemoveAt(0);
            Destroy(old);
        }

        while (tallies.Count < value)
        {
            tallies.Add(Instantiate(tally, tallyArea));
        }
    }
}
