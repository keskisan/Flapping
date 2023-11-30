
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Mirror : UdonSharpBehaviour
{
    [SerializeField]
    GameObject mirror;

    public override void Interact()
    {
        mirror.SetActive(!mirror.activeSelf);
    }
}
