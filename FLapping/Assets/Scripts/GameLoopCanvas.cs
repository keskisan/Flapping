
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameLoopCanvas : UdonSharpBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    GameCanvas gameCanvas;

    [SerializeField]
    TextMeshProUGUI textCountdown, textOutcome, textMasterName, textGameStatus, textWaitingForMapError;

    [SerializeField]
    Text textButtonMasterOrPlayers;

    [SerializeField]
    Button buttonStart, buttonAbort, buttonMasterOnly;

    [UdonSynced, FieldChangeCallback(nameof(MasterOnlyBool))]
    private bool _masterOnlyBool;
    public bool MasterOnlyBool
    {
        set
        {
            if (value)
            {
                textButtonMasterOrPlayers.text = "Everyone";
            }
            else
            {
                textButtonMasterOrPlayers.text = "Master Only";
            }

            _masterOnlyBool = value;
        }
        get => _masterOnlyBool;
    }

    private void Start()
    {
        textMasterName.text = Networking.GetOwner(gameObject).displayName;
        textWaitingForMapError.gameObject.SetActive(false);
    }

    private void ToggleButtonVisibilityInGame()
    {
        buttonAbort.gameObject.SetActive(true);
        buttonStart.gameObject.SetActive(false);
        gameCanvas.ToggleButtonVisibilityInGame();
    }

    private void ToggleButtonVisibilityOutGame()
    {
        buttonAbort.gameObject.SetActive(false);
        buttonStart.gameObject.SetActive(true);
        gameCanvas.ToggleButtonVisibilityOutGame();
    }

    public void ToggleButtonCountdown()
    {
        buttonAbort.gameObject.SetActive(false);
        buttonStart.gameObject.SetActive(false);
        gameCanvas.ToggleButtonCountdown();
    }

    public void CER()
    {
        textWaitingForMapError.gameObject.SetActive(false);
    }

    public void ERR()
    {
        textWaitingForMapError.gameObject.SetActive(true);
    }

    public void STR() //start master only networked
    {
        if (gameManager.Status == GAMESTATUS.WON || gameManager.Status == GAMESTATUS.ABORTED || gameManager.Status == GAMESTATUS.NOTPLAYING)
        {
            if (gameManager.playerManager.HasEveryOnesMeshBeenDrawn())
            {
                gameManager.StartNewGame();
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "CER");
            }
            else
            { 
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "ERR");
            }
        }
    }

    public void StartButton() //master or everyone master networked
    {
        if (MasterOnlyBool && Networking.IsMaster || !MasterOnlyBool)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "STR");
        }
    }


    public void ABT() //start master only networked
    {
        if (gameManager.Status == GAMESTATUS.PLAYING)
        {
            gameManager.AbortGame();
        }
    }

    public void AbortButton() //master or everyone master networked
    {
        if (MasterOnlyBool && Networking.IsMaster || !MasterOnlyBool)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "ABT");
        }
    }

    public void MasterOnlyButton() //master or everyone master networked
    {
        if (Networking.IsMaster)
        {
            MasterOnlyBool = !MasterOnlyBool;
            RequestSerialization();
        }
    }

    public void DisplayCountdown(string value)
    {
        textCountdown.text = value;
    }

    public void UpdateMenuItemVisibility(GAMESTATUS status)
    {
        switch (status)
        {
            case GAMESTATUS.NOTPLAYING:
                {

                    ToggleButtonVisibilityOutGame();
                    textGameStatus.text = "Not Playing";
                    textOutcome.text = "Not Playing";
                    break;
                }
            case GAMESTATUS.ABORTED:
                {
                    ToggleButtonVisibilityOutGame();
                    textGameStatus.text = "Not Playing";
                    textOutcome.text = "Aborted";
                    break;
                }
            case GAMESTATUS.WON:
                {
                    ToggleButtonVisibilityOutGame();
                    textGameStatus.text = "Not Playing";
                    //textOutcome.text = "Won player most points"; see UpdateWinText below
                    break;
                }
            case GAMESTATUS.PLAYING:
                {
                    ToggleButtonVisibilityInGame();
                    textGameStatus.text = "Playing";
                    textOutcome.text = "";
                    break;
                }
            default:
                {
                    Debug.Log("Menumanager UpdateMenuItem... invalid gamestate");
                    break;
                }
        }
    }

    public void UpdateWinText(string value)
    {
        textOutcome.text = value;
    }
}
