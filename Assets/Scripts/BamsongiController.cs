/*
 * ���콺�� Ŭ���ϸ� ����̰� �������� ���ư��� ���� ���� ��ũ��Ʈ
 */
using UnityEngine;

public class BamsongiController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //����̽� ���ɿ� ���� ������ ���� ���ֱ�
        Application.targetFrameRate = 60;

        /*
         * ����̰� ȭ�� �������� ���ư����� +Z�� ������ ���͸� �Ű������� �����ϰ� f_TargetShoot �޼ҵ� ȣ��
         * Y�� �������� ���� 200.0f ���ϴ� ������ ����̰� ���ῡ �ݱ� ���� �߷��� ������ �޾�
         * �������� �����ϴ� ���� ���� ����
         * Start �޼ҵ带 ȣ���ϴ� ���۰� ���ÿ� ����̰� �������� ���ư�
         */
        f_TargetShoot(new Vector3(0.0f, 200.0f, 2000.0f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// �Ű����� �������� ����̿��� ���� ���ϴ� �޼ҵ�
    /// </summary>
    /// <param name="argDir">����</param>
    public void f_TargetShoot(Vector3 argDir)
    {
        //�Ű������� ���޵� Vector������ ���� ���Ѵ�.
        GetComponent<Rigidbody>().AddForce(argDir);
    }

    //Physics�� ����ϹǷ� ����� ����̰� �浹�ϸ� OnCollisionEnter �޼ҵ尡 ȣ��Ǿ� �����
    private void OnCollisionEnter(Collision collision)
    {
        /*
         * ����̰� ���ῡ ��� ���� ����� �������� ���߹Ƿ�, Rigidbody ������Ʈ�� isKinematic �޼ҵ带 true�� ����
         * isKinematic �޼ҵ带 true�� ���� �ϸ�, ������Ʈ�� �ۿ��ϴ� ���� �����ϰ� ����̸� ������Ŵ
         * isKinematic �޼ҵ� : �ܺο��� �������� ������ ���� �������� �ʴ� ������Ʈ��� �ǹ�. �߷°� �扛�� �������� �ʵ��� ��
         */
        GetComponent<Rigidbody>().isKinematic = true;

        /*
         */
        GetComponent<ParticleSystem>().Play();
    }
}
