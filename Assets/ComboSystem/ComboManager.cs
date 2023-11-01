using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����״̬ö�� ����|ִ����|�ȴ�
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
/// ���м��״̬ö�� �ɹ�|ʧ��|����
/// </summary>
public enum ComboState
{
    Success,
    Fail,
    Wrong
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

        //��ʼ������ �˴���ʱ�ô���д��
        //ʵ����������
        //����A
        SkillTree skillA1 = new SkillTree();
        skillA1.key = KeyCode.A;
        skillA1.name = "����A1";
        skillA1.id = 1;
        skillA1.outOfTime = 0.5f;
        skillA1.skillTime = 0.5f;
        skillA1.progressColor = Color.red;

        SkillTree skillA2 = new SkillTree();
        skillA2.key = KeyCode.A;
        skillA2.name = "����A2";
        skillA2.id = 1;
        skillA2.outOfTime = 0.5f;
        skillA2.skillTime = 2f;
        skillA2.progressColor = new Color(0.8f, 0, 0, 1);
        skillA2.skillType = SkillType.Hold;
        skillA2.holdingTime = 2;

        SkillTree skillA3 = new SkillTree();
        skillA3.key = KeyCode.A;
        skillA3.name = "����A3-1";
        skillA3.id = 1;
        skillA3.outOfTime = 0.5f;
        skillA3.skillTime = 0.5f;
        skillA3.progressColor = new Color(0.6f, 0, 0, 1);

        SkillTree skillA4 = new SkillTree();
        skillA4.key = KeyCode.S;
        skillA4.name = "����A3-2";
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

        //����B
        SkillTree skillB1 = new SkillTree();
        skillB1.key = KeyCode.B;
        skillB1.name = "����B1";
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
        skillB2.name = "����B2";
        skillB2.id = 2;
        skillB2.outOfTime = 0.5f;
        skillB2.skillTime = 10f;
        skillB2.progressColor = new Color(0, 0.8f, 0, 1);

        skillB1.next.Add(skillB2);
        _comboScript.AddSkill(skillB1);

        Debug.Log("��ʼ�����");
    }

    /// <summary>
    /// ��õ�ǰ����
    /// </summary>
    /// <returns></returns>
    public SkillTree GetCurSkill()
    {
        return _comboScript.GetCurSkill();
    }

    /// <summary>
    /// Idle����
    /// </summary>
    /// <param name="callback"></param>
    public void OnIdle(Action callback)
    {
        _comboScript.OnIdle(callback);
    }

    /// <summary>
    /// Running����
    /// </summary>
    /// <param name="callback"></param>
    public void OnRunning(Action callback)
    {
        _comboScript.OnRunning(callback);
    }

    /// <summary>
    /// Holding����
    /// </summary>
    /// <param name="callback"></param>
    public void OnHolding(Action callback)
    {
        _comboScript.OnHolding(callback);
    }

    /// <summary>
    /// Charging����
    /// </summary>
    /// <param name="callback"></param>
    public void OnCharging(Action callback)
    {
        _comboScript.OnCharging(callback);
    }

    /// <summary>
    /// Waiting����
    /// </summary>
    /// <param name="callback"></param>
    public void OnWaiting(Action callback)
    {
        _comboScript.OnWaiting(callback);
    }
}
