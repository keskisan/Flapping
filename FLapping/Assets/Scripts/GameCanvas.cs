
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GameCanvas : UdonSharpBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    GameLoopCanvas gameLoopCanvas;

    [SerializeField]
    Slider calprogress, voxelProgress;


    [SerializeField]
    InputField inputRandomVecX, inputRandomVecY, inputRandomVecZ;

    [SerializeField]
    TextMeshProUGUI XtagRandom, YtagRandom, ZtagRandom;


    [SerializeField]
    InputField inputSizeVecX, inputSizeVecY, inputSizeVecZ;

    [SerializeField]
    TextMeshProUGUI XtagSize, YtagSize, ZtagSize, cuttoffValue, WorldToBigError;

    [SerializeField]
    Slider slidercutoff;

    [SerializeField]
    Button buttonGenerateNewMesh;


    private void Update()
    {
        UpdateMenuDisplay();
    }

    private void UpdateMenuDisplay()
    {
        calprogress.value = gameManager.voxels.CalProgress;
        voxelProgress.value = gameManager.voxels.VoxelProgress;

        XtagRandom.text = gameManager.randomiseNoise.x.ToString();
        YtagRandom.text = gameManager.randomiseNoise.y.ToString();
        ZtagRandom.text = gameManager.randomiseNoise.z.ToString();

        XtagSize.text = gameManager.xDir.ToString();
        YtagSize.text = gameManager.yDir.ToString();
        ZtagSize.text = gameManager.zDir.ToString();

        cuttoffValue.text = gameManager.cutoff.ToString("F2");

        if (gameManager.worldToBigToMake)
        {
            WorldToBigError.gameObject.SetActive(true);
        } else
        {
            WorldToBigError.gameObject.SetActive(false);
        }
    }

    public void GMB()
    {
        gameManager.voxels.MakeANewMesh();
    }

    public void GenerateMeshButton() //master or everyone master networked
    {
        if (gameLoopCanvas.MasterOnlyBool && Networking.IsMaster || !gameLoopCanvas.MasterOnlyBool)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "GMB");
        }
    }

    public void ResyncMap() 
    {
        gameManager.voxels.MakeANewMesh();
    }

    public void CutoffSlider()
    {
        if (gameLoopCanvas.MasterOnlyBool && Networking.IsMaster || !gameLoopCanvas.MasterOnlyBool)
        {
            gameManager.playerManager.LocalPlayerWings.Cutoff = slidercutoff.value;
        }
    }

    public void UpdateRandomVecX()
    {
        if (gameLoopCanvas.MasterOnlyBool && Networking.IsMaster || !gameLoopCanvas.MasterOnlyBool)
        {
            float tmp;
            bool isNum = float.TryParse(inputRandomVecX.text, out tmp);
            if (isNum)
            {
                gameManager.playerManager.LocalPlayerWings.RandomiseNoise =
                    new Vector3(tmp,
                    gameManager.playerManager.LocalPlayerWings.RandomiseNoise.y,
                    gameManager.playerManager.LocalPlayerWings.RandomiseNoise.z);
            }
        }      
    }

    public void UpdateRandomVecY()
    {
        if (gameLoopCanvas.MasterOnlyBool && Networking.IsMaster || !gameLoopCanvas.MasterOnlyBool)
        {
            float tmp;
            bool isNum = float.TryParse(inputRandomVecY.text, out tmp);
            if (isNum)
            {
                gameManager.playerManager.LocalPlayerWings.RandomiseNoise =
                    new Vector3(gameManager.playerManager.LocalPlayerWings.RandomiseNoise.x,
                    tmp,
                    gameManager.playerManager.LocalPlayerWings.RandomiseNoise.z);
            }
        }
    }

    public void UpdateRandomVecZ()
    {
        if (gameLoopCanvas.MasterOnlyBool && Networking.IsMaster || !gameLoopCanvas.MasterOnlyBool)
        {
            float tmp;
            bool isNum = float.TryParse(inputRandomVecZ.text, out tmp);
            if (isNum)
            {
                gameManager.playerManager.LocalPlayerWings.RandomiseNoise =
                    new Vector3(gameManager.playerManager.LocalPlayerWings.RandomiseNoise.x,
                    gameManager.playerManager.LocalPlayerWings.RandomiseNoise.y,
                    tmp);
            }
        }
    }

    public void UpdateInputSizeVecX()
    {
        if (gameLoopCanvas.MasterOnlyBool && Networking.IsMaster || !gameLoopCanvas.MasterOnlyBool)
        {
            int tmp;
            bool isNum = int.TryParse(inputSizeVecX.text, out tmp);
            if (isNum)
            {
                if (tmp > 0)
                {
                    gameManager.playerManager.LocalPlayerWings.XDir = tmp;
                }
            }
        }
    }

    public void UpdateInputSizeVecY()
    {
        if (gameLoopCanvas.MasterOnlyBool && Networking.IsMaster || !gameLoopCanvas.MasterOnlyBool)
        {
            int tmp;
            bool isNum = int.TryParse(inputSizeVecY.text, out tmp);
            if (isNum)
            {
                if (tmp > 0)
                {
                    gameManager.playerManager.LocalPlayerWings.YDir = tmp;
                }   
            }
        }    
    }

    public void UpdateInputSizeVecZ()
    {
        if (gameLoopCanvas.MasterOnlyBool && Networking.IsMaster || !gameLoopCanvas.MasterOnlyBool)
        {
            int tmp;
            bool isNum = int.TryParse(inputSizeVecZ.text, out tmp);
            if (isNum)
            {
                if (tmp > 0)
                {
                    gameManager.playerManager.LocalPlayerWings.ZDir = tmp;
                }   
            }
        }     
    }

    public void ToggleButtonVisibilityInGame()
    {
        inputRandomVecX.gameObject.SetActive(false);
        inputRandomVecY.gameObject.SetActive(false);
        inputRandomVecZ.gameObject.SetActive(false);
        inputSizeVecX.gameObject.SetActive(false);
        inputSizeVecY.gameObject.SetActive(false);
        inputSizeVecZ.gameObject.SetActive(false);
        buttonGenerateNewMesh.gameObject.SetActive(false);
    }

    public void ToggleButtonVisibilityOutGame()
    {
        inputRandomVecX.gameObject.SetActive(true);
        inputRandomVecY.gameObject.SetActive(true);
        inputRandomVecZ.gameObject.SetActive(true);
        inputSizeVecX.gameObject.SetActive(true);
        inputSizeVecY.gameObject.SetActive(true);
        inputSizeVecZ.gameObject.SetActive(true);
        buttonGenerateNewMesh.gameObject.SetActive(true);
    }

    public void ToggleButtonCountdown()
    {
        inputRandomVecX.gameObject.SetActive(false);
        inputRandomVecY.gameObject.SetActive(false);
        inputRandomVecZ.gameObject.SetActive(false);
        inputSizeVecX.gameObject.SetActive(false);
        inputSizeVecY.gameObject.SetActive(false);
        inputSizeVecZ.gameObject.SetActive(false);
        buttonGenerateNewMesh.gameObject.SetActive(false);
    }
}
