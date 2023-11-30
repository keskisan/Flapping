
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PickUpPoint : UdonSharpBehaviour
{
    [SerializeField]
    GameManager gameManager;

    VRCPlayerApi localPlayer;

    Vector3 startPosition;

    [SerializeField]
    float speed = 5f;

    [HideInInspector]
    public bool isInGame = false; //check on master

    [UdonSynced, FieldChangeCallback(nameof(Position))]
    private Vector3 position;

    public Vector3 Position
    {
        set
        {
            transform.position = value;
            position = value;
        }
        get => position;
    }

    

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        startPosition = transform.position;
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0f, speed * Time.deltaTime, 0f)); 
    }

    public override void OnPlayerCollisionEnter(VRCPlayerApi player) //doesnt work check spawnstuff
    {
    }


    public void SpawnIn(Vector3 spawnPosition) //only master ever calls. this should belong master
    {
        isInGame = true;
        Position = spawnPosition;
        RequestSerialization();
    }

    public void DSP()
    {
        Position = startPosition;
        isInGame = false;
        gameManager.spawnStuff.CheckIfAllCollectedGameEnd();
    }

    public void DeSpawn()
    {
        Position = startPosition; //should stop player from multi collecting if network slow
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "DSP");
    }

    public void DeSpawnNoGameCheck()
    {
        Position = startPosition;
    }
}
