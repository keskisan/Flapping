
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class Player : UdonSharpBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    PlayerManager playerManager;

    public PlayerSpecific playerSpecific;

    private VRCPlayerApi ownerPlayer = null; //owner if object in game

    public VRCPlayerApi OwnerPlayer 
    {
        get
        {
            return ownerPlayer;
        }
    }

    private void Start()
    {
        SetOwnerPlayer();
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player)
    {
        SetOwnerPlayer();
    }

    public void SetOwnerPlayer()
    {
        ownerPlayer = Networking.GetOwner(gameObject);
        if (ownerPlayer.isMaster)
        {
            if (this != playerManager.players[0]) //returns to pool
            {
                ownerPlayer = null;
            }
        }
        else
        {
            //code for when someone gets it
        }
    }

    //Sunced veriables. These get variables from everyone and updates them in gamemanager depending on whom ever changed last

    [UdonSynced, FieldChangeCallback(nameof(Cutoff))]  
    private float cutoff = 0.3f;
    public float Cutoff
    {
        set
        {
            if (value > 0.7f)
            {
                cutoff = 0.7f;
            } else if (value < 0f)
            {
                cutoff = 0f;
            }
            else
            {
                cutoff = value;
            }
            if (Networking.IsMaster) gameManager.UpdateCutoff(cutoff);
        }
        get => cutoff;
    }


    [UdonSynced, FieldChangeCallback(nameof(RandomiseNoise))] 
    private Vector3 randomiseNoise;
    public Vector3 RandomiseNoise
    {
        set
        { 
            randomiseNoise = value;
            if (Networking.IsMaster) gameManager.UpdateRandomNoise(value);
        }
        get => randomiseNoise;
    }


    [UdonSynced, FieldChangeCallback(nameof(XDir))]
    private int xDir = 10;
    public int XDir
    {
        set
        {
            xDir = value;
            if (Networking.IsMaster) gameManager.UpdateXDir(value);
        }
        get => xDir;
    }

    [UdonSynced, FieldChangeCallback(nameof(YDir))] 
    private int yDir = 15;
    public int YDir
    {
        set
        {
            yDir = value;
            if (Networking.IsMaster) gameManager.UpdateYDir(value);
        }
        get => yDir;
    }

    [UdonSynced, FieldChangeCallback(nameof(ZDir))] 
    private int zDir = 20;
    public int ZDir
    {
        set
        {
            zDir = value;
            if (Networking.IsMaster) gameManager.UpdateZDir(value);
        }
        get => zDir;
    }
}
