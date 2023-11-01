using System.Collections.Generic;
using UnityEngine;


public class SkillTree
{
    public string name { get; set; }
    public int id { get; set; }
    public KeyCode key { get; set; }
    public List<SkillTree> next { get; set; }
    public float skillTime { get; set; }
    public bool isRunSkill { get; set; }
    public float outOfTime { get; set; }
    public bool isCanForceStop { get; set; }
    public int forceTimes { get; set; }
    public SkillType skillType { get; set; }
    public float holdingTime { get; set; }
    public int defaultForceTimes { get; set; }

    public float forceResetTime { get; set; }
    public Color progressColor { get; set; }

    public SkillTree() { 
        skillType = SkillType.Click;
        progressColor = Color.white;
        next = new List<SkillTree>();
    }
}
