/*
 * ���콺�� Ŭ���ϸ� �����(Chestnut Bur)�� �������� ���ư��� ���� ���� ��ũ��Ʈ
 */
using UnityEngine;

public class BamsongiController : MonoBehaviour
{
    GameObject gTarget = null; //���� ������Ʈ ������Ʈ ����
    BoxCollider boxCollider = null; //������ Box Collider ���� �������� ����

    Vector2 vHitXY = Vector2.zero;      //�Ÿ� ����� ���� ������� Ÿ���� X, Y��ǥ�� ����
    Vector2 vCenterXY = Vector2.zero;   //�Ÿ� ����� ���� �߽��� X, Y��ǥ�� ����

    float fDistance = 0.0f;     //����� Ÿ��������, ���� �߽ɱ����� �Ÿ�
    float fMaxRadius = 0.0f;    //������ ũ��
    float fKillObjTime = 3.0f;  //������Ʈ ���� �ð�

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gTarget = GameObject.Find("Target");                //���� ������Ʈ �ҷ�����
        boxCollider = gTarget.GetComponent<BoxCollider>();  //���� ������Ʈ�� BoxCollider ��� �ҷ�����
    }

    // Update is called once per frame
    void Update()
    {
        //����̰� ȭ�� �Ʒ��� ���Ͻ� ����
        if (transform.position.y < -3.0f)
        {
            Destroy(gameObject); 
        }
    }

    /*
     * ����̰� ȭ�� �������� ���ư����� +Z�� ������ ���͸� �Ű������� �����ϰ� f_TargetShoot �޼ҵ� ȣ��
     * Y�� �������� ���� 200.0f ���ϴ� ������ ����̰� ���ῡ �ݱ� ���� �߷��� ������ �޾�
     * �������� �����ϴ� ���� ���� ����
     * Start �޼ҵ带 ȣ���ϴ� ���۰� ���ÿ� ����̰� �������� ���ư�
     */

    ///<summary>����̿��� ���� ���ϴ� �޼ҵ�</summary>
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
        GetComponent<ParticleSystem>().Play(); //��ƼŬ ���

        if (boxCollider == null) //BoxCollider�� ���� ���
        {
            Debug.LogWarning($"{gTarget.gameObject.name}�� BoxCollider�� �����ϴ�.");
            return;
        }

        //boxCollider.size�� ����Ͽ� Inspector���� ���� �����ص� �ڵ����� Ÿ�� ���� ũ�� ���
        fMaxRadius = (boxCollider.size.x / 2.0f);

        /*
         * ����̰� Sphere Collider ���¶� Ÿ�������� ��ȣ�� �� �ִ� ������ �߻���
         * collision.contacts[0].point�� ����Ͽ� ��Ȯ�� �浹 ���� ��ġ���� �����
         * �׷���, collision.contacts[0].point ���� �״�� ����ϸ� z�ప�� ��꿡 ���Ե�
         * z�ప�� ���� �߽ɿ� Ÿ���ص� �Ÿ��� ũ�� ���Ǵ� ������ �߻���
         * ���� Vector2�� ����Ͽ� x,y���� �ݿ���
         */
        vHitXY = collision.contacts[0].point; //Ÿ������ ����
        vCenterXY = new Vector2(boxCollider.center.x, boxCollider.center.y); //�߽��� ����
        fDistance = Vector2.Distance(vHitXY, vCenterXY); //Ÿ�������� �߽������� �Ÿ� ���

        GameManager.Instance.f_AddScoreByDistance(fDistance, fMaxRadius); //���� ��� �� ���� ó��

        UIManager.Instance.f_UpdateScore(); //���� UI ����
        UIManager.Instance.f_UpdateTotalScore(); //���� UI ����

        Destroy(gameObject, fKillObjTime); //3�ʵ� ����
    }
}
