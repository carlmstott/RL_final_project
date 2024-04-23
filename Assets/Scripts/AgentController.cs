using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Numerics;

public class AgentController : Agent
{


    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 4f;

    public override void OnEpisodeBegin()
    {
        // Agent
        transform.localPosition = new UnityEngine.Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));

        // Pellet
        target.localPosition = new UnityEngine.Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        
        UnityEngine.Vector3 velocity = new UnityEngine.Vector3(moveX, 0f, moveZ);
        
        velocity = velocity.normalized * Time.deltaTime * moveSpeed; 
        transform.localPosition += velocity;

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Pellet")
        {
            AddReward(15f);
            EndEpisode();
        }
        if(other.gameObject.tag == "Wall")
        {
            AddReward(-1f);
            EndEpisode();
        }
    }
}
