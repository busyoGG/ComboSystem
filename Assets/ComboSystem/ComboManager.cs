using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 连招状态枚举 空闲|执行中|等待
/// </summary>
public enum State
{
    Idle,
    Running,
    Holding,
    Charging,
    Waiting
}
/// <summary>
/// 连招检测状态枚举 成功|失败|错误
/// </summary>
public enum ComboState
{
    Success,
    Fail,
    Wrong
}

/// <summary>
/// 技能类型
/// </summary>
public enum SkillType
{
    Click,
    Hold,
    Charge
}

public class ComboManager
{
    private static ComboManager _instance = null;

    private ComboScript _comboScript;

    public static ComboManager Instance()
    {
        if(_instance == null)
        {
            _instance = new ComboManager();
        }
        return _instance;
    }

    private ComboManager() { }

    public void Init()
    {
        GameObject root = new GameObject();
        root.name = "ComboSystem";
        _comboScript = root.AddComponent<ComboScript>();

        //初始化技能 此处暂时用代码写死
        //实例化技能链
        //技能A
        SkillTree skillA1 = new SkillTree();
        skillA1.key = KeyCode.A;
        skillA1.name = "连招A1";
        skillA1.id = 1;
        skillA1.outOfTime = 0.5f;
        skillA1.skillTime = 0.5f;
        skillA1.progressColor = Color.red;

        SkillTree skillA2 = new SkillTree();
        skillA2.key = KeyCode.A;
        skillA2.name = "连招A2";
        skillA2.id = 1;
        skillA2.outOfTime = 0.5f;
        skillA2.skillTime = 2f;
        skillA2.progressColor = new Color(0.8f, 0, 0, 1);
        skillA2.skillType = SkillType.Hold;
        skillA2.holdingTime = 2;

        SkillTree skillA3 = new SkillTree();
        skillA3.key = KeyCode.A;
        skillA3.name = "连招A3-1";
        skillA3.id = 1;
        skillA3.outOfTime = 0.5f;
        skillA3.skillTime = 0.5f;
        skillA3.progressColor = new Color(0.6f, 0, 0, 1);

        SkillTree skillA4 = new SkillTree();
        skillA4.key = KeyCode.S;
        skillA4.name = "连招A3-2";
        skillA4.id = 1;
        skillA4.outOfTime = 0.5f;
        skillA4.skillTime = 0.5f;
        skillA4.progressColor = new Color(0.4f, 0, 0, 1);
        skillA4.skillType = SkillType.Charge;
        skillA4.holdingTime = 2;

        skillA1.next.Add(skillA2);
        skillA2.next.Add(skillA3);
        skillA2.next.Add(skillA4);

        _comboScript.AddSkill(skillA1);

        //技能B
        SkillTree skillB1 = new SkillTree();
        skillB1.key = KeyCode.B;
        skillB1.name = "连招B1";
        skillB1.id = 2;
        skillB1.outOfTime = 0.5f;
        skillB1.skillTime = 0.5f;
        skillB1.isCanForceStop = true;
        skillB1.forceTimes = 1;
        skillB1.defaultForceTimes = 1;
        skillB1.forceResetTime = 5;
        skillB1.progressColor = Color.green;

        SkillTree skillB2 = new SkillTree();
        skillB2.key = KeyCode.B;
        skillB2.name = "连招B2";
        skillB2.id = 2;
        skillB2.outOfTime = 0.5f;
        skillB2.skillTime = 10f;
        skillB2.progressColor = new Color(0, 0.8f, 0, 1);

        skillB1.next.Add(skillB2);
        _comboScript.AddSkill(skillB1);

        Debug.Log("初始化完成");
    }

    /// <summary>
    /// 获得当前技能
    /// </summary>
    /// <returns></returns>
    public SkillTree GetCurSkill()
    {
        return _comboScript.GetCurSkill();
    }

    /// <summary>
    /// Idle函数
    /// </summary>
    /// <param name="callback"></param>
    public void OnIdle(Action callback)
    {
        _comboScript.OnIdle(callback);
    }

    /// <summary>
    /// Running函数
    /// </summary>
    /// <param name="callback"></param>
    public void OnRunning(Action callback)
    {
        _comboScript.OnRunning(callback);
    }

    /// <summary>
    /// Holding函数
    /// </summary>
    /// <param name="callback"></param>
    public void OnHolding(Action callback)
    {
        _comboScript.OnHolding(callback);
    }

    /// <summary>
    /// Charging函数
    /// </summary>
    /// <param name="callback"></param>
    public void OnCharging(Action callback)
    {
        _comboScript.OnCharging(callback);
    }

    /// <summary>
    /// Waiting函数
    /// </summary>
    /// <param name="callback"></param>
    public void OnWaiting(Action callback)
    {
        _comboScript.OnWaiting(callback);
    }
}
