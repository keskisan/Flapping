
using System;
using UdonSharp;
using UnityEngine;
using VRC.Core;
using VRC.SDKBase;
using VRC.Udon;
using static VRC.SDKBase.VRC_Pickup;


//do camera views. 
//points display both at win and on game menu.
//pickup points and player specific need testing


public enum GAMESTATUS
{
    NOTPLAYING, ABORTED, WON, PLAYING, NUMBEROFTYPES
}

[DefaultExecutionOrder(-1), UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameManager : UdonSharpBehaviour
{
    public PlayerManager playerManager;

    public Voxels voxels;

    public SpawnStuff spawnStuff;

    [SerializeField]
    PlayerPointsDisplay playerPointsDisplay;

    [UdonSynced]
    public int xDir = 10, yDir = 15, zDir = 20;  

    [UdonSynced]
    public Vector3 randomiseNoise;  

    [UdonSynced]
    public float cutoff = 0.3f;
    //

    [HideInInspector]
    public bool worldToBigToMake;

    public void UpdateCutoff(float value)
    {
        cutoff = value;
        RequestSerialization();
    }

    public void UpdateRandomNoise(Vector3 value)
    {
        randomiseNoise = value;
        RequestSerialization();
    }

    public void UpdateXDir(int value)
    {
        xDir = value;
        RequestSerialization();
    }

    public void UpdateYDir(int value)
    {
        yDir = value;
        RequestSerialization();
    }

    public void UpdateZDir(int value)
    {
        zDir = value;
        RequestSerialization();
    }

    public void MeshHasBeenDrawn()
    {
        SpawnStuffInAfterMapDrawn();
    }

    public void SpawnStuffInAfterMapDrawn()
    {
        if (Networking.IsMaster)
        {
            spawnStuff.SpawnStuffIn(voxels.flatsurfaces, voxels.flatSurfaceCounter, voxels.transform.localScale.y * 0.7f);
        }
    }

    //Game manager non voxel stuff


    [SerializeField]
    Transform spawnInArea, startPosition, spawnInPoint;

    [SerializeField]
    float spawnIndistance;

    [SerializeField]
    GameLoopCanvas gameLoopCanvas;

    private float countdown = -1f;

    private float time_player_teleports_in = 5f;

    private bool resetHappenOnce = true;

    VRCPlayerApi localPlayer;

    [UdonSynced, FieldChangeCallback(nameof(Status))]
    private GAMESTATUS _status;

    public GAMESTATUS Status
    {
        set
        {
            if (Networking.IsMaster && countdown > 0f) return;
            if (value == GAMESTATUS.PLAYING)
            {
                countdown = 10f;
                resetHappenOnce = true;
            }
            else
            {
                EndGame(value);
            }
            gameLoopCanvas.UpdateMenuItemVisibility(value);
            _status = value;
        }
        get => _status;
    }

    public bool IsGamePlaying()
    {
        return Status == GAMESTATUS.PLAYING;
    }


    public void EndGameCollectedAllPoints() //owner master
    {
        Status = GAMESTATUS.WON;
        RequestSerialization();
    }

    private void EndGame(GAMESTATUS value)
    {
        switch (value)
        {
            case GAMESTATUS.NOTPLAYING:
                {
                    break;
                }
            case GAMESTATUS.ABORTED:
                {
                    TeleportPlayersOut();
                    break;
                }
            case GAMESTATUS.WON:
                {
                    TeleportPlayersOut();
                    ShowPointsAndWinner();
                    break;
                }
            default:
                {
                    Debug.Log("GameManager Endgame function called with invalid value");
                    break;
                }

        }
    }

    private void ShowPointsAndWinner() //everyone
    {
        gameLoopCanvas.UpdateWinText(playerManager.GetPlayerWithMostPoints());
        playerPointsDisplay.UpdatePointsDisplay();
    }

    private void TeleportPlayersOut() //game ends
    {
        if (playerManager.LocalPlayerWings.playerSpecific.isPlayerInGame)
        {
            localPlayer.TeleportTo(startPosition.position, startPosition.rotation);
            playerManager.LocalPlayerWings.playerSpecific.isPlayerInGame = false;

        }
    }

    private void Start()
    {
        gameLoopCanvas.UpdateMenuItemVisibility(Status);
        localPlayer = Networking.LocalPlayer;
    }

    private void UpdateGameStatus(GAMESTATUS newStatus)
    {
        Status = newStatus;
        RequestSerialization();
    }

    public void AbortGame()
    {
        if (!Networking.IsMaster) return;
        UpdateGameStatus(GAMESTATUS.ABORTED);
    }

    public void Update()
    {
        DoCountDown();

        CheckEndGameConditions();

    }

    private void CheckEndGameConditions()
    {
        if (!Networking.IsMaster) return;
        if (countdown > 0f) return;
        if (Status == GAMESTATUS.PLAYING)
        {
            if (!playerManager.AtLeastOnePlayerInGame()) //cant die in this game so players in game crashed or left
            {
               UpdateGameStatus(GAMESTATUS.NOTPLAYING);
            }
        }
    }

    private void DoCountDown()
    {
        if (countdown < 0f)
        {
            return;
        }
        countdown -= Time.deltaTime;
        int countdownToSpawnIn = (int)(countdown - time_player_teleports_in);
        if (countdownToSpawnIn > 0)
        {
            gameLoopCanvas.ToggleButtonCountdown();
            gameLoopCanvas.DisplayCountdown(countdownToSpawnIn.ToString());
        }
        else
        {
            HappenOnce();
        }
    }


    private void HappenOnce()
    {
        if (resetHappenOnce)
        {
            resetHappenOnce = false;
            TelepostPlayerIn(spawnInPoint);
            gameLoopCanvas.UpdateMenuItemVisibility(Status);
            gameLoopCanvas.DisplayCountdown("");
        }
    }


    private void TelepostPlayerIn(Transform teleportTo)
    {
        if (Vector3.Distance(localPlayer.GetPosition(), spawnInArea.position) < spawnIndistance * 0.5f)
        {  
            localPlayer.TeleportTo(teleportTo.position, teleportTo.rotation);
            playerManager.LocalPlayerWings.playerSpecific.TeleportIn();
        }
    }

    public override void OnPlayerRespawn(VRCPlayerApi player) //players dont die so respawning wont remove player from game
    {
        
    }

    public void StartNewGame()
    {
        if (!Networking.IsMaster)
        {
            Debug.Log("GameManager non master tried calling startnew game");
            return;
        }
        UpdateGameStatus(GAMESTATUS.PLAYING);
    }
}
