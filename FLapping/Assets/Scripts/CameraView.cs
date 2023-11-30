
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class CameraView : UdonSharpBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    TextMeshProUGUI playerName;

    [SerializeField]
    Text[] buttonNamesArray;

    [SerializeField]
    Button buttonDisplay, buttonPrev, buttonNext;

    [SerializeField]
    Button[] playerButtons;

    [SerializeField]
    GameObject playerCamera;

    [SerializeField]
    GameObject playerCameraDisplay;

    Player cameraPlayer;
    int cameraPlayerPositionInArray;
    VRCPlayerApi playerApi;

    void Start()
    {
        playerCameraDisplay.SetActive(false);
        playerCamera.SetActive(false);
        cameraPlayer = gameManager.playerManager.players[0]; //master
        cameraPlayerPositionInArray = 0;
    }

    public void ButtonDisplay()
    {
        playerCameraDisplay.SetActive(!playerCameraDisplay.activeSelf);
        playerCamera.SetActive(playerCameraDisplay.activeSelf);
    }

    private void Update()
    {
        UpdateCameraPosition();
        ForAllPlayersInGameUpdateButtonClickable();
    }

    int buttonCounter;
    private void ForAllPlayersInGameUpdateButtonClickable()
    {
        buttonCounter++;
        if (buttonCounter >= buttonNamesArray.Length) buttonCounter = 0;


        if (gameManager.playerManager.players[buttonCounter].playerSpecific.isPlayerInGame)
        {
            playerButtons[buttonCounter].gameObject.SetActive(true);
            buttonNamesArray[buttonCounter].text = Networking.GetOwner(gameManager.playerManager.players[buttonCounter].gameObject).playerId + " " + Networking.GetOwner(gameManager.playerManager.players[buttonCounter].gameObject).displayName;
        }
        else
        {
            playerButtons[buttonCounter].gameObject.SetActive(false);
        }
    }

    private void UpdateCameraPosition()
    {
        if (cameraPlayer == null) return;

        if (!playerCameraDisplay.activeSelf) return;

        if (gameManager.Status == GAMESTATUS.PLAYING) NoGame(); //not playing


        if (!cameraPlayer.playerSpecific.isPlayerInGame)
        {
            PickANewPlayer();
            return;
        }

        playerApi = Networking.GetOwner(cameraPlayer.gameObject);
        playerCamera.transform.position = playerApi.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        playerCamera.transform.rotation = playerApi.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
        playerName.text = playerApi.displayName;

    }

    private void PickANewPlayer()
    {
        cameraPlayer = findNextPlayer(cameraPlayerPositionInArray);
    }

    private void NoGame()
    {
        playerCamera.transform.position = Vector3.zero;
        playerCamera.transform.rotation = Quaternion.identity;
        playerName.text = "No game playing";
    }

    private Player findNextPlayer(int startposition) //everyone local
    {
        for (int i = startposition + 1; i < gameManager.playerManager.players.Length; i++)
        {
            if (gameManager.playerManager.players[i].playerSpecific.isPlayerInGame) //only show players thats ingame ingame
            {
                cameraPlayerPositionInArray = i;
                return gameManager.playerManager.players[i];
            }
        }
        for (int i = 0; i < startposition + 1; i++)
        {
            if (gameManager.playerManager.players[i].playerSpecific.isPlayerInGame) //only show players thats ingame ingame
            {
                cameraPlayerPositionInArray = i;
                return gameManager.playerManager.players[i];
            }
        }
        return null; //nothing found
    }


    private Player findPreviousPlayer(int startposition) //everyone local
    {
        for (int i = startposition - 1; i >= 0; i--)
        {
            if (gameManager.playerManager.players[i].playerSpecific.isPlayerInGame) //only show players thats ingame ingame
            {
                cameraPlayerPositionInArray = i;
                return gameManager.playerManager.players[i];
            }
        }
        for (int i = gameManager.playerManager.players.Length - 1; i >= startposition; i--)
        {
            if (gameManager.playerManager.players[i].playerSpecific.isPlayerInGame) //only show players thats ingame ingame
            {
                cameraPlayerPositionInArray = i;
                return gameManager.playerManager.players[i];
            }
        }
        return null; //nothing found
    }


    public void ViewNextPlayer() //everyone local
    {
        cameraPlayer = findNextPlayer(cameraPlayerPositionInArray);
    }

    public void ViewPreviousPlayer() //everyone local
    {
        cameraPlayer = findPreviousPlayer(cameraPlayerPositionInArray);
    }

    public void ButtonPlayer1()
    {
        cameraPlayer = gameManager.playerManager.players[0];
        cameraPlayerPositionInArray = 0;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }
    public void ButtonPlayer2()
    {
        cameraPlayer = gameManager.playerManager.players[1];
        cameraPlayerPositionInArray = 1;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }
    public void ButtonPlayer3()
    {
        cameraPlayer = gameManager.playerManager.players[2];
        cameraPlayerPositionInArray = 2;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }
    public void ButtonPlayer4()
    {
        cameraPlayer = gameManager.playerManager.players[3];
        cameraPlayerPositionInArray = 3;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }
    public void ButtonPlayer5()
    {
        cameraPlayer = gameManager.playerManager.players[4];
        cameraPlayerPositionInArray = 4;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }

    public void ButtonPlayer6()
    {
        cameraPlayer = gameManager.playerManager.players[5];
        cameraPlayerPositionInArray = 5;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }
    public void ButtonPlayer7()
    {
        cameraPlayer = gameManager.playerManager.players[6];
        cameraPlayerPositionInArray = 6;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }
    public void ButtonPlayer8()
    {
        cameraPlayer = gameManager.playerManager.players[7];
        cameraPlayerPositionInArray = 7;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }
    public void ButtonPlayer9()
    {
        cameraPlayer = gameManager.playerManager.players[8];
        cameraPlayerPositionInArray = 8;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }
    public void ButtonPlayer10()
    {
        cameraPlayer = gameManager.playerManager.players[9];
        cameraPlayerPositionInArray = 9;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }
    public void ButtonPlayer11()
    {
        cameraPlayer = gameManager.playerManager.players[10];
        cameraPlayerPositionInArray = 10;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }

    public void ButtonPlayer12()
    {
        cameraPlayer = gameManager.playerManager.players[11];
        cameraPlayerPositionInArray = 11;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }

    public void ButtonPlayer13()
    {
        cameraPlayer = gameManager.playerManager.players[12];
        cameraPlayerPositionInArray = 12;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }

    public void ButtonPlayer14()
    {
        cameraPlayer = gameManager.playerManager.players[13];
        cameraPlayerPositionInArray = 13;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }

    public void ButtonPlayer15()
    {
        cameraPlayer = gameManager.playerManager.players[14];
        cameraPlayerPositionInArray = 14;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }

    public void ButtonPlayer16()
    {
        cameraPlayer = gameManager.playerManager.players[15];
        cameraPlayerPositionInArray = 15;
        playerCameraDisplay.SetActive(true);
        playerCamera.SetActive(true);
    }
}
