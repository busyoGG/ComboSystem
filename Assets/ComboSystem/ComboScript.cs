using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class ComboScript : MonoBehaviour
{
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
    /// <summary>
    /// 长按时间
    /// </summary>
    private float _holdingTime = 0;

    private Dictionary<int, bool> _forceReset = new Dictionary<int, bool>();

    private Coroutine _skillCo;

    private Action _onIdle;

    private Action _onRunning;

    private Action _onHolding;

    private Action _onCharging;

    private Action _onWaiting;

    void Start()
    {
        //获取UI
        _skillProgress = GameObject.FindGameObjectWithTag("Player").GetComponent<RectTransform>();
        _skillProgress.sizeDelta = new Vector2(0, 30);
        _skillImage = GameObject.FindGameObjectWithTag("Player").GetComponent<Image>();

        _runProgress = GameObject.FindGameObjectWithTag("Finish").GetComponent<RectTransform>();
        _runProgress.sizeDelta = new Vector2(0, 30);
        _runImage = GameObject.FindGameObjectWithTag("Finish").GetComponent<Image>();

        DoChangeState(State.Idle);
    }

    // Update is called once per frame
    void Update()
    {
        ChangeState();
    }

    private void OnGUI()
    {
        if (_curSkill != null)
        {
            if (_inputDelta == 0)
            {
                if (_skillProgress.sizeDelta.x != 0)
                {
                    _skillProgress.sizeDelta = new Vector2(0, 30);
                }
            }
            else
            {
                float ratio = (_curSkill.outOfTime - _inputDelta) / _curSkill.outOfTime;
                if (ratio < 0)
                {
                    ratio = 0;
                }
                _skillProgress.sizeDelta = new Vector2(ratio * 100, 30);
                _skillImage.color = _curSkill.progressColor;
            }

            if (_skillTime == 0)
            {
                if (_runProgress.sizeDelta.x != 0)
                {
                    _runProgress.sizeDelta = new Vector2(0, 30);
                }
            }
            else
            {
                float total = _curSkill.skillTime;
                if (_curSkill.skillType == SkillType.Charge && _state == State.Charging)
                {
                    total = _curSkill.holdingTime;
                }
                if (total - _skillTime >= 0)
                {
                    float skillRatio = (total - _skillTime) / total;
                    _runProgress.sizeDelta = new Vector2(skillRatio * 100, 30);
                    _runImage.color = _curSkill.progressColor;
                }
                else
                {
                    _runProgress.sizeDelta = new Vector2(0, 30);
                }
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
                    DoChangeState(CheckSkillType(_curSkill.skillType));
                }
                break;
            case State.Running:
                //等待技能执行
                if (!_curSkill.isRunSkill)
                {
                    Debug.Log("执行技能 === " + _curSkill.name + ":" + _curSkill.skillTime);
                    _curSkill.isRunSkill = true;
                    _skillCo = StartCoroutine(WaitForSkill());
                }
                _skillTime += Time.deltaTime;
                //检测强制打断
                if (CheckSkillKey(true) == ComboState.Success)
                {
                    //_inputDelta = 0;
                    DoChangeState(State.Running);
                }
                break;
            case State.Holding:
                _holdingTime += Time.deltaTime;
                _skillTime += Time.deltaTime;
                if (_holdingTime > _curSkill.holdingTime || !Input.anyKey)
                {
                    Debug.Log("超时或没有长按");
                    DoChangeState(State.Waiting);
                    _holdingTime = 0;
                    _skillTime = _curSkill.skillTime;
                }
                else
                {
                    Debug.Log("执行长按技能 === " + _curSkill.name);
                }
                break;
            case State.Charging:
                _holdingTime += Time.deltaTime;
                _skillTime += Time.deltaTime;
                if (Input.anyKey)
                {
                    if (_holdingTime > _curSkill.holdingTime)
                    {
                        Debug.Log("蓄力完成");
                        DoChangeState(State.Running);
                        _holdingTime = 0;
                    }
                    else
                    {
                        Debug.Log("蓄力 === " + _curSkill.name);
                    }
                }
                else
                {
                    Debug.Log("停止蓄力");
                    DoChangeState(State.Idle);
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
                        DoChangeState(CheckSkillType(_curSkill.skillType));
                        Debug.Log("连招成功");
                    }
                    else if (res == ComboState.Wrong)
                    {
                        //连招错误 此处选择立即执行当前连招指令 也可以不执行，放下一帧再判断
                        if (CheckSkillKey(true) == ComboState.Success)
                        {
                            //如果有招，直接转为别的连招
                            DoChangeState(CheckSkillType(_curSkill.skillType));
                            Debug.Log("转换连招");
                        }
                        else
                        {
                            //没有别的连招 状态重置
                            DoChangeState(State.Idle);
                            Debug.Log("没有新连招");
                        }
                    }
                }
                else
                {
                    //超时重置
                    DoChangeState(State.Idle);
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
                if (_onIdle != null)
                {
                    _onIdle();
                }
                break;
            case State.Running:
                //RunSkill
                if (_onRunning != null)
                {
                    _onRunning();
                }
                break;
            case State.Holding:
                //HoldingSkill
                if (_onHolding != null)
                {
                    _onHolding();
                }
                break;
            case State.Charging:
                //HoldingSkill
                if (_onCharging != null)
                {
                    _onCharging();
                }
                break;
            case State.Waiting:
                //RunWaiting
                if (_onWaiting != null)
                {
                    _onWaiting();
                }
                break;
        }
    }

    private void DoChangeState(State state)
    {
        _state = state;
        _inputDelta = 0;
        _skillTime = 0;
        _holdingTime = 0;
        DoStatus();
        Debug.Log("重置计数器");
    }

    /// <summary>
    /// 检测连招是否成功
    /// </summary>
    /// <param name="isRoot"></param>
    /// <returns></returns>
    private ComboState CheckSkillKey(bool isRoot)
    {
        if (Input.anyKey)
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

                            StopCoroutine(_skillCo);

                            if (skill.name != _curSkill.name)
                            {
                                _curSkill.isRunSkill = false;
                                //_curSkill.forceTimes = _curSkill.defaultForceTimes;
                            }
                            _curSkill = skill;
                            --_curSkill.forceTimes;

                            bool reset;
                            _forceReset.TryGetValue(_curSkill.id, out reset);
                            if (!reset)
                            {
                                StartCoroutine(WaitForForceTimesReset(_curSkill));
                            }

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
                //检测连招分支
                if (Input.anyKeyDown)
                {
                    for (int i = 0, len = _curSkill.next.Count; i < len; i++)
                    {
                        SkillTree skill = _curSkill.next[i];
                        if (Input.GetKeyDown(skill.key))
                        {
                            //连招成功
                            _curSkill = _curSkill.next[i];
                            return ComboState.Success;
                        }
                    }

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
            DoChangeState(State.Waiting);
            _curSkill.isRunSkill = false;
        }
    }

    private IEnumerator WaitForForceTimesReset(SkillTree obj)
    {
        Debug.Log("开始重置强制打断次数 ：" + obj.forceResetTime);
        yield return new WaitForSeconds(obj.forceResetTime);
        obj.forceTimes = obj.defaultForceTimes;
        _forceReset[obj.id] = false;
        Debug.Log("重置强制打断次数");
    }

    /// <summary>
    /// 添加技能
    /// </summary>
    /// <param name="skill"></param>
    public void AddSkill(SkillTree skill)
    {
        _skills.Add(skill);
    }

    /// <summary>
    /// 获取当前技能
    /// </summary>
    /// <returns></returns>
    public SkillTree GetCurSkill()
    {
        return _curSkill;
    }

    public void OnIdle(Action callback)
    {
        _onIdle = callback;
    }

    public void OnRunning(Action callback)
    {
        _onRunning = callback;
    }

    public void OnHolding(Action callback)
    {
        _onHolding = callback;
    }

    public void OnCharging(Action callback)
    {
        _onCharging = callback;
    }

    public void OnWaiting(Action callback)
    {
        _onWaiting = callback;
    }
}
