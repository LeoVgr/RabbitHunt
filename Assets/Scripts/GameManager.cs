using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //entity
    public GameObject player;
    public List<GameObject> rabbits;

    //Start
    public Transform playerStart;
    public List<Transform> rabbitsStarts;
    public Transform catchStart;

    public void StartNewGame()
    {
        player.transform.position = new Vector3(playerStart.position.x, playerStart.position.y + 0.37f, playerStart.position.z);


        foreach (GameObject rabbit in rabbits)
        {
            rabbit.GetComponent<RabbitAgentLogic>().isCatched = false;

            //Choose a random spawn location
            Transform choosenPos = rabbitsStarts[Random.Range(0, rabbitsStarts.Count)];
            rabbit.transform.position = new Vector3(choosenPos.position.x, choosenPos.position.y + 0.253f, choosenPos.position.z);


        }
    }

    public void RabbitCatched(GameObject rabbit)
    {
        rabbit.GetComponent<RabbitAgentLogic>().isCatched = true;
        rabbit.transform.position = catchStart.position;
    }

    public bool IsWon()
    {
        bool allRabbitCatched = true;

        foreach (GameObject rabbit in rabbits)
        {
            if (rabbit.GetComponent<RabbitAgentLogic>().isCatched == false)
                allRabbitCatched = false;
        }

        return allRabbitCatched;

    }

    public void GameOver()
    {

    }

    public void Win()
    {

    }
}
