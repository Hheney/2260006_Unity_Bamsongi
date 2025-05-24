using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections; //List ����� ���� using


/// <summary>���� ���� ������ �����ϰ� �ϳ��� �������� Ȱ��ȭ��Ű�� ���� �Ŵ��� Ŭ����</summary>
public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance { get; private set; } //�̱��� ������ ���� �ν��Ͻ�
    /*
     * ����� Ÿ���� �������̹Ƿ� List�� �־ �����ϵ��� ��
     * [SerializeField] private ������� ����Ͽ� Inspector���� �߰��ϵ� ĸ��ȭ ����
     */
    [SerializeField] private List<TargetController> listTargets = new List<TargetController>();
    [SerializeField] private float fInterval = 3.0f;    //������ ����� ������ ����
    [SerializeField] private float fMinInterval = 0.3f; //������ ����� ������ �ּ� ����
    [SerializeField] private float fDecreaseRate = 0.5f;//������ ����� ������ ���� ������
    [SerializeField] private float fSpeedGrowthRate = 0.1f;//������ �ӵ� ������

    //Ȱ��ȭ�� ������ Box Collider Ȱ��ȭ ���¸� �����ϱ� ����
    private TargetController currentActiveTarget = null;
    public TargetController ActiveTarget {  get { return currentActiveTarget; } } //Ȱ��ȭ�� ���� ������Ƽ
    bool isPaused = false; //ī�޶� Blend���϶� ���� Coroutine�� �Ͻ��ߴ��ϱ� ���� bool �ʵ�

    /*
     * ������ ������ ������ ���� Index���� ����Ǹ� ��õ� �ϱ����� �ʵ�
     * -1�� ������ ����� ��ġ�� List �ڷ����� ����ϹǷ� Index�� 0���� �����ϱ⿡ �ʱ�ȭ�� 0�� �ƴ϶� -1�̿��� ��
     */
    int nPreviousIndex = -1;

    private float currentInterval;
    private float currentSpeedMultiplier;

    private void Awake()
    {
        //�̱��� ���� ����
        if(Instance == null)
        {
            Instance = this; //this : ���� �ν��Ͻ��� ����Ű�� ���۷���
        }
        else
        {
            Debug.LogWarning("TargetManager has another instance.");
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentInterval = fInterval;    //�ʱ� ���� ����
        currentSpeedMultiplier = 1.0f;  //�ʱ� �ӵ� ���� ����
        StartCoroutine(f_ActiveRandomTargetRoutine()); //���� Ÿ�� ��ƾ Ȱ��ȭ
    }

    //ĸ��ȭ ������ ���� �Ͻ����� �ܺ� ���� ���� �޼ҵ�

    /// <summary>�������� ������ ����Ű�� ��ƾ �Ͻ����� �޼ҵ�</summary>
    public void f_PauseTargetRoutine() //�Ͻ�����
    {
        isPaused = true;
    }

    /// <summary>�������� ������ ����Ű�� ��ƾ �簳 �޼ҵ�</summary>
    public void f_ResumeTargetRoutine() //��ƾ �簳
    {
        isPaused = false;
    }

    /// <summary>���� �ð����� ���� �ϳ��� �������� ����Ű�� ��ƾ</summary>
    private IEnumerator f_ActiveRandomTargetRoutine()
    {
        int nNewIndex = 0; //�� ��÷ Index��ȣ ���� ����
        
        while (true)
        {
            while(isPaused) //�Ͻ������� true���� ��� ��ƾ ���
            {
                yield return null;
            }

            if(currentActiveTarget != null)
            {
                currentActiveTarget.f_LieDownTarget(); //���� ������
            }

            do
            {
                nNewIndex = Random.Range(0, listTargets.Count);
            } while (nNewIndex == nPreviousIndex && listTargets.Count > 1); //�ߺ� ��÷ ����

            nPreviousIndex = nNewIndex;

            //--------------------------------[�ߺ���÷ ���� ���]--------------------------------
            //nNewIndex = nPreviousIndex; //���� �� ����

            ////�� �ε��� ���� ���� ���� ������ while �ݺ�
            //while (nNewIndex == nPreviousIndex && listTargets.Count > 1) //listTargets.Count > 1 : ������ 1���� ��� ���ѷ��� �߻� ����
            //{
            //    nNewIndex = Random.Range(0, listTargets.Count); //0���� ����Ʈ ����(���� ��)��ŭ�� �������� ������ ����
            //}

            //nPreviousIndex = nNewIndex; //���ο� �� ����

            //�����۵��� ��Ұ� �����Ѵٸ� Range ��� ������ĺ��� ����ġ ��� ������� �����Ͽ�, Ư�� ���ǿ��� Ư�� ������ ���� Ȯ���� ������ �� �ֵ��� Ȯ�� ����
            //--------------------------------[�ߺ���÷ ���� ���]--------------------------------

            Debug.Log($"{nNewIndex + 1}�� Ÿ��");
            Debug.Log($"����: {currentInterval:F2} / ȸ�� ����: {currentSpeedMultiplier:F2}");

            currentActiveTarget = listTargets[nNewIndex];
            currentActiveTarget.f_StandUpTarget(currentSpeedMultiplier); //����� �ε������� ���� Ÿ�� �����

            yield return new WaitForSeconds(currentInterval); //����� ������ ����(Interval)��ŭ ���

            currentInterval = Mathf.Max(fMinInterval, currentInterval - fDecreaseRate); //���� ����, �ּ� ����(fMinInterval)���� �۾����� �ʵ��� ����
            currentSpeedMultiplier += fSpeedGrowthRate;
        }
    }

    /// <summary>
    /// ���� Ȱ��ȭ�� ������ ��ȯ�ϴ� �޼ҵ�
    /// </summary>
    /// <returns>���� ������ ���� ������Ʈ</returns>
    public GameObject f_GetCurrentTarget()
    {
        //���׿����� : null�� �ƴҰ�� currentTarget.gameObject ��ȯ null�� ��� null ��ȯ
        return currentActiveTarget != null ? currentActiveTarget.gameObject : null;
    }
}
