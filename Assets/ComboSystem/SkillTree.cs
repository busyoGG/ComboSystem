using System.Collections.Generic;
using UnityEngine;

public class SkillTree
{
    /// <summary>
    /// ��������
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// ����id һϵ�м���ʹ��ͬһid
    /// </summary>
    public int id { get; set; }
    /// <summary>
    /// ��λ����
    /// </summary>
    public KeyCode key { get; set; }
    /// <summary>
    /// ��һ�����б�
    /// </summary>
    public List<SkillTree> next { get; set; }
    /// <summary>
    /// ���ܳ���ʱ��
    /// </summary>
    public float skillTime { get; set; }
    /// <summary>
    /// �Ƿ������ͷż���
    /// </summary>
    public bool isRunSkill { get; set; }
    /// <summary>
    /// ���г�ʱʱ��
    /// </summary>
    public float outOfTime { get; set; }
    /// <summary>
    /// �Ƿ����ǿ�ƴ�ϼ���
    /// </summary>
    public bool isCanForceStop { get; set; }
    /// <summary>
    /// ǿ�ƴ�ϴ���
    /// </summary>
    public int forceTimes { get; set; }
    /// <summary>
    /// Ĭ��ǿ�ƴ�ϴ���
    /// </summary>
    public int defaultForceTimes { get; set; }
    /// <summary>
    /// ǿ�ƴ������ʱ��
    /// </summary>
    public float forceResetTime { get; set; }
    /// <summary>
    /// ��������
    /// </summary>
    public SkillType skillType { get; set; }
    /// <summary>
    /// ����/����ʱ��
    /// </summary>
    public float holdingTime { get; set; }
    /// <summary>
    /// ��������ɫ
    /// </summary>
    public Color progressColor { get; set; }

    public SkillTree() { 
        skillType = SkillType.Click;
        progressColor = Color.white;
        next = new List<SkillTree>();
    }
}
