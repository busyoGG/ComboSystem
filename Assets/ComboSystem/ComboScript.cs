using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    /// <summary>
    /// ����ʱ��
    /// </summary>
    private float _holdingTime = 0;

    private Dictionary<int, bool> _forceReset = new Dictionary<int, bool>();

    private Coroutine _skillCo;


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
        _skills.Add(skillB1);
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
                    DoChangeState(CheckSkillType(_curSkill.skillType));
                }
                break;
            case State.Running:
                //�ȴ�����ִ��
                if (!_curSkill.isRunSkill)
                {
                    Debug.Log("ִ�м��� === " + _curSkill.name + ":" + _curSkill.skillTime);
                    _curSkill.isRunSkill = true;
                    _skillCo = StartCoroutine(WaitForSkill());
                }
                _skillTime += Time.deltaTime;
                //���ǿ�ƴ��
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
                    Debug.Log("��ʱ��û�г���");
                    DoChangeState(State.Waiting);
                    _holdingTime = 0;
                    _skillTime = _curSkill.skillTime;
                }
                else
                {
                    Debug.Log("ִ�г������� === " + _curSkill.name);
                }
                break;
            case State.Charging:
                _holdingTime += Time.deltaTime;
                _skillTime += Time.deltaTime;
                if (Input.anyKey)
                {
                    if (_holdingTime > _curSkill.holdingTime)
                    {
                        Debug.Log("�������");
                        DoChangeState(State.Running);
                        _holdingTime = 0;
                    }
                    else
                    {
                        Debug.Log("���� === " + _curSkill.name);
                    }
                }
                else
                {
                    Debug.Log("ֹͣ����");
                    DoChangeState(State.Idle);
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
                        DoChangeState(CheckSkillType(_curSkill.skillType));
                        Debug.Log("���гɹ�");
                    }
                    else if (res == ComboState.Wrong)
                    {
                        //���д��� �˴�ѡ������ִ�е�ǰ����ָ�� Ҳ���Բ�ִ�У�����һ֡���ж�
                        if (CheckSkillKey(true) == ComboState.Success)
                        {
                            //������У�ֱ��תΪ�������
                            DoChangeState(CheckSkillType(_curSkill.skillType));
                            Debug.Log("ת������");
                        }
                        else
                        {
                            //û�б������ ״̬����
                            DoChangeState(State.Idle);
                            Debug.Log("û��������");
                        }
                    }
                }
                else
                {
                    //��ʱ����
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
    /// ִ��״̬
    /// </summary>
    private void DoStatus()
    {
        switch (_state)
        {
            case State.Idle:
                //RunIdle
                Debug.Log("ִ�� Idle");
                break;
            case State.Running:
                //RunSkill
                Debug.Log("ִ�� Running");
                break;
            case State.Holding:
                //HoldingSkill
                Debug.Log("ִ�� Holding");
                break;
            case State.Charging:
                //HoldingSkill
                Debug.Log("ִ�� Charging");
                break;
            case State.Waiting:
                //RunWaiting
                Debug.Log("ִ�� Waiting");
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
        Debug.Log("���ü�����");
    }

    /// <summary>
    /// ��������Ƿ�ɹ�
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
                    //����������ܣ����ҴӼ�������ʽ��ʼ��� �����Ϊ��ϼ��
                    for (int i = 0, len = _skills.Count; i < len; i++)
                    {
                        SkillTree skill = _skills[i];
                        if (skill.isCanForceStop && skill.forceTimes > 0 && Input.GetKeyDown(skill.key))
                        {
                            Debug.Log("��ϲ�ǿ�ƿ�ʼ����" + skill.key);

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
                //������з�֧
                if (Input.anyKeyDown)
                {
                    for (int i = 0, len = _curSkill.next.Count; i < len; i++)
                    {
                        SkillTree skill = _curSkill.next[i];
                        if (Input.GetKeyDown(skill.key))
                        {
                            //���гɹ�
                            _curSkill = _curSkill.next[i];
                            return ComboState.Success;
                        }
                    }

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
            DoChangeState(State.Waiting);
            _curSkill.isRunSkill = false;
        }
    }

    private IEnumerator WaitForForceTimesReset(SkillTree obj)
    {
        Debug.Log("��ʼ����ǿ�ƴ�ϴ��� ��" + obj.forceResetTime);
        yield return new WaitForSeconds(obj.forceResetTime);
        obj.forceTimes = obj.defaultForceTimes;
        _forceReset[obj.id] = false;
        Debug.Log("����ǿ�ƴ�ϴ���");
    }
}
