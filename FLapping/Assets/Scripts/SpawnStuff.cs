
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SpawnStuff : UdonSharpBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    PickUpPoint[] PickupsToSpawn;

    public float pickUpDistance = 3f;

    public void SpawnStuffIn(Vector3[] positions, int length, float spawnOffset) //only master call
    {
        Vector3[] tempPositions = new Vector3[length]; //copy used part of array
        Array.Copy(positions, tempPositions, length);

        for (int t = 0; t < length; t++) //shuffle array
        {
            Vector3 tmp = tempPositions[t];
            int r = UnityEngine.Random.Range(t, length);
            tempPositions[t] = tempPositions[r];
            tempPositions[r] = tmp;
        }

        for (int i = 0; i < PickupsToSpawn.Length; i++) //place items
        {
            if (i < length)
            {
                PickupsToSpawn[i].SpawnIn(new Vector3(tempPositions[i].x, tempPositions[i].y + spawnOffset, tempPositions[i].z));
            }
            else
            {
                PickupsToSpawn[i].DeSpawnNoGameCheck();
            }
        }
    }

    public void CheckIfAllCollectedGameEnd() //owner master
    {
        for (int i = 0; i < PickupsToSpawn.Length; i++)
        {
            if (PickupsToSpawn[i].isInGame) return; //at least one point in game
        } 
        gameManager.EndGameCollectedAllPoints();
        gameManager.SpawnStuffInAfterMapDrawn();
    }


    private void Update()
    {
        CheckIfPickup();
    }

    private void CheckIfPickup() //ineffecient way but vrc oncollisionEnter not functioning
    {

        if (gameManager.playerManager.LocalPlayerWings.playerSpecific.isPlayerInGame)
        {
            for (int i = 0; i < PickupsToSpawn.Length; i++)
            {
                if (PickupsToSpawn[i].isInGame)
                {
                    if (Vector3.Distance(gameManager.playerManager.LocalPlayerWings.transform.position, PickupsToSpawn[i].transform.position) < pickUpDistance)
                    {
                        gameManager.playerManager.LocalPlayerWings.playerSpecific.AddPlayerPoints(100);
                        PickupsToSpawn[i].DeSpawn();
                    }
                }
            }
        }
    }
}
