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

    public void f_MoveCameraRoutine()
    {
        StartCoroutine(CameraMoveSequence());
    }

    private IEnumerator CameraMoveSequence()
    {
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
