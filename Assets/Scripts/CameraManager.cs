using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance = null;

    [SerializeField] private CinemachineCamera camDefault = null;       //�⺻ ī�޶�
    [SerializeField] private CinemachineCamera camZoomTarget = null;    //�� ī�޶�
    [SerializeField] private CinemachineCamera camTrajectory = null;    //���� ī�޶�

    [SerializeField] private Transform transDynamicTargetPoint = null;

    const int nDefaultPriority = 10;  //�⺻ �켱����
    const int nActivePriority = 20;   //Ȱ��ȭ �켱����
    const float fWaitTime = 2.0f;     //ī�޶� ��ȯ�� ���ð�

    //------------------------[�߻� ���� ��� ����]------------------------
    [SerializeField] private CinemachineBrain cinemachineBrain = null; //Blend ���θ� Ȯ���ϱ� ���� CinemachineBrain ������Ʈ�� �ҷ����� ����

    //��������Ʈ(delegate): ���� �Ϸ� �� ȣ��� �޼ҵ� ���� ����
    public delegate void CameraBlendCompleteDelegate();
    
    //�̺�Ʈ(event): �ܺο��� ������ �� �ִ� �̺�Ʈ
    public event CameraBlendCompleteDelegate OnCameraBlendComplete;

    /// <summary>���� ī�޶� ���� ���°� �ƴ��� ���θ� ��ȯ�ϴ� ������Ƽ</summary>
    public bool IsCameraReady
    {
        //�⺻ ī�޶� Ȱ��ȭ ���� && cinemachineBrain�� null�� �ƴ� && Blend ���°� �ƴ� ��� IsCameraReady "true" �� ����
        get { return camDefault.Priority == 20 && cinemachineBrain != null && !cinemachineBrain.IsBlending; }
    }
    //------------------------[�߻� ���� ��� ����]------------------------

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
        GameObject gCamera = GameObject.Find("Main Camera"); //���� ī�޶� ������Ʈ �ҷ�����

        if(gCamera != null)
        {
            cinemachineBrain = gCamera.GetComponent<CinemachineBrain>(); //CinemachineBrain ��� ��������
        }
        else
        {
            Debug.LogWarning("Main Camera ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        /*
         * LateUpdate()���� �޼ҵ带 ȣ���ؾ� �ϴ� ����
         * Unity�� �����ֱ⿡ ���� 1.FixedUpdate(), 2.Update(), 3.LateUpdate() ������ ȣ���
         * Update���� ��ġ �̵�, ��ġ ������ ���ÿ� �����ϸ� ī�޶� ���������Ʈ(�̵� ����)�� �� ������ �ʰ� ���󰡰� ��
         * �̷� ��쿡 �÷��̾� �������� ī�޶� ���� ������ �� ó�� ���̴� ������ �߻��Ѵ�
         * ���� �����ֱ⿡ ���� Update() �ڿ� ȣ��Ǵ� LateUpdate()���� �����ϵ��� �ϸ� ������ �ذ�ȴ�.
         */
        f_UpdateDynamicTrackingTarget();
    }

    /// <summary> ���� Ȱ��ȭ�� ���� �� ��ġ�� �̵���Ű�� ī�޶� �����ϵ��� �����ϴ� �޼ҵ� </summary>
    private void f_UpdateDynamicTrackingTarget()
    {
        if (TargetManager.Instance == null || TargetManager.Instance.ActiveTarget == null)
        {
            return;
        }

        Transform tActiveTarget = TargetManager.Instance.ActiveTarget.transform;

        Vector3 vForwardOffset = tActiveTarget.forward.normalized * 2.5f; //Ȱ��ȭ�� ���� ���� ���� ���� ��ġ ���

        //x��ǥ�� ���� �״��, z��ǥ���� ���� 2.5f��ŭ �տ� ��ġ�ϴ� ���Ͱ� ����
        Vector3 vAdjustedPosition = new Vector3(
            tActiveTarget.position.x,                         //��Ȯ�� ������ x�� ��ġ
            tActiveTarget.position.y + 1.0f,                  //�ణ ���ʿ��� �����ٺ���
            tActiveTarget.position.z + vForwardOffset.z       //���� ���� ����
        );

        //�̵� �� ȸ��
        transDynamicTargetPoint.position = vAdjustedPosition;
        transDynamicTargetPoint.LookAt(tActiveTarget);

        if (camZoomTarget.Follow != transDynamicTargetPoint)
        {
            camZoomTarget.Follow = transDynamicTargetPoint;
        }

        if (camZoomTarget.LookAt != transDynamicTargetPoint)
        {
            camZoomTarget.LookAt = transDynamicTargetPoint;
        }
        
    }


    /// <summary>ī�޶� ���� �������� �ڷ�ƾ���� �����ϴ� �޼ҵ�</summary>
    public void f_MoveCameraRoutine()
    {
        //Coroutine : �Ͻ� �ߴ��� ������ �޼ҵ�, ���� �����ӿ� ���� �۾��� ������ ó���� �� �ֵ��� ���ִ� ����� ������
        StartCoroutine(CameraMoveSequence()); //CameraMoveSequence ����
    }

    /// <summary>ī�޶� ������ ���� �� �̺�Ʈ�� ��������� ȣ��</summary>
    private IEnumerator CameraMoveSequence()
    {
        /*
         * IEnumerator : C#�� �������̽���, �ݺ� ������ ����(Enumerable)�� ��ȯ
         * �⺻������ Unity�� yield �� ������ �����ӿ� �ڷ�ƾ�� �ٽ� �����Ѵ�
         */
        f_SetCameraPriority(camZoomTarget); //�� ī�޶� Ȱ��ȭ
        yield return new WaitForSeconds(fWaitTime);

        f_SetCameraPriority(camTrajectory); //���� ī�޶� Ȱ��ȭ
        yield return new WaitForSeconds(fWaitTime);

        f_SetCameraPriority(camDefault); //�⺻ ī�޶� Ȱ��ȭ

        yield return new WaitUntil(() => IsCameraReady); //Blend�� �Ϸ�ɶ����� ��ƾ ���
        yield return new WaitForSeconds(0.5f); //blend�� �Ϸ�Ǿ����� ī�޶� ���󺹱ͱ��� 0.5�ʰ� �߰��� �ʿ���

        //event�� �ƹ��� �������� ������ null ������, null üũ�� ���� �ʰ� Invoke() �ϸ� ���ܰ� �߻���
        if (OnCameraBlendComplete != null) //���� null üũ�� ������
        {
            OnCameraBlendComplete.Invoke(); //�ܺ� �����ڿ��� Blend �ϷḦ �˸�
        }
    }

    /// <summary>��� ī�޶��� �켱������ �����ϰ� �־��� ī�޶� Ȱ��ȭ�ϴ� �޼ҵ�</summary>
    private void f_SetCameraPriority(CinemachineCamera camera)
    {
        camDefault.Priority = nDefaultPriority;       //�⺻ ī�޶� �켱���� �⺻���·� ����
        camZoomTarget.Priority = nDefaultPriority;    //�� ī�޶� �켱���� �⺻���·� ����
        camTrajectory.Priority = nDefaultPriority;    //���� ī�޶� �켱���� �⺻���·� ����

        camera.Priority = nActivePriority;   //�Ű����� ī�޶� �ֿ켱���� ���� �� Ȱ��ȭ
    }
}
