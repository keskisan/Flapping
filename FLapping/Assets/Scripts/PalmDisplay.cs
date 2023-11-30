
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PalmDisplay : UdonSharpBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    TextMeshProUGUI textPlayerPoints;

    VRCPlayerApi localplayer;

    private void Start()
    {
        localplayer = Networking.LocalPlayer;
    }

    private void Update()
    {
        if (gameManager.playerManager.LocalPlayerWings == null) return;

        textPlayerPoints.text = gameManager.playerManager.LocalPlayerWings.playerSpecific.playerPoints.ToString();
        transform.position = localplayer.GetBonePosition(HumanBodyBones.LeftHand);
        transform.rotation = localplayer.GetBoneRotation(HumanBodyBones.LeftHand);
    }

}
