using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

/*
 * [���ǻ���]
 * CameraManager ����, GameManager �Ǵ� InitScene ���� �ʱ�ȭ �ʼ�
 * GameManager �� CameraManager.Instance.f_Init();
 */

/// <summary> ī�޶� �켱������ �����ϰ� Blend �̺�Ʈ�� ó���ϴ� ī�޶� �Ŵ��� Ŭ���� </summary>
public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance = null;
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
            _instance = this;
            DontDestroyOnLoad(gameObject); //ī�޶� �Ŵ��� ����
        }
        else if (_instance != this)
        {
            Debug.Log("CameraManager has another instance.");
            Destroy(gameObject);
        }
    }

    // ī�޶� ���� �ʵ�
    private CinemachineCamera camDefault = null;        //�⺻ ī�޶�
    private CinemachineCamera camZoomTarget1 = null;    //�� 1�� ī�޶�
    private CinemachineCamera camZoomTarget2 = null;    //�� 2�� ī�޶�
    private CinemachineCamera camZoomTarget3 = null;    //�� 3�� ī�޶�
    private CinemachineCamera camTrajectory = null;     //���� ī�޶�
    
    private CinemachineBrain cinemachineBrain = null;   //Blend ���θ� Ȯ���ϱ� ���� CinemachineBrain ������Ʈ�� �ҷ����� ����

    //ī�޶� �켱���� �����
    private const int nDefaultPriority = 10;  //�⺻ �켱����
    private const int nActivePriority = 20;   //Ȱ��ȭ �켱����
    private const float fWaitTime = 2.0f;     //ī�޶� ��ȯ�� ���ð�

    public delegate void CameraBlendCompleteDelegate(); //��������Ʈ(delegate): ���� �Ϸ� �� ȣ��� �޼ҵ� ���� ����
    public event CameraBlendCompleteDelegate OnCameraBlendComplete; //�̺�Ʈ(event): �ܺο��� ������ �� �ִ� �̺�Ʈ

    /// <summary> ī�޶� ������ Blend ����� �������� ��ȯ�ϴ� ������Ƽ </summary>
    public bool IsCameraReady
    {
        get
        {
            return camDefault != null &&
                   camDefault.Priority == nActivePriority &&
                   cinemachineBrain != null &&
                   !cinemachineBrain.IsBlending;
            //�⺻ ī�޶� Ȱ��ȭ ���� && cinemachineBrain�� null�� �ƴ� && Blend ���°� �ƴ� ��� IsCameraReady "true" �� ����
        }
    }

    /// <summary> ��Ÿ��(Runtime)���� ī�޶�� ������Ʈ�� �����ϴ� �ʱ�ȭ �޼ҵ� </summary>
    public void f_Init()
    {
        /*
         * �ڵ��� �������� ���� �߻� ������ ���� C#�� null ���� �������� '?.'�� �����
         * ���� : gPowerGauge?.GetComponent<Image>() ���� gPowerGauge�� null�� �ƴϸ� GetComponent<Image>()�� ����
         *        null�̸� null�� ��ȯ�ϰ� ���ܸ� �߻���Ű�� ����
         */

        camDefault = GameObject.Find("Cam_Default")?.GetComponent<CinemachineCamera>();
        camZoomTarget1 = GameObject.Find("Cam_ZoomTarget1")?.GetComponent<CinemachineCamera>();
        camZoomTarget2 = GameObject.Find("Cam_ZoomTarget2")?.GetComponent<CinemachineCamera>();
        camZoomTarget3 = GameObject.Find("Cam_ZoomTarget3")?.GetComponent<CinemachineCamera>();
        camTrajectory = GameObject.Find("Cam_Trajectory")?.GetComponent<CinemachineCamera>();

        GameObject gMainCamera = GameObject.Find("Main Camera");
        if (gMainCamera != null)
        {
            cinemachineBrain = gMainCamera.GetComponent<CinemachineBrain>(); //CinemachineBrain ������Ʈ ��������
        }

        //�����
        if (camDefault == null || camZoomTarget1 == null || camTrajectory == null)
        {
            Debug.LogWarning("�Ϻ� ī�޶� ������� �ʾҽ��ϴ�. �̸��� Ȯ�����ּ���.");
        }

        f_SetCameraPriority(camDefault); //f_Init ȣ���, �⺻ ī�޶�� �ʱ�ȭ
    }

    /// <summary> ī�޶� ���� �������� �ڷ�ƾ���� �����ϴ� �޼ҵ� </summary>
    public void f_MoveCameraRoutine()
    {
        //Coroutine : �Ͻ� �ߴ��� ������ �޼ҵ�, ���� �����ӿ� ���� �۾��� ������ ó���� �� �ֵ��� ���ִ� ����� ������
        StartCoroutine(CameraMoveSequence()); //CameraMoveSequence ����
    }

    /*
     * IEnumerator : C#�� �������̽���, �ݺ� ������ ����(Enumerable)�� ��ȯ
     * �⺻������ Unity�� yield �� ������ �����ӿ� �ڷ�ƾ�� �ٽ� �����Ѵ�
     */

    /// <summary>ī�޶� ������ ���� �� �̺�Ʈ�� ��������� ȣ��</summary>
    private IEnumerator CameraMoveSequence()
    {
        int nActiveTargetIndex = TargetManager.Instance.f_GetActiveTargetIndex(); //Ȱ��ȭ ���� Index�� ������

        switch(nActiveTargetIndex) //Index = 0 ���� ����
        {
            case 0:
                f_SetCameraPriority(camZoomTarget1); //1�� �� ī�޶� Ȱ��ȭ
                break;

            case 1:
                f_SetCameraPriority(camZoomTarget2); //2�� �� ī�޶� Ȱ��ȭ
                break;

            case 2:
                f_SetCameraPriority(camZoomTarget3); //3�� �� ī�޶� Ȱ��ȭ
                break;

            default:
                f_SetCameraPriority(camDefault);    //�⺻ ī�޶� Ȱ��ȭ
                break;
        }
        yield return new WaitForSeconds(fWaitTime);

        f_SetCameraPriority(camTrajectory); //���� ī�޶� Ȱ��ȭ
        yield return new WaitForSeconds(fWaitTime);

        f_SetCameraPriority(camDefault); //�⺻ ī�޶� Ȱ��ȭ
        yield return new WaitUntil(() => IsCameraReady); //Blend�� �Ϸ�ɶ����� ��ƾ ���
        yield return new WaitForSeconds(0.5f); //blend�� �Ϸ�Ǿ����� ī�޶� ���󺹱ͱ��� 0.5�ʰ� �߰��� �ʿ���(���� ����)

        /*
         * event�� �ƹ��� �������� ������ null ����, null üũ�� ���� �ʰ� Invoke() �ϸ� ���ܰ� �߻��Ѵ�.
         * ���� null üũ�� �ؾ��Ѵ�. null ���� �����ڸ� ���� nullüũ �� Invoke() �޼ҵ� ȣ��
         */
        OnCameraBlendComplete?.Invoke(); //�ܺ� �����ڿ��� Blend �ϷḦ �˸�

        /*
        if (OnCameraBlendComplete != null) 
        {
            OnCameraBlendComplete.Invoke();
        }
        */
    }

    /// <summary>��� ī�޶��� �켱������ �����ϰ� �־��� ī�޶� Ȱ��ȭ�ϴ� �޼ҵ�</summary>
    private void f_SetCameraPriority(CinemachineCamera camera)
    {
        camDefault.Priority = nDefaultPriority;
        camZoomTarget1.Priority = nDefaultPriority;
        camZoomTarget2.Priority = nDefaultPriority;
        camZoomTarget3.Priority = nDefaultPriority;
        camTrajectory.Priority = nDefaultPriority;

        if (camera != null)
        {
            camera.Priority = nActivePriority;
        }
    }
}
