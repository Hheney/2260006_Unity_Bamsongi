using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

/*
 * [���ǻ���]
 * TargetManager ����, GameManager �Ǵ� InitScene ���� �ʱ�ȭ �ʼ�
 * GameManager �� TargetManager.Instance.f_Init();
 */

/// <summary> ���� ���� ������ �����ϰ� �ϳ��� �������� Ȱ��ȭ��Ű�� ���� �Ŵ��� Ŭ���� </summary>
public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //���� �Ŵ��� ����
        }
        else if (Instance != this)
        {
            Debug.Log("TargetManager has another instance.");
            Destroy(gameObject);
        }
    }

    //����ϴ� ������ �ټ��̹Ƿ� List�� ����Ͽ� ����
    private List<TargetController> listTargets = new List<TargetController>();

    //Ȱ��ȭ�� ������ Box Collider Ȱ��ȭ ���¸� �����ϱ� ����(�浹�� ���� ���� ��ȿȭ)
    private TargetController currentActiveTarget = null; 

    private bool isPaused = false;          //ī�޶� Blend���϶� ���� Coroutine�� �Ͻ��ߴ��ϱ� ���� bool �ʵ�
    private bool isRoutineRunning = false;  //��ƾ �ߺ� ���� ���� �÷���
    private Coroutine routineHandle = null; //��ƾ �ڵ� ����

    /*
     * ������ ������ ������ ���� Index���� ����Ǹ� ��õ� �ϱ����� �ʵ�
     * -1�� ������ ����� ��ġ�� List �ڷ����� ����ϹǷ� Index�� 0���� �����ϱ⿡ �ʱ�ȭ�� 0�� �ƴ϶� -1�̿��� ��
     */
    private int nPreviousIndex = -1;
    private const float fInterval = 3.0f; //������ ����� ������ ����
    public TargetController ActiveTarget {  get { return currentActiveTarget; } } //Ȱ��ȭ�� ���� Read-Only ������Ƽ


    /// <summary> TargetController(TargetPrefab�� ������Ʈ)�� ��Ÿ�ӿ��� �����ϰ� ��ƾ�� �����ϴ� �ʱ�ȭ �޼��� </summary>
    public void f_Init()
    {
        listTargets.Clear(); //�ߺ� ���� �ʱ�ȭ
        TargetController[] foundTargets = Object.FindObjectsByType<TargetController>(FindObjectsSortMode.None); //TargetController ã��
        //#pragma warning disable CS0618
        //TargetController[] foundTargets = Object.FindObjectsOfType<TargetController>(true);
        //#pragma warning restore CS0618

        listTargets.AddRange(foundTargets); //����Ʈ�� ã�� ���� �߰�
        listTargets = listTargets.OrderBy(Target => Target.name).ToList(); //������Ʈ �̸��� �������� ����

        if (listTargets.Count == 0)
        {
            Debug.LogWarning("TargetManager: Ȱ��ȭ�� TargetController�� �����ϴ�.");
            return;
        }

        if (!isRoutineRunning)
        {
            routineHandle = StartCoroutine(f_ActiveRandomTargetRoutine()); //���� Ÿ�� ��ƾ Ȱ��ȭ �� ��ƾ �ڵ� ����
        }
    }

    /*
     * ���� �������� Retry ��Ȳ���� GameScene�� �ٽ� �ε��� ��� TargetManager�� DontDestroyOnLoad�� ���Ͽ� �ı����� �ʾ�����,
     * �������� TargetController ������Ʈ�� �� ��ȯ���� �ı��ǰ� TargetManager�� f_ActiveRandomTargetRoutine() ��ƾ�� ��� �������� ���·�
     * ������ ������ �߻��Ͽ� MissingReferenceException ������ �߻���
     * ����, �� ��ȯ ���� f_ActiveRandomTargetRoutine() ��ƾ�� �����ϴ� �Ʒ� �޼ҵ带 �߰��Ͽ� �ذ���
     */
    public void f_Reset()
    {
        if (isRoutineRunning && routineHandle != null)
        {
            StopCoroutine(routineHandle);
        }
        routineHandle = null;
        isRoutineRunning = false;
        currentActiveTarget = null;
        listTargets.Clear();
    }

    public void f_StopTargetRoutine()
    {
        if (isRoutineRunning && routineHandle != null)
        {
            StopCoroutine(routineHandle);
            routineHandle = null;
            isRoutineRunning = false;
        }
    }

    public void f_PauseTargetRoutine() => isPaused = true;
    public void f_ResumeTargetRoutine() => isPaused = false;

    /// <summary>���� �ð����� ���� �ϳ��� �������� ����Ű�� ��ƾ</summary>
    private IEnumerator f_ActiveRandomTargetRoutine()
    {
        isRoutineRunning = true; //��ƾ�� ���������� �˸��� �÷��� Ȱ��ȭ
        int nNewIndex = 0;       //�� ��÷ Index��ȣ ���� ����

        while (true)
        {
            while (isPaused) //�Ͻ������� true���� ��� ��ƾ ���
            {
                yield return null;
            }

            if (currentActiveTarget != null)
            {
                currentActiveTarget.f_LieDownTarget(); //���� ������
            }

            nNewIndex = nPreviousIndex; //���� �� ����

            //�� �ε��� ���� ���� ���� ������ while �ݺ�
            while (nNewIndex == nPreviousIndex && listTargets.Count > 1) //listTargets.Count > 1 : ������ 1���� ��� ���ѷ��� �߻� ����
            {
                nNewIndex = Random.Range(0, listTargets.Count); //0���� ����Ʈ ����(���� ��)��ŭ�� �������� ������ ����
            }

            nPreviousIndex = nNewIndex; //���ο� �� ����
            currentActiveTarget = listTargets[nNewIndex];
            //�����۵��� ��Ұ� �����Ѵٸ� Range ��� ������ĺ��� ����ġ ��� ������� �����Ͽ�, Ư�� ���ǿ��� Ư�� ������ ���� Ȯ���� ������ �� �ֵ��� Ȯ�� ����

            if (currentActiveTarget != null)
            {
                currentActiveTarget.f_StandUpTarget(); //����� �ε������� ���� Ÿ�� �����
            }

            yield return new WaitForSeconds(fInterval); //����� ������ ����(Interval)��ŭ ���
        }
    }

    /// <summary> ���� Ȱ��ȭ�� ������ GameObject ��ȯ�ϴ� �޼ҵ� </summary>
    public GameObject f_GetCurrentTarget()
    {
        //���׿����� : null�� �ƴҰ�� currentTarget.gameObject ��ȯ null�� ��� null ��ȯ
        return currentActiveTarget != null ? currentActiveTarget.gameObject : null;
    }

    /// <summary> ���� Ȱ��ȭ�� ������ �ε��� ��ȯ�ϴ� �޼ҵ� </summary>
    public int f_GetActiveTargetIndex()
    {
        return listTargets.IndexOf(currentActiveTarget);
    }
}
