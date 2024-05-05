using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Numerics;
using Unity.VisualScripting;

public class InstrumentController : Agent
{
    [SerializeField] private float moveDistance = .001f;

    // Better way to link many objects?
    public GameObject instrument1;
    public GameObject instrument2;
    public GameObject instrument3;
    public GameObject centerOfMassIndicator;


    public GameObject targetCOM;

    private UnityEngine.Vector3 lastPositionInstrument1;
    private UnityEngine.Vector3 lastPositionInstrument2;
    private UnityEngine.Vector3 lastPositionInstrument3;


    private float lastDistance;

    private int stepCount = 0;
    public int SimSteps = 1000;
    public float tol = 0.5f;

    public override void OnEpisodeBegin()
    {
        GameObject satelliteBody = GameObject.Find("SatelliteBody");
        Renderer SatelliteBodyRenderer = satelliteBody.GetComponent<Renderer>();
        Bounds arenaSize = SatelliteBodyRenderer.bounds;
        float width = arenaSize.size.x;
        float length = arenaSize.size.z;
        //lets do -9.5 to 9.5 for possible spawn range, should insure that instruments are always on the satellite
        float minx = -width/2 + .5f;
        float maxx = width/2  - .5f; //the .5f's make sure no cubes fall off
        float minz = -length/2  + .5f;
        float maxz = length/2  - .5f;
        

        // lets randomize the position of instuments each episode start
        instrument1.transform.localPosition = new UnityEngine.Vector3(Random.Range(minx, maxx), 3.2f, Random.Range(minz, maxz));
        instrument2.transform.localPosition = new UnityEngine.Vector3(Random.Range(minx, maxx), 3.2f, Random.Range(minz, maxz));
        instrument3.transform.localPosition = new UnityEngine.Vector3(Random.Range(minx, maxx), 3.2f, Random.Range(minz, maxz));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // sensor.AddObservation(instrument1.transform.position);
        // sensor.AddObservation(instrument2.transform.position);
        // sensor.AddObservation(instrument3.transform.position);

        //sensor.AddObservation(targetCOM.transform.position);

        Rigidbody rb_instrument1 = instrument1.GetComponent<Rigidbody>();
        Rigidbody rb_instrument2 = instrument2.GetComponent<Rigidbody>();
        Rigidbody rb_instrument3 = instrument3.GetComponent<Rigidbody>();
        UnityEngine.Vector3 COM = (rb_instrument1.mass * instrument1.transform.position + rb_instrument2.mass * instrument2.transform.position + rb_instrument3.mass * instrument3.transform.position) / (rb_instrument1.mass + rb_instrument2.mass+ rb_instrument3.mass);
        float error = UnityEngine.Vector3.Distance(COM, targetCOM.transform.position);

        // sensor.AddObservation(COM);
        sensor.AddObservation(error);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Seeing if episode should end by max step count
        stepCount++;
        // print(stepCount);
        if (stepCount >= SimSteps)
        {
            stepCount = 0;
            EndEpisode();
            stepCount = 0;
            print("Episode Ended by Step Count");
        }

        // Seeing if episode should end by successful task completion
        Rigidbody rb_instrument1 = instrument1.GetComponent<Rigidbody>();
        Rigidbody rb_instrument2 = instrument2.GetComponent<Rigidbody>();
        Rigidbody rb_instrument3 = instrument3.GetComponent<Rigidbody>();
        UnityEngine.Vector3 COM = (rb_instrument1.mass * instrument1.transform.position + rb_instrument2.mass * instrument2.transform.position + rb_instrument3.mass * instrument3.transform.position) / (rb_instrument1.mass + rb_instrument2.mass+ rb_instrument3.mass);
        centerOfMassIndicator.transform.position = COM;

        float error = UnityEngine.Vector3.Distance(COM, targetCOM.transform.position);
        if (error < tol)
        {
            AddReward(1.0f);
            EndEpisode();
            print("Episode Ended by Success");
        }

        // Tracking prev locations
        lastPositionInstrument1 = instrument1.transform.position;
        lastPositionInstrument2 = instrument2.transform.position;
        lastPositionInstrument3 = instrument3.transform.position;
        lastDistance = error;


        // Extracting actions
        int actionForInstrument1 = actions.DiscreteActions[0];
        int actionForInstrument2 = actions.DiscreteActions[1];   
        int actionForInstrument3 = actions.DiscreteActions[2];  

        ApplyMovement(actionForInstrument1, instrument1);
        ApplyMovement(actionForInstrument2, instrument2);
        ApplyMovement(actionForInstrument3, instrument3);
    }

    void ApplyMovement(int action, GameObject obj)
    {
        UnityEngine.Vector3 direction = UnityEngine.Vector3.zero;
        // This might not be the most elegant way to do this
        switch(action)
        {
            case 0: direction = UnityEngine.Vector3.left; break;
            case 1: direction = UnityEngine.Vector3.right; break;
            case 2: direction = UnityEngine.Vector3.forward; break;
            case 3: direction = UnityEngine.Vector3.back; break;
        }
        obj.transform.localPosition += direction * moveDistance * Time.deltaTime;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Random.Range(0, 4);
        discreteActionsOut[1] = Random.Range(0, 4);
        discreteActionsOut[2] = Random.Range(0, 4);
        
        
    }

    private void FixedUpdate()
    {
        CalculateReward();
    }

    void CalculateReward()
    {
        Rigidbody rb_instrument1 = instrument1.GetComponent<Rigidbody>();
        Rigidbody rb_instrument2 = instrument2.GetComponent<Rigidbody>();
        Rigidbody rb_instrument3 = instrument3.GetComponent<Rigidbody>();
        UnityEngine.Vector3 COM = (rb_instrument1.mass * instrument1.transform.position + rb_instrument2.mass * instrument2.transform.position + rb_instrument3.mass * instrument3.transform.position) / (rb_instrument1.mass + rb_instrument2.mass+ rb_instrument3.mass);
        float error = UnityEngine.Vector3.Distance(COM, targetCOM.transform.position);

        float reward = 0f;
        AddReward(reward);
        
        // Easy to see if agent moving in right direction by printing these results
        //print(reward);
        //print(error);

        // Amplify negative rewards  honestly lets not do this, makes the whole thing harder
        // if (reward < 0)
        // {
        //     AddReward(10.0f*reward);
        // }
        // else
        // {
        //     AddReward(reward);
        // }
    }
}
