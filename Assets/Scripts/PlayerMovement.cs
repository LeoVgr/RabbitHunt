using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : Agent
{
    public float speed;
    public GameObject floor;
    public GameManager gameManager;
    public GameObject movementParticles;
    public List<AudioClip> footStepsSound;

    private Rigidbody rBody;   
    private Vector3 forward, right;
    private Vector3 lastHeading;
    private int nextFootStep = 0;
    private bool isPlayingFootStep = false;
    private float timer = 0;
    private float timeBetweenFootStep = 0.2f;



    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();

        movementParticles.SetActive(false);
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
        lastHeading = new Vector3();

    }

    private void Update()
    {
        timer += Time.deltaTime;
        FootStep();
    }

    public override void OnEpisodeBegin()
    {
        // Reset agent
        this.rBody.angularVelocity = Vector3.zero;
        this.rBody.velocity = Vector3.zero;
        gameManager.StartNewGame();



    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Hunter and Agent positions & velocity
        foreach (GameObject rabbit in gameManager.rabbits)
        {
            if (!rabbit.GetComponent<RabbitAgentLogic>().isCatched)
            {
                sensor.AddObservation(rabbit.transform.localPosition);
                sensor.AddObservation(rabbit.GetComponent<Rigidbody>().velocity);
            }
            
        }
        
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rBody.velocity);
        sensor.AddObservation(floor.transform.position - this.transform.position);
    }

    public override void OnActionReceived(float[] vectorAction)
    {

        AddReward(-.1f);
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

        Vector3 rightMovement = right * speed * Time.deltaTime * controlSignal.x;
        Vector3 upMovment = forward * speed * Time.deltaTime * controlSignal.z;
        Vector3 heading = Vector3.Normalize(rightMovement + upMovment);

        //To maintain the direction of the player when no key are pressed
        if (heading != new Vector3(0,0,0))
        {
            transform.forward = heading;
            lastHeading = heading;
            movementParticles.SetActive(true);

            isPlayingFootStep = true;
            
        }
        else
        {
            isPlayingFootStep = false;
            movementParticles.SetActive(false);
            transform.forward = lastHeading;
        }

        
        transform.position += rightMovement;
        transform.position += upMovment;
       

        // Fell of platform
        if (this.transform.localPosition.y < -1)
        {
            AddReward(-20f);
            gameManager.GameOver();
            //EndEpisode();
         
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (gameManager.rabbits.Contains(collision.gameObject))
        {
            if (!collision.gameObject.GetComponent<RabbitAgentLogic>().isCatched)
            {
                AddReward(100.0f);
                gameManager.RabbitCatched(collision.gameObject);
            }

            if (gameManager.IsWon())
            {
                gameManager.Win();
                //EndEpisode();
            }
                
            
        }
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = -Input.GetAxis("Horizontal");
        actionsOut[1] = -Input.GetAxis("Vertical");
    }

    private void FootStep()
    {
        if (!isPlayingFootStep)
            GetComponent<AudioSource>().Stop();

        if (isPlayingFootStep && timer >= timeBetweenFootStep)
        {
            GetComponent<AudioSource>().clip = footStepsSound[nextFootStep];
            GetComponent<AudioSource>().Play();

            nextFootStep++;

            if (nextFootStep == footStepsSound.Count)
                nextFootStep = 0;

            timer = 0;
        }
    }
}
