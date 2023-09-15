using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class HookAgent : Agent
{
    public Rigidbody tC_Rails;
    public Rigidbody boom_point_Rotation;
    public Rigidbody truck;
    public Rigidbody hook;
    public Rigidbody hook_point_Rotation;

    private CargoContact cargoContact;

    private float tC_Rails_Range;
    private float boom_point_Rotation_Range = 360f;
    private float truck_Range;
    private float hook_Range;
    private float hook_point_Rotation_Range = 360f;



    public float speed = 0.1f;  // 用于控制物体的移动速度


    public override void Initialize()
    {
        // 初始化，方便访问和调整trigger_ancoragePoint的contactHook状态
        cargoContact = GetComponent<CargoContact>();

        StartCoroutine(CalculateMovementRange(tC_Rails, range => tC_Rails_Range = range));
        StartCoroutine(CalculateMovementRange(truck, range => truck_Range = range));
        StartCoroutine(CalculateMovementRange(hook, range => hook_Range = range));
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 收集观察
        sensor.AddObservation(tC_Rails.transform.localPosition);
        sensor.AddObservation(boom_point_Rotation.transform.localPosition);
        sensor.AddObservation(truck.transform.localPosition);
        sensor.AddObservation(hook.transform.localPosition);
        sensor.AddObservation(hook_point_Rotation.transform.localPosition);

        sensor.AddObservation(tC_Rails.velocity);
        sensor.AddObservation(boom_point_Rotation.velocity);
        sensor.AddObservation(truck.velocity);
        sensor.AddObservation(hook.velocity);
        sensor.AddObservation(hook_point_Rotation.velocity);

        sensor.AddObservation(tC_Rails.transform.position);
        sensor.AddObservation(boom_point_Rotation.transform.position);
        sensor.AddObservation(truck.transform.position);
        sensor.AddObservation(hook.transform.position);
        sensor.AddObservation(hook_point_Rotation.transform.position);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float[] continuousActions = actionBuffers.ContinuousActions.Array;

        // 执行动作
        StartCoroutine(MoveToPosition(tC_Rails, continuousActions[0] * tC_Rails_Range));
        StartCoroutine(RotateToAngle(boom_point_Rotation, continuousActions[1] * boom_point_Rotation_Range));
        StartCoroutine(MoveToPosition(truck, continuousActions[2] * truck_Range));
        StartCoroutine(MoveToPosition(hook, continuousActions[3] * hook_Range));
        StartCoroutine(RotateToAngle(hook_point_Rotation, continuousActions[4] * hook_point_Rotation_Range));


        // 如果hook_point_Rotation与cargo的trigger_ancoragePoint发生了trigger，把contactHook设置为true; 但是这个逻辑在CargoContact里已经实现了，这里只需要等着状态就行了吧？
        if (contactHook)
        {
            AddReward(1f);
            contactHook = false;
        }
        else
        {
            AddReward(-0.01f);
        }
        else
        {
            AddReward(0.01f);
        }

    }

    public override void OnEpisodeBegin()
    {
        // 如果需要，可以在这里重置环境
    }

    public override void Heuristic(float[] actionsOut)
    {
        // 如果需要，可以在这里添加启发式方法
        ;
    }

    IEnumerator CalculateMovementRange(Rigidbody rb, Action<float> setRange)
    {
        //这种方法可能有bug，先凑活用。
        // 开始向一个方向施加力
        rb.AddForce(Vector3.right * 1000f); // 需要根据具体情况选择合适的方向和力量
        rb.AddForce(Vector3.up * 1000f);

        // 等待直到物体停止
        while (rb.velocity.magnitude > 0.01f) 
        {
            yield return null; // 等待下一帧
        }

        // 记录位置
        Vector3 startPos = rb.transform.position;

        // 现在向相反的方向施加力
        rb.AddForce(Vector3.left * 1000f); // 需要根据具体情况选择合适的方向和力量
        rb.AddForce(Vector3.down * 1000f);

        // 再次等待直到物体停止
        while (rb.velocity.magnitude > 0.01f) 
        {
            yield return null; // 等待下一帧
        }

        // 记录位置
        Vector3 endPos = rb.transform.position;

        // 计算距离
        float movementRange = Vector3.Distance(startPos, endPos);
        setRange(movementRange);

        // 将结果打印或者存储到合适的地方
        Debug.Log(rb.name + " movement range: " + movementRange);
    }

    IEnumerator MoveToPositionLerp(Rigidbody rb, float targetPosition)
    {
        Vector3 startPosition = rb.transform.localPosition;
        Vector3 target = startPosition + Vector3.forward * targetPosition;

        while (Vector3.Distance(rb.transform.localPosition, target) > 0.01f)
        {
            rb.MovePosition(Vector3.Lerp(rb.transform.localPosition, target, speed * Time.deltaTime));
            yield return null;
        }
    }


    IEnumerator MoveToPosition(Rigidbody rb, float targetPosition)
    {
        Vector3 startPosition = rb.transform.localPosition;
        Vector3 target = startPosition + Vector3.forward * targetPosition;

        while (Vector3.Distance(rb.transform.localPosition, target) > 0.01f)
        {
            rb.MovePosition(Vector3.Lerp(rb.transform.localPosition, target, speed * Time.deltaTime));
            yield return null;
        }
    }

    IEnumerator RotateToAngle(Rigidbody rb, float targetAngle, float speed)
    {
        Quaternion startRotation = rb.transform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += speed;
            rb.MoveRotation(Quaternion.Slerp(startRotation, targetRotation, t));
            yield return null;
        }
    }

}
