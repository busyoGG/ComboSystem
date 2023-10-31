using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboScript : MonoBehaviour
{
    /// <summary>
    /// 连招状态枚举 空闲|执行中|等待
    /// </summary>
    private enum State
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
    private enum ComboState
    {
        Success,
        Fail,
        Wrong
    }
    /// <summary>
    /// 连招状态
    /// </summary>
    private State _state;
    /// <summary>
    /// 技能列表
    /// </summary>
    private List<SkillTree> _skills = new List<SkillTree>();
    /// <summary>
    /// 当前技能
    /// </summary>
    private SkillTree _curSkill;
    /// <summary>
    /// 输入间隔
    /// </summary>
    private float _inputDelta = 0;
    /// <summary>
    /// 输入间隔进度条控件
    /// </summary>
    private RectTransform _skillProgress = null;
    /// <summary>
    /// 输入间隔进度条图像
    /// </summary>
    private Image _skillImage;
    /// <summary>
    /// 技能进度条控件
    /// </summary>
    private RectTransform _runProgress = null;
    /// <summary>
    /// 技能进度条图像
    /// </summary>
    private Image _runImage;
    /// <summary>
    /// 技能执行时间
    /// </summary>
    private float _skillTime = 0;

    void Start()
    {
        //获取UI
        _skillProgress = GameObject.FindGameObjectWithTag("Player").GetComponent<RectTransform>();
        _skillProgress.sizeDelta = new Vector2(0, 30);
        _skillImage = GameObject.FindGameObjectWithTag("Player").GetComponent<Image>();

        _runProgress = GameObject.FindGameObjectWithTag("Finish").GetComponent<RectTransform>();
        _runProgress.sizeDelta = new Vector2(0, 30);
        _runImage = GameObject.FindGameObjectWithTag("Finish").GetComponent<Image>();
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
        skillA2.skillTime = 0.5f;
        skillA2.progressColor = new Color(0.8f, 0, 0, 1);

        SkillTree skillA3 = new SkillTree();
        skillA3.key = KeyCode.A;
        skillA3.name = "连招A3";
        skillA3.id = 1;
        skillA3.outOfTime = 0.5f;
        skillA3.skillTime = 0.5f;
        skillA3.progressColor = new Color(0.6f, 0, 0, 1);

        skillA1.next = skillA2;
        skillA2.next = skillA3;

        _skills.Add(skillA1);

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
        skillB1.progressColor = Color.green;

        SkillTree skillB2 = new SkillTree();
        skillB2.key = KeyCode.B;
        skillB2.name = "连招B2";
        skillB2.id = 2;
        skillB2.outOfTime = 0.5f;
        skillB2.skillTime = 0.5f;
        skillB2.progressColor = new Color(0, 0.8f, 0, 1);

        skillB1.next = skillB2;
        _skills.Add(skillB1);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeState();
        DoStatus();
    }

    private void OnGUI()
    {
        if (_curSkill != null)
        {
            float ratio = (_curSkill.outOfTime - _inputDelta) / _curSkill.outOfTime;
            if (ratio < 0)
            {
                ratio = 0;
            }
            _skillProgress.sizeDelta = new Vector2(ratio * 100, 30);
            _skillImage.color = _curSkill.progressColor;


            if (_curSkill.skillTime - _skillTime >= 0)
            {
                float skillRatio = (_curSkill.skillTime - _skillTime) / _curSkill.skillTime;
                _runProgress.sizeDelta = new Vector2(skillRatio * 100, 30);
                _runImage.color = _curSkill.progressColor;
            }
            else
            {
                _runProgress.sizeDelta = new Vector2(0, 30);
            }
        }
        else
        {
            if (_skillProgress.sizeDelta.x != 0)
            {
                _skillProgress.sizeDelta = new Vector2(0, 30);
            }

            if (_runProgress.sizeDelta.x != 0)
            {
                _runProgress.sizeDelta = new Vector2(0, 30);
            }
        }
    }

    /// <summary>
    /// 改变状态
    /// </summary>
    private void ChangeState()
    {
        switch (_state)
        {
            case State.Idle:
                _curSkill = null;
                //起始技能检测
                if (CheckSkillKey(true) == ComboState.Success)
                {
                    _state = CheckSkillType(_curSkill.skillType);
                }
                break;
            case State.Running:
                //等待技能执行
                if (!_curSkill.isRunSkill)
                {
                    _skillTime = 0;
                    Debug.Log("执行技能 === " + _curSkill.name);
                    _curSkill.isRunSkill = true;
                    StartCoroutine(WaitForSkill());
                }
                _skillTime += Time.deltaTime;
                //检测强制打断
                if (CheckSkillKey(true) == ComboState.Success)
                {
                    _inputDelta = 0;
                }
                break;
            case State.Waiting:
                //等待中增加间隔时间
                _inputDelta += Time.deltaTime;
                //连击时间小于技能最大间隔时间
                if (_inputDelta <= _curSkill.outOfTime)
                {
                    //连击时间内检测
                    ComboState res = CheckSkillKey(false);
                    if (res == ComboState.Success)
                    {
                        //连招成功
                        _state = CheckSkillType(_curSkill.skillType);
                        _inputDelta = 0;
                    }
                    else if (res == ComboState.Wrong)
                    {
                        //连招错误 此处选择立即执行当前连招指令 也可以不执行，放下一帧再判断
                        if (CheckSkillKey(true) == ComboState.Success)
                        {
                            //如果有招，直接转为别的连招
                            _state = CheckSkillType(_curSkill.skillType);
                            Debug.Log("转换连招");
                        }
                        else
                        {
                            //没有别的连招 状态重置
                            _state = State.Idle;
                            Debug.Log("没有新连招");
                        }
                        _inputDelta = 0;
                    }
                }
                else
                {
                    //超时重置
                    _state = State.Idle;
                    _inputDelta = 0;
                }
                break;
        }
    }

    private State CheckSkillType(SkillType type)
    {
        switch (type)
        {
            case SkillType.Click:
                return State.Running;
            case SkillType.Hold:
                return State.Holding;
            case SkillType.Charge:
                return State.Charging;
        }
        return State.Running;
    }

    /// <summary>
    /// 执行状态
    /// </summary>
    private void DoStatus()
    {
        switch (_state)
        {
            case State.Idle:
                //RunIdle
                break;
            case State.Running:
                //RunSkill
                break;
            case State.Waiting:
                //RunWaiting
                break;
        }
    }

    /// <summary>
    /// 检测连招是否成功
    /// </summary>
    /// <param name="isRoot"></param>
    /// <returns></returns>
    private ComboState CheckSkillKey(bool isRoot)
    {
        if (Input.anyKeyDown)
        {
            if (isRoot)
            {
                if (_curSkill != null)
                {
                    //存在最近技能，并且从技能起手式开始检测 该情况为打断检测
                    for (int i = 0, len = _skills.Count; i < len; i++)
                    {
                        SkillTree skill = _skills[i];
                        if (skill.isCanForceStop && skill.forceTimes > 0 && Input.GetKeyDown(skill.key))
                        {
                            Debug.Log("打断并强制开始连招" + skill.key);
                            StopCoroutine(WaitForSkill());
                            _curSkill.isRunSkill = false;
                            if (skill.id != _curSkill.id)
                            {
                                _curSkill.forceTimes = _curSkill.defaultForceTimes;
                            }
                            _curSkill = skill;
                            --_curSkill.forceTimes;
                            return ComboState.Success;
                        }
                    }
                }
                else
                {
                    //查找连招起手式
                    for (int i = 0, len = _skills.Count; i < len; i++)
                    {
                        SkillTree skill = _skills[i];
                        if (Input.GetKeyDown(skill.key))
                        {
                            Debug.Log("开始连招" + skill.key);
                            _curSkill = skill;
                            return ComboState.Success;
                        }
                    }
                }
            }
            else
            {
                if (_curSkill.next != null && Input.GetKeyDown(_curSkill.next.key))
                {
                    //连招成功
                    _curSkill = _curSkill.next;
                    return ComboState.Success;
                }
                else
                {
                    //连错招 重置
                    Debug.Log("连招重置");
                    return ComboState.Wrong;
                }
            }
        }
        return ComboState.Fail;
    }

    /// <summary>
    /// 模拟技能执行时间
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForSkill()
    {
        yield return new WaitForSeconds(_curSkill.skillTime);
        if (_curSkill != null)
        {
            _state = State.Waiting;
            _curSkill.isRunSkill = false;
            _curSkill.forceTimes = _curSkill.defaultForceTimes;
        }
    }
}
