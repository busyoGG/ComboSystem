using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboScript : MonoBehaviour
{
    /// <summary>
    /// ����״̬ö�� ����|ִ����|�ȴ�
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
    /// ���м��״̬ö�� �ɹ�|ʧ��|����
    /// </summary>
    private enum ComboState
    {
        Success,
        Fail,
        Wrong
    }
    /// <summary>
    /// ����״̬
    /// </summary>
    private State _state;
    /// <summary>
    /// �����б�
    /// </summary>
    private List<SkillTree> _skills = new List<SkillTree>();
    /// <summary>
    /// ��ǰ����
    /// </summary>
    private SkillTree _curSkill;
    /// <summary>
    /// ������
    /// </summary>
    private float _inputDelta = 0;
    /// <summary>
    /// �������������ؼ�
    /// </summary>
    private RectTransform _skillProgress = null;
    /// <summary>
    /// ������������ͼ��
    /// </summary>
    private Image _skillImage;
    /// <summary>
    /// ���ܽ������ؼ�
    /// </summary>
    private RectTransform _runProgress = null;
    /// <summary>
    /// ���ܽ�����ͼ��
    /// </summary>
    private Image _runImage;
    /// <summary>
    /// ����ִ��ʱ��
    /// </summary>
    private float _skillTime = 0;

    void Start()
    {
        //��ȡUI
        _skillProgress = GameObject.FindGameObjectWithTag("Player").GetComponent<RectTransform>();
        _skillProgress.sizeDelta = new Vector2(0, 30);
        _skillImage = GameObject.FindGameObjectWithTag("Player").GetComponent<Image>();

        _runProgress = GameObject.FindGameObjectWithTag("Finish").GetComponent<RectTransform>();
        _runProgress.sizeDelta = new Vector2(0, 30);
        _runImage = GameObject.FindGameObjectWithTag("Finish").GetComponent<Image>();
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
        skillA2.skillTime = 0.5f;
        skillA2.progressColor = new Color(0.8f, 0, 0, 1);

        SkillTree skillA3 = new SkillTree();
        skillA3.key = KeyCode.A;
        skillA3.name = "����A3";
        skillA3.id = 1;
        skillA3.outOfTime = 0.5f;
        skillA3.skillTime = 0.5f;
        skillA3.progressColor = new Color(0.6f, 0, 0, 1);

        skillA1.next = skillA2;
        skillA2.next = skillA3;

        _skills.Add(skillA1);

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
        skillB1.progressColor = Color.green;

        SkillTree skillB2 = new SkillTree();
        skillB2.key = KeyCode.B;
        skillB2.name = "����B2";
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
    /// �ı�״̬
    /// </summary>
    private void ChangeState()
    {
        switch (_state)
        {
            case State.Idle:
                _curSkill = null;
                //��ʼ���ܼ��
                if (CheckSkillKey(true) == ComboState.Success)
                {
                    _state = CheckSkillType(_curSkill.skillType);
                }
                break;
            case State.Running:
                //�ȴ�����ִ��
                if (!_curSkill.isRunSkill)
                {
                    _skillTime = 0;
                    Debug.Log("ִ�м��� === " + _curSkill.name);
                    _curSkill.isRunSkill = true;
                    StartCoroutine(WaitForSkill());
                }
                _skillTime += Time.deltaTime;
                //���ǿ�ƴ��
                if (CheckSkillKey(true) == ComboState.Success)
                {
                    _inputDelta = 0;
                }
                break;
            case State.Waiting:
                //�ȴ������Ӽ��ʱ��
                _inputDelta += Time.deltaTime;
                //����ʱ��С�ڼ��������ʱ��
                if (_inputDelta <= _curSkill.outOfTime)
                {
                    //����ʱ���ڼ��
                    ComboState res = CheckSkillKey(false);
                    if (res == ComboState.Success)
                    {
                        //���гɹ�
                        _state = CheckSkillType(_curSkill.skillType);
                        _inputDelta = 0;
                    }
                    else if (res == ComboState.Wrong)
                    {
                        //���д��� �˴�ѡ������ִ�е�ǰ����ָ�� Ҳ���Բ�ִ�У�����һ֡���ж�
                        if (CheckSkillKey(true) == ComboState.Success)
                        {
                            //������У�ֱ��תΪ�������
                            _state = CheckSkillType(_curSkill.skillType);
                            Debug.Log("ת������");
                        }
                        else
                        {
                            //û�б������ ״̬����
                            _state = State.Idle;
                            Debug.Log("û��������");
                        }
                        _inputDelta = 0;
                    }
                }
                else
                {
                    //��ʱ����
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
    /// ִ��״̬
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
    /// ��������Ƿ�ɹ�
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
                    //����������ܣ����ҴӼ�������ʽ��ʼ��� �����Ϊ��ϼ��
                    for (int i = 0, len = _skills.Count; i < len; i++)
                    {
                        SkillTree skill = _skills[i];
                        if (skill.isCanForceStop && skill.forceTimes > 0 && Input.GetKeyDown(skill.key))
                        {
                            Debug.Log("��ϲ�ǿ�ƿ�ʼ����" + skill.key);
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
                    //������������ʽ
                    for (int i = 0, len = _skills.Count; i < len; i++)
                    {
                        SkillTree skill = _skills[i];
                        if (Input.GetKeyDown(skill.key))
                        {
                            Debug.Log("��ʼ����" + skill.key);
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
                    //���гɹ�
                    _curSkill = _curSkill.next;
                    return ComboState.Success;
                }
                else
                {
                    //������ ����
                    Debug.Log("��������");
                    return ComboState.Wrong;
                }
            }
        }
        return ComboState.Fail;
    }

    /// <summary>
    /// ģ�⼼��ִ��ʱ��
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
