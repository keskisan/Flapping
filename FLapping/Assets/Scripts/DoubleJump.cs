
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class DoubleJump : UdonSharpBehaviour
{
    [SerializeField]
    float jupminpulse = 5f;
    int canjump = 0;

    VRCPlayerApi localPlayer;

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
    }

    private void Update()
    {
        if (localPlayer.IsPlayerGrounded())
        {
            canjump = 1;
        }
    }

    public override void InputJump(bool value, VRC.Udon.Common.UdonInputEventArgs args)
    {
        if (value)
        {
            if (canjump >= 0)
            {
                canjump -= 1;

                Vector3 newVelocity = localPlayer.GetVelocity();
                newVelocity += new Vector3(0f, jupminpulse, 0f);

                localPlayer.SetVelocity(newVelocity);
            }
        }
    }
}
