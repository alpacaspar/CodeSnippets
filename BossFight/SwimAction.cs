using Cinemachine;
using UnityEngine;

public class SwimAction : Action
{
    [SerializeField] private float swimSpeed;
    [SerializeField] private Vector2 swimDistance;
    private float distance;
    private float distanceTravelled;
    private bool distanceSet;
    
    [SerializeField] private CinemachineDollyCart cdc;

    public override bool PerformAction()
    {
        if (!distanceSet)
        {
            distance = Random.Range(swimDistance.x, swimDistance.y);
            distanceSet = true;
        }

        cdc.m_Speed = swimSpeed;
        distanceTravelled += cdc.m_Speed * Time.deltaTime;

        var targetDir = cdc.transform.position - transform.position;
        var step = swimSpeed * Time.deltaTime;
        var newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDir);
        
        transform.position += transform.forward * swimSpeed * Time.deltaTime;

        return distanceTravelled >= distance;
    }

    protected override void Reset()
    {
        cdc.m_Speed = 0;
        distanceTravelled = 0;
        distanceSet = false;
    }
}
