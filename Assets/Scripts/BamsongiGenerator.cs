using UnityEngine;
using UnityEngine.UI;

public class BamsongiGenerator : MonoBehaviour
{
    public GameObject gBamsongiPrefab = null;   //������ ���赵 ������ ���� public GameObject �ʵ�
    GameObject insBamsongiPrefab = null;        //Instantiate�� ����� ������Ʈ ���� �ʵ�
    Vector3 vBamsongiWorldDir = Vector3.zero;   //����� ���� ��ǥ

    //------------------------[������� ���� Ʋ���� ����]------------------------
    Vector3 vSpawnPosition = Vector3.zero;      //����̰� ������ ��ǥ
    Quaternion vRotation = Quaternion.identity; //����̰� �����ǰ� �ٶ� ����

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
            SoundManager.Instance.f_PlaySFX(SoundName.SFX_PopSound, 1.0f); //����� �߻� ȿ����
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

    /// <summary> ���콺 Ŭ���� ������ ����̸� �����ϰ� Ŭ�� �������� �߻��ϴ� �޼ҵ� </summary>
    void f_ReleaseGaugeAndShoot()
    {
        /*
         * Ray Ŭ����
         * Ray(����)�� �̸� �״�� �����̸�, ������ ��ǥ(Origin)�� ������ ����(direction)�� ��� ������ ����
         * Ray�� �ݶ��̴��� ����� ������Ʈ�� �浹�� �����ϴ� Ư¡�� ����
         * ScreenPointToRay �޼ҵ��� ��ȯ������ ���� �� �ִ� Ray�� Origin�� Main camera�� ��ǥ��,
         *      direction�� ī�޶󿡼� ���� ��ǥ�� ���ϴ� ����  
         */

        isCharging = false; //������ �ƴ� ���� ��ȯ

        fGaugeLastValue = UIManager.Instance.GaugeFillAmount; //UI �������� ���� fillAmount �� ����
        fThrowStrength = fMinThrowStrength + fGaugeLastValue * fGaugePowerMultiplier; //����̸� �߻��� ���� ����


        //------------------------[������� ���� Ʋ���� ����]------------------------

        Ray ScreenPointToRayBamsongi = Camera.main.ScreenPointToRay(Input.mousePosition); //Ŭ�� �������� Ray ����
        vBamsongiWorldDir = ScreenPointToRayBamsongi.direction.normalized; //Ray�� ���� ���͸� ����ȭ(normalized) �Ͽ� ���� ����(�߻��� ����)�� ����

        vSpawnPosition = ScreenPointToRayBamsongi.origin + vBamsongiWorldDir * 0.5f; //����̰� ������ ��ġ ����(Ŭ�� ���� ���)

        vRotation = Quaternion.LookRotation(vBamsongiWorldDir); //LookRotation �޼ҵ带 ����Ͽ� ����̰� �ٶ������ ���� ����
        
        insBamsongiPrefab = Instantiate(gBamsongiPrefab, vSpawnPosition, vRotation); //�����(gBamsongiPrefab)�� vSpawnPosition ��ġ���� vRotation �������� ����

        insBamsongiPrefab.GetComponent<BamsongiController>().f_TargetShoot(vBamsongiWorldDir * fThrowStrength); //����̿� ���� �����ϴ� �޼ҵ忡 ������ �� ����

        //------------------------[������� ���� Ʋ���� ����]------------------------

        //������ UI �ʱ�ȭ
        UIManager.Instance.f_SetGaugeAmount(0.0f);
        UIManager.Instance.f_ActivePowerGauge(false);
    }
    
}