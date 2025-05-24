/*
 * ���콺�� Ŭ���ϸ� �����(Chestnut Bur)�� �������� ���ư��� ���� ���� ��ũ��Ʈ
 */
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BamsongiController : MonoBehaviour
{
    GameObject gActiveTarget = null; //Ȱ��ȭ ������Ʈ ����
    GameObject gTarget = null; //���� ������Ʈ ����
    BoxCollider boxCollider = null; //������ Box Collider ���� �������� ����

    Vector2 vHitXY = Vector2.zero;      //�Ÿ� ����� ���� ������� Ÿ���� X, Y��ǥ�� ����
    Vector2 vCenterXY = Vector2.zero;   //�Ÿ� ����� ���� �߽��� X, Y��ǥ�� ����

    float fDistance = 0.0f;     //����� Ÿ��������, ���� �߽ɱ����� �Ÿ�
    float fMaxRadius = 0.0f;    //������ ũ��
    //float fKillObjTime = 7.0f;  //������Ʈ ���� �ð�

    //������� ������ �ð��� ǥ���� ���� LineRenderer�� �����
    LineRenderer lineRenderer = null;
    List<Vector3> trajectoryPoint = new List<Vector3>(); //������� ���� ���� List

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //gTarget = GameObject.Find("Target"); //���� ������Ʈ �ҷ�����

        /*
         * FindAnyObjectByType : Unity 2023���� ���Ե� �ֽ� API, GameObject.Find()���� �����ϰ� ��Ȯ�� ���
         * GameObject.Find()�� ���ڿ��� ã�ƿ��� ������ ��Ÿ�� ���ɼ��� ����������, ���׸� ������� Ÿ���� ���� ã�� ������ ��Ÿ ���� ������
         * ������Ʈ Ÿ�Ը� �����ϸ� �ǹǷ� �����ϰ� �������� ���
         */
        gTarget = FindAnyObjectByType<TargetManager>().f_GetCurrentTarget(); //���� Ȱ��ȭ�� ���� ������Ʈ �ҷ�����
        
        boxCollider = gTarget.GetComponent<BoxCollider>();  //���� ������Ʈ�� BoxCollider ��� �ҷ�����
        
        lineRenderer = GetComponent<LineRenderer>();    //LineRenderer ��� ��������
        lineRenderer.positionCount = 0;
    }

    private void FixedUpdate()
    {
        f_RenderTrajectory(); //ȭ��� ���� �׸���
    }


    // Update is called once per frame
    void Update()
    {
        //���ῡ ���� �ʰ� ����Ʒ��� ���Ͻ� ������Ʈ�� ������
        f_CheckFallAndDestroy();
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
        if(TargetManager.Instance != null && TargetManager.Instance.ActiveTarget !=null)
        {
            gActiveTarget = TargetManager.Instance.ActiveTarget.gameObject;
        }

        if (collision.gameObject != gActiveTarget) //��Ȱ�� ���ῡ ������� ��������� ���� �ʵ��� return
        {
            Debug.LogWarning("��Ȱ�� ���ῡ �浹�Ͽ����ϴ�.");
            return;
        }

        TargetManager.Instance.f_PauseTargetRoutine(); //����̰� ���ῡ �浹�� ���� ��ƾ �Ͻ����� 

        /*
         * ����̰� ���ῡ ��� ���� ����� �������� ���߹Ƿ�, Rigidbody ������Ʈ�� isKinematic �޼ҵ带 true�� ����
         * isKinematic �޼ҵ带 true�� ���� �ϸ�, ������Ʈ�� �ۿ��ϴ� ���� �����ϰ� ����̸� ������Ŵ
         * isKinematic �޼ҵ� : �ܺο��� �������� ������ ���� �������� �ʴ� ������Ʈ��� �ǹ�. �߷°� �扛�� �������� �ʵ��� ��
         */
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<ParticleSystem>().Play(); //��ƼŬ ���
        f_ApplyScoreAndDestroy(collision); //���� ��� �� ������Ʈ �� ������Ʈ ����
    }

    /// <summary>�浹�� ������ ����ϰ� ������Ʈ�� ���ִ� �޼ҵ�</summary>
    void f_ApplyScoreAndDestroy(Collision collision)
    {
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

        GameManager.Instance.CanShoot = false; //����� �߻� ����

        CameraManager.Instance.f_MoveCameraRoutine(); //ī�޶� ���� ����

        //Destroy(gameObject, fKillObjTime); //fKillObjTime ��ŭ ����� ������Ʈ ����

        CameraManager.Instance.OnCameraBlendComplete += f_DestroyBamsongiAfterBlend; //Bland�� �������� �˸��� �̺�Ʈ �߻��� ����� ����
    }

    private void f_DestroyBamsongiAfterBlend()
    {
        Destroy(gameObject); //������Ʈ ����

        //�޸� ���� ���� "-=" ȣ�� �� �̺�Ʈ���� ����
        CameraManager.Instance.OnCameraBlendComplete -= f_DestroyBamsongiAfterBlend;
    }

    /// <summary>RigidBody�� ������ �ǽð����� ����ϰ� �׸��� �޼ҵ�</summary>
    void f_RenderTrajectory()
    {
        //Rigidbody�� �����̴� ������ ��ġ�� �������� �׸�
        if (!GetComponent<Rigidbody>().isKinematic) //������ ����� ��� �Ʒ� �ڵ� ����
        {
            trajectoryPoint.Add(transform.position); //���� ��ġ�� ����Ʈ�� �����

            //����Ʈ ������ŭ lineRenderer�� �� ���� ���� �׸��� �Ҵ�
            lineRenderer.positionCount = trajectoryPoint.Count;
            //SetPositions �޼ҵ带 ����ؼ� Vector3 �迭�� LineRenderer�� ȭ��� �׸���.
            lineRenderer.SetPositions(trajectoryPoint.ToArray()); 
        }
    }

    /// <summary>����̰� ȭ�� �Ʒ��� ���Ͻ� �����ϴ� �޼ҵ�</summary>
    void f_CheckFallAndDestroy()
    {
        if (transform.position.y < -3.0f) //���� �Ʒ��� ���Ͻ� 
        {
            Destroy(gameObject);
        }
    }
}
