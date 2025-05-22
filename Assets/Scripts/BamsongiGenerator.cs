using UnityEngine;
using UnityEngine.UI;

public class BamsongiGenerator : MonoBehaviour
{
    public GameObject gBamsongiPrefab = null;   //������ ���赵 ������ ���� public GameObject �ʵ�
    GameObject insBamsongiPrefab = null;        //Instantiate�� ����� ������Ʈ ���� �ʵ�
    Vector3 vBamsongiWorldDir = Vector3.zero;   //����� ���� ��ǥ

    bool isCharging = false;        //�������� ���� ����
    float fChargingTime = 0.0f;     //�������� ��,�� �������� ���� �ð� �ʵ�
    float fGaugeSpeed = 1.5f;       //�������� ��, �� ������ �ӵ�
    float fGaugeValue = 0.0f;       //������ UI�� FillAmount�� ����� ���� �ֱ� ���� �ʵ�
    float fGaugeMaxValue = 1.0f;    //�������� �ִ밪 �ʵ�
    float fGaugeLastValue = 0.0f;   //�������� ������ �� �ʵ�
    float fThrowStrength = 0.0f;    //����̸� ������ �� �ʵ�

    [SerializeField] private float fMinThrowStrength = 300.0f;
    [SerializeField] private float fGaugePowerMultiplier = 1500.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.Instance.CanShoot /*|| !CameraManager.Instance.IsCameraReady*/) //CanShoot�� false���̸� return ���� ���콺 ������ ������Ŵ
        {
            return;
        }

        //���콺 ��Ŭ���� �����ϸ� ������ ���� ����
        if (Input.GetMouseButtonDown(0))
        {
            f_StartChargeGauge();
        }

        //���콺 Ŭ�� ���� ���̸� ���� ������ ������Ʈ
        if (Input.GetMouseButton(0) && isCharging) 
        {
            f_UpdateChargeGauge();
        }

        //���콺 Ŭ�� ���� �� ����̸� �߻�
        if (Input.GetMouseButtonUp(0) && isCharging) //���콺�� �� ���
        {
            f_ReleaseGaugeAndShoot();
        }
    }

    void f_StartChargeGauge()
    {
        isCharging = true;
        fChargingTime = 0.0f;

        UIManager.Instance.f_ActivePowerGauge(true); //PowerGauge Ȱ��ȭ
    }

    void f_UpdateChargeGauge()
    {
        fChargingTime += Time.deltaTime * fGaugeSpeed;
        fGaugeValue = Mathf.PingPong(fChargingTime, fGaugeMaxValue);

        UIManager.Instance.f_SetGaugeAmount(fGaugeValue);
    }

    void f_ReleaseGaugeAndShoot()
    {
        isCharging = false;

        //������ �������� ������ �� ����
        fGaugeLastValue = UIManager.Instance.GaugeFillAmount;

        //������ �����ϴ� ���߿� ����� ������Ʈ�� ����
        insBamsongiPrefab = Instantiate(gBamsongiPrefab);

        /*
         * Ray Ŭ����
         * Ray(����)�� �̸� �״�� �����̸�, ������ ��ǥ(Origin)�� ������ ����(direction)�� ��� ������ ����
         * Ray�� �ݶ��̴��� ����� ������Ʈ�� �浹�� �����ϴ� Ư¡�� ����
         * ScreenPointToRay �޼ҵ��� ��ȯ������ ���� �� �ִ� Ray�� Origin�� Main camera�� ��ǥ��,
         *      direction�� ī�޶󿡼� ���� ��ǥ�� ���ϴ� ����
         * direction �������� ����̸� ������ ������ direction ���Ͱ� ���� nomalized ������ ����� ���̰� 1�� ���ͷ� ���� ��
         * ���� 2000 ���Ѵ�. �ϴ� ���̸� 1 ���ͷ� �ؼ� direction ���� ũ�⿡ ������� ����̿� ������ ���� ���� �� ����    
         */
        Ray ScreenPointToRayBamsongi = Camera.main.ScreenPointToRay(Input.mousePosition);
        vBamsongiWorldDir = ScreenPointToRayBamsongi.direction;

        //����̸� �߻��� ���� ����
        fThrowStrength = fMinThrowStrength + fGaugeLastValue * fGaugePowerMultiplier;

        //����̿� ���� �����ϴ� �޼ҵ忡 ������ �� ����
        insBamsongiPrefab.GetComponent<BamsongiController>().f_TargetShoot(vBamsongiWorldDir.normalized * fThrowStrength);

        //������ UI �ʱ�ȭ
        UIManager.Instance.f_SetGaugeAmount(0.0f);
        UIManager.Instance.f_ActivePowerGauge(false);
    }
    
}