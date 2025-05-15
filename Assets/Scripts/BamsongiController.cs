/*
 * ���콺�� Ŭ���ϸ� �����(Chestnut Bur)�� �������� ���ư��� ���� ���� ��ũ��Ʈ
 */
using UnityEngine;

public class BamsongiController : MonoBehaviour
{
    Transform targetCenter = null;  //������ �߽��� ���� ������Ʈ�� ��ǥ�� 
    BoxCollider boxCollider = null; //������ Box Collider ���� �������� ����

    Vector3 vHitPoint = Vector3.zero;   //������� �浹��ġ ��ǥ�� ����
    Vector2 vHitXY = Vector2.zero;      //�Ÿ� ����� ���� ������� X, Y��ǥ�� ����
    Vector2 vCenterXY = Vector2.zero;   //�Ÿ� ����� ���� �߽��� X, Y��ǥ�� ����

    float fDistance = 0.0f;     //����� Ÿ��������, ���� �߽ɱ����� �Ÿ�
    float fMaxRadius = 0.0f;    //������ ũ��

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TargetCenter ��ġ�� �������� ���� ����ϹǷ� TargetCenter�� transform ���� �ҷ��´�
        targetCenter = GameObject.Find("TargetCenter").transform;

        //TargetCenter�� �θ�(Target)���� BoxCollider�� �����´�.
        //Target ������Ʈ�� Find�Ͽ��� ������ Target�� TargetCenter�� ��Ӱ����̹Ƿ� �Ʒ��� ���� �ۼ���
        boxCollider = targetCenter.transform.parent.GetComponent<BoxCollider>();

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
        GetComponent<ParticleSystem>().Play(); //��ƼŬ ���

        
        if (targetCenter == null)
        {
            Debug.LogWarning("TargetCenter�� ã�� �� �����ϴ�.");
            return;
        }
        
        if (boxCollider == null)
        {
            Debug.LogWarning("Target�� BoxCollider�� �����ϴ�.");
            return;
        }

        //boxCollider.size�� ����Ͽ� Inspector���� ���� �����ص� �ڵ����� Ÿ�� ���� ũ�� ���
        fMaxRadius = (boxCollider.size.x / 2.0f);

        /*
         * ����̰� Sphere Collider ���¶� Ÿ�������� ��ȣ�� �� �ִ� ������ �߻���
         * collision.contacts[0].point�� ����Ͽ� ��Ȯ�� �浹 ���� ��ġ���� �����
         */
        vHitPoint = collision.contacts[0].point;

        /*
         * collision.contacts[0].point ���� �״�� ����ϸ�
         * TargetCenter�� Target�� ���ο� �����Ͽ� z���� ��꿡 ���Ե�
         * ��, Vector3.Distance ���� z���� ���ԵǱ� ������ ���� ���� �߽ɿ� Ÿ���ص� �Ÿ��� ũ�� ���Ǵ� ������ �߻���
         * �� ������ �ذ��ϱ�����, z�� ���� ���ܽ�Ű�� ������
         */
        vHitXY = new Vector2(vHitPoint.x, vHitPoint.y);
        vCenterXY = new Vector2(targetCenter.position.x, targetCenter.position.y);

        fDistance = Vector2.Distance(vHitXY, vCenterXY);

        GameManager.Instance.f_AddScoreByDistance(fDistance, fMaxRadius); //���� ��� �� ���� ó��
    }
}
