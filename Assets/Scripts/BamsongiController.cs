/*
 * ���콺�� Ŭ���ϸ� �����(Chestnut Bur)�� �������� ���ư��� ���� ���� ��ũ��Ʈ
 */
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BamsongiController : MonoBehaviour
{
    //������� ������ �ð��� ǥ���� ���� LineRenderer�� �����
    LineRenderer lineRenderer = null;
    List<Vector3> trajectoryPoint = new List<Vector3>(); //������� ���� ���� List

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        lineRenderer = GetComponent<LineRenderer>(); //LineRenderer ��� ��������
        lineRenderer.positionCount = 0;
    }

    private void FixedUpdate()
    {
        f_RenderTrajectory(); //ȭ��� ���� �׸���
    }


    // Update is called once per frame
    void Update()
    {
        f_CheckFallAndDestroy(); //���ῡ ���� �ʰ� ����Ʒ��� ���Ͻ� ������Ʈ�� ������
    }

    ///<summary>����̿��� �߻� �������� ���� ���ϴ� �޼ҵ�</summary>
    public void f_TargetShoot(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(direction); //�Ű������� ���޵� Vector������ ���� ���Ѵ�.
    }

    //Physics�� ����ϹǷ� ����� ����̰� �浹�ϸ� OnCollisionEnter �޼ҵ尡 ȣ��Ǿ� �����
    //�浹 �߻��� ���� ��� �� ������ ó����
    private void OnCollisionEnter(Collision collision)
    {
        TargetController activeTarget = TargetManager.Instance.ActiveTarget; //Ȱ��ȭ�� Ÿ�� ���� �ҷ�����

        //���� Ȱ��ȭ�� ������ ���ų� �浹 ����� �ٸ��� Early return�Ͽ� ���� �ڵ�κ� ������ ����
        if (activeTarget == null || collision.gameObject != activeTarget.gameObject)
        {
            Debug.Log("��Ȱ���� ���ῡ �浹�Ͽ����ϴ�. �浹�� �����մϴ�.");
            return;
        }

        //���� ���� ��ƾ �Ͻ����� �� �浹���� ó��
        TargetManager.Instance.f_PauseTargetRoutine();  
        GetComponent<Rigidbody>().isKinematic = true;   
        GetComponent<ParticleSystem>().Play();
        SoundManager.Instance.f_PlaySFX(SoundName.SFX_Crash, 1.0f);
        
        //BoxCollider�� Center ��ǥ���� ���� ��ǥ�������� ��ȯ
        BoxCollider boxCollider = activeTarget.GetComponent<BoxCollider>();
        Vector3 vCenterWorld = activeTarget.transform.TransformPoint(boxCollider.center);

        //���� ��� : �浹 ������ ������ �߽������� �Ÿ� ����
        float fDistance = Vector3.Distance(collision.contacts[0].point, vCenterWorld);
        float fMaxRadius = boxCollider.size.x / 2.0f; //�� ũ�� ���

        GameManager.Instance.f_AddScoreByDistance(fDistance, fMaxRadius);
        UIManager.Instance.f_UpdateScore();
        UIManager.Instance.f_UpdateTotalScore();

        GameManager.Instance.CanShoot = false;
        CameraManager.Instance.f_MoveCameraRoutine();

        //ī�޶��� Blend�� ����� ��� ������Ʈ ����
        CameraManager.Instance.OnCameraBlendComplete += f_DestroyBamsongiAfterBlend;
    }

    private void f_DestroyBamsongiAfterBlend()
    {
        Destroy(gameObject); //������Ʈ ����
        CameraManager.Instance.OnCameraBlendComplete -= f_DestroyBamsongiAfterBlend; //�޸� ���� ���� "-=" ȣ�� �� �̺�Ʈ���� ����
    }

    /// <summary>RigidBody�� ������ �ǽð����� ����ϰ� �׸��� �޼ҵ�</summary>
    void f_RenderTrajectory()
    {
        if (!GetComponent<Rigidbody>().isKinematic) //Rigidbody�� �����̴� ������ ��ġ�� �������� �׸�
        {
            trajectoryPoint.Add(transform.position);                //���� ��ġ�� ����Ʈ�� �����
            lineRenderer.positionCount = trajectoryPoint.Count;     //����Ʈ ������ŭ lineRenderer�� �� ���� ���� �׸��� �Ҵ�
            lineRenderer.SetPositions(trajectoryPoint.ToArray());   //SetPositions �޼ҵ带 ����ؼ� Vector3 �迭�� LineRenderer�� ȭ��� �׸���.
        } 
    }

    /// <summary>����̰� ȭ�� �Ʒ��� ���Ͻ� �����ϴ� �޼ҵ�</summary>
    void f_CheckFallAndDestroy()
    {
        if (transform.position.y < -3.0f) //���� �Ʒ��� ���Ͻ� 
        {
            GameManager.Instance.f_DecreaseShotCount(); //��ȸ 1ȸ ����
            UIManager.Instance.f_UpdateTotalScore(); //UI ����
            Destroy(gameObject);
        }
    }
}
