
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PlayerManager : UdonSharpBehaviour
{
    public Player[] players;

    private VRCPlayerApi localPlayer;
    Player _localPlayerWings = null;

    public Player LocalPlayerWings
    {
        get
        {
            return _localPlayerWings;
        }
    }

    void Start()
    {
        localPlayer = Networking.LocalPlayer;

        AssignePlayersAWing();
    }


    private void AssignePlayersAWing()
    {
        if (Networking.IsMaster)
        {
            _localPlayerWings = players[0];
            return;
        }
        else
        {
            for (int i = 1; i < players.Length; i++) //all capsules except 0 that belongs to master that is unused
            {
                if (Networking.GetOwner(players[i].gameObject).isMaster)
                {
                    _localPlayerWings = players[i];
                    Networking.SetOwner(localPlayer, players[i].gameObject);
                    return;
                }
            }
        }
    }

    public override void OnPlayerLeft(VRCPlayerApi player) //master always has capsule 0 so new masters loose thier capsule
    {
        if (localPlayer.isMaster)
        {
            if (_localPlayerWings != players[0])
            {
                players[0].playerSpecific.meshDrawed = _localPlayerWings.playerSpecific.meshDrawed;
                players[0].playerSpecific.playerPoints = _localPlayerWings.playerSpecific.playerPoints;
                players[0].playerSpecific.isPlayerInGame = _localPlayerWings.playerSpecific.isPlayerInGame;
                _localPlayerWings.SetOwnerPlayer();
                _localPlayerWings = players[0];
            }
        }
    }

    public bool AtLeastOnePlayerInGame()
    {
        for (int i = 0; i < players.Length; i++) 
        {
            if (players[i].playerSpecific.isPlayerInGame) return true;
        }
        return false;
    }


    public bool IsWingsAssigned(Player wing)
    {
        if (wing == players[0]) return true; //master
        else if (Networking.GetOwner(wing.gameObject).isMaster) return false; //object not owned by anyone
        else return true; //is owned by someone
    }


    public bool HasEveryOnesMeshBeenDrawn()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (IsWingsAssigned(players[i]))
            {
                if (!players[i].playerSpecific.meshDrawed) return false; //found one not drawn
            }
        }
        return true;
    }

    

    public string GetPlayerWithMostPoints()
    {
        int mostPoints = 0;
        VRCPlayerApi winner = null;
        for (int i = 0; i < players.Length; i++)
        {
            if (IsWingsAssigned(players[i]))
            {
                if (players[i].playerSpecific.playerPoints > mostPoints)
                {
                    mostPoints = players[i].playerSpecific.playerPoints;
                    winner = Networking.GetOwner(players[i].gameObject);
                }
            }
        }
        if (winner == null)
        {
            return "";
        }
        else
        {   
            return "The winner is: " + winner.displayName + " with " + mostPoints + " points";
        }
    }
}
