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
    float fGaugeMaxValue = 1.0f;    //�������� �ִ밪
    float fGaugeLastValue = 0.0f;   //�������� ������ �� 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) //���콺 ��Ŭ����
        {
            f_StartChargeGauge();
        }

        if(Input.GetMouseButton(0) && isCharging) //���콺 ��Ŭ���� && ������ ��¡�� 
        {
            f_UpdateChargeGauge();
        }

        if(Input.GetMouseButtonUp(0) && isCharging) //���콺�� �� ���
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

        float fShootPower = 500.0f + 1200.0f * fGaugeLastValue;

        insBamsongiPrefab.GetComponent<BamsongiController>().f_TargetShoot(vBamsongiWorldDir.normalized * fShootPower);

        UIManager.Instance.f_SetGaugeAmount(0.0f);
        UIManager.Instance.f_ActivePowerGauge(false);
    }
    
}