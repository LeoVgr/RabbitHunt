using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class RabbitAgentLogic : Agent
{
    Rigidbody rBody;
    public GameObject hunter;
    public GameObject floor;
    public bool isCatched = false;
    public float speed;
    public GameManager gameManager;

    //temp
    Vector3 forward, right;


    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();      

        //movement
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }

    public override void OnEpisodeBegin()
    {
        // Reset agent
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Hunter and Agent positions & velocity
        sensor.AddObservation(hunter.transform.localPosition);
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity);
        sensor.AddObservation(hunter.GetComponent<Rigidbody>().velocity);
        sensor.AddObservation(floor.transform.position - this.transform.position);
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 controlSignal = Vector3.zero;

        if (vectorAction[0] == 2)
        {
            controlSignal.x = 1;
        }
        else
        {
            controlSignal.x = -vectorAction[0];
        }

        if (vectorAction[1] == 2)
        {
            controlSignal.z = 1;
        }
        else
        {
            controlSignal.z = -vectorAction[1];
        }

        ////temp
        Vector3 rightMovement = right * speed * Time.deltaTime * controlSignal.x;
        Vector3 upMovment = forward * speed * Time.deltaTime * controlSignal.z;

        Vector3 heading = Vector3.Normalize(rightMovement + upMovment);

        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovment;

        ////// Fell of platform
        if (this.transform.localPosition.y < -1)
        {
            AddReward(-20f);
            this.isCatched = true;
            //EndEpisode();
        }
        else
        {
            AddReward(0.1f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == hunter)
        {
            AddReward(-1f);
            
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxis("Vertical");
        actionsOut[1] = -Input.GetAxis("Horizontal");
    }
}
