using System.Collections.Generic;
using UnityEngine;

public class SkillTree
{
    /// <summary>
    /// 技能名称
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// 技能id 一系列技能使用同一id
    /// </summary>
    public int id { get; set; }
    /// <summary>
    /// 键位需求
    /// </summary>
    public KeyCode key { get; set; }
    /// <summary>
    /// 下一连招列表
    /// </summary>
    public List<SkillTree> next { get; set; }
    /// <summary>
    /// 技能持续时间
    /// </summary>
    public float skillTime { get; set; }
    /// <summary>
    /// 是否正在释放技能
    /// </summary>
    public bool isRunSkill { get; set; }
    /// <summary>
    /// 连招超时时间
    /// </summary>
    public float outOfTime { get; set; }
    /// <summary>
    /// 是否可以强制打断技能
    /// </summary>
    public bool isCanForceStop { get; set; }
    /// <summary>
    /// 强制打断次数
    /// </summary>
    public int forceTimes { get; set; }
    /// <summary>
    /// 默认强制打断次数
    /// </summary>
    public int defaultForceTimes { get; set; }
    /// <summary>
    /// 强制打断重置时间
    /// </summary>
    public float forceResetTime { get; set; }
    /// <summary>
    /// 技能类型
    /// </summary>
    public SkillType skillType { get; set; }
    /// <summary>
    /// 长按/蓄力时间
    /// </summary>
    public float holdingTime { get; set; }
    /// <summary>
    /// 进度条颜色
    /// </summary>
    public Color progressColor { get; set; }

    public SkillTree() { 
        skillType = SkillType.Click;
        progressColor = Color.white;
        next = new List<SkillTree>();
    }
}
