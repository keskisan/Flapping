
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PlayerPointsDisplay : UdonSharpBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    TextMeshProUGUI textNames, textPoints;

    public void UpdatePointsDisplay()
    {
        textNames.text = textPoints.text = "";

        for (int i = 0; i < gameManager.playerManager.players.Length; i++)
        {
            if (gameManager.playerManager.IsWingsAssigned(gameManager.playerManager.players[i]))
            {
                textNames.text += Networking.GetOwner(gameManager.playerManager.players[i].gameObject).displayName + " <br>";
                textPoints.text += gameManager.playerManager.players[i].playerSpecific.playerPoints.ToString() + " <br>";
            }
        }

        



    }
}
