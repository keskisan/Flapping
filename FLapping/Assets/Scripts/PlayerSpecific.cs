
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class PlayerSpecific : UdonSharpBehaviour
{
    [UdonSynced]
    public bool meshDrawed = false;

    [UdonSynced]
    public int playerPoints;

    [UdonSynced]
    public bool isPlayerInGame;

    public void AddPlayerPoints(int points)
    {
        playerPoints += points;
    }

    public void TPI()
    {
        playerPoints = 0;
        isPlayerInGame = true;
    }

    public void TeleportIn()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "TPI");
    }
}
