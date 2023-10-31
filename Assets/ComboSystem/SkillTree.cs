using UnityEngine;

public enum SkillType {
    Click,
    Hold,
    Charge
}

public class SkillTree
{
    public string name { get; set; }
    public int id { get; set; }
    public KeyCode key { get; set; }
    public SkillTree next { get; set; }
    public float skillTime { get; set; }
    public bool isRunSkill { get; set; }
    public float outOfTime { get; set; }
    public bool isCanForceStop { get; set; }
    public int forceTimes { get; set; }
    public SkillType skillType { get; set; }
    public float longPressTime { get; set; }
    public int defaultForceTimes { get; set; }
    public Color progressColor { get; set; }

    public SkillTree() { 
        skillType = SkillType.Click;
        progressColor = Color.white;
    }
}
