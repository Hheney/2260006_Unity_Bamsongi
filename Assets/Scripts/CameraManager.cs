using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance = null;

    [SerializeField] private CinemachineCamera camDefault = null;       //�⺻ ī�޶�
    [SerializeField] private CinemachineCamera camZoomTarget = null;    //�� ī�޶�
    [SerializeField] private CinemachineCamera camTrajectory = null;    //���� ī�޶�

    float fWaitTime = 2.0f;

    public static CameraManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogWarning("CameraManager is null.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this; //this : ���� �ν��Ͻ��� ����Ű�� ���۷���
        }
        else if (_instance != this)
        {
            Debug.Log("CameraManager has another instance.");

            Destroy(gameObject); //���� �ν��Ͻ� �ı�(GameManger Object)
        }
        DontDestroyOnLoad(gameObject); //���� ����Ǿ ���� ���� ������Ʈ�� ������Ű�� �޼ҵ�
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Coroutine : �Ͻ� �ߴ��� ������ �޼ҵ�, ���� �����ӿ� ���� �۾��� ������ ó���� �� �ֵ��� ���ִ� ����� ������
    public void f_MoveCameraRoutine()
    {
        StartCoroutine(CameraMoveSequence()); //CameraMoveSequence ����
    }

    //IEnumerator : C#�� �������̽���, �ݺ� ������ ����(Enumerable)�� ��ȯ
    //�⺻������ Unity�� yield �� ������ �����ӿ� �ڷ�ƾ�� �ٽ� �����Ѵ�
    private IEnumerator CameraMoveSequence()
    {
        //Blend�� ������ ������ Ȯ������ ���ϸ� Priority�� ��
        //�ߺ� Ŭ���� Ŭ�������� ���ؼ� Blend ���� �Ǵ� �߰��ؾ���
        //GameManager������ �Ǵ��� ���� Bool �ʵ带 get set���� ����
        //BamsongiController Ŭ������ ���������� CanClick �޼ҵ带 �����Ͽ� bool������ ������ų �� �ֵ��� �ؾ���

        f_SetCameraPriority(camZoomTarget); //�� ī�޶� Ȱ��ȭ
        yield return new WaitForSeconds(fWaitTime);

        f_SetCameraPriority(camTrajectory); //���� ī�޶� Ȱ��ȭ
        yield return new WaitForSeconds(fWaitTime);

        f_SetCameraPriority(camDefault); //�⺻ ī�޶� Ȱ��ȭ
    }

    private void f_SetCameraPriority(CinemachineCamera camera)
    {
        camDefault.Priority = 10;       //�⺻ ī�޶� �켱���� �⺻���·� ����
        camZoomTarget.Priority = 10;    //�� ī�޶� �켱���� �⺻���·� ����
        camTrajectory.Priority = 10;    //���� ī�޶� �켱���� �⺻���·� ����

        camera.Priority = 20;   //�Ű����� ī�޶� �ֿ켱���� ���� �� Ȱ��ȭ
    }
}
