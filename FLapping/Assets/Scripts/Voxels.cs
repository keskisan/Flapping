
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class Voxels : UdonSharpBehaviour //runs for everyone
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    PerLinNoise perlinNoise;  

    int maxLength;

    [SerializeField]
    float frequency = 0.5f, ySquash = 1f;

    [SerializeField]
    MeshCollider meshCollider;

    bool[] voxelArray;

    Mesh mesh;

    Vector3[] newVertices;
    int VerticeCounter;
    Vector2[] newUV;
    int[] newTriangles;
    int TriangleCounter;

    int voxelValuesX, voxelValuesY, voxelValuesZ, voxelValuesCounter;
    int calcValuesX, calcValuesY, calcValuesZ, calcCounter;

    float voxelProgress, calProgress;

    bool hasSetVoxelValues = false, hasCalculateVoxels = false; 


    public Vector3[] flatsurfaces; //surfaces stuff can be spawned on
    [HideInInspector]
    public int flatSurfaceCounter;


    public int xDir = 10, yDir = 15, zDir = 20;
    public Vector3 randomiseNoise;
    public float cutoff = 0.3f;

    public float VoxelProgress
    {
        get
        {
            return voxelProgress;
        }
    }

    public float CalProgress
    {
        get
        {
            return calProgress;
        }
    }

    void Start()
    {
        
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
        ResetObjectToDefaults();   
    }

    public void MakeANewMesh()
    {
        //if (gameManager.playerManager.LocalPlayerWings.playerSpecific.meshDrawed == false) return; Maybe necessary but shouldnt be
        ResetObjectToDefaults();
    }

    void ResetObjectToDefaults()
    {
        gameManager.worldToBigToMake = false;

        xDir = gameManager.xDir;
        yDir = gameManager.yDir;
        zDir = gameManager.zDir;
        randomiseNoise = gameManager.randomiseNoise;
        cutoff = gameManager.cutoff;


        voxelValuesX = voxelValuesY = voxelValuesZ = voxelValuesCounter = 0;
        calcValuesX = calcValuesY = calcValuesZ = calcCounter = 0;

        flatsurfaces = new Vector3[xDir * yDir * zDir];
        flatSurfaceCounter = 0;
        maxLength = xDir * yDir * zDir;
        voxelArray = new bool[maxLength];
        
        VerticeCounter = 0;
        TriangleCounter = 0;
        mesh.Clear();

        newVertices = new Vector3[240000];
        newUV = new Vector2[240000];
        newTriangles = new int[360000];

        hasSetVoxelValues = false;
        hasCalculateVoxels = false;
        gameManager.playerManager.LocalPlayerWings.playerSpecific.meshDrawed = false;
    }

    private void Update()
    {
        if (!hasSetVoxelValues)
        {
            SetVoxelsValues();
        } else if (!hasCalculateVoxels)
        {
            CalculateVoxels();
        } else if (!gameManager.playerManager.LocalPlayerWings.playerSpecific.meshDrawed)
        {
            DrawMesh();
        }    
    }

   
    void SetVoxelsValues()
    {
        for (int i = 0; i < 30; i++) 
        {
            voxelArray[voxelValuesCounter] = perlinNoise.get3DPerlinNoise(new Vector3(voxelValuesX + randomiseNoise.x, voxelValuesY * ySquash + randomiseNoise.y, voxelValuesZ + randomiseNoise.z), frequency) < cutoff ? false : true;
            //voxelArray[voxelValuesCounter] = true;
            voxelValuesCounter++;
            voxelProgress = (float)voxelValuesCounter / (float)maxLength;
            voxelValuesZ++;
            if (voxelValuesZ == zDir)
            {
                voxelValuesZ = 0;
                voxelValuesY++;
            }
            if (voxelValuesY == yDir)
            {
                voxelValuesY = 0;
                voxelValuesX++;
            }
            if (voxelValuesX == xDir)
            {
                hasSetVoxelValues = true;
                return;
            }
        }
        



        /*int counter = 0;
        for (int x = 0; x < xDir; x++)
        {
            for (int y = 0; y < yDir; y++)
            {
                for (int z = 0; z < zDir; z++)
                {
                    voxelArray[counter] = perlinNoise.get3DPerlinNoise(new Vector3(x, y * ySquash, z), frequency) < cutoff ? false : true;
                    counter++;
                }
            }
        }*/
    }

    void CalculateVoxels()
    {
        for (int i = 0; i < 30;i++)
        {
            if (voxelArray[calcCounter]) //only add mesh if it is
            {
                CheckZPosFace(calcValuesX, calcValuesY, calcValuesZ);
                CheckZNegFace(calcValuesX, calcValuesY, calcValuesZ);
                CheckYPosFace(calcValuesX, calcValuesY, calcValuesZ);
                CheckYNegFace(calcValuesX, calcValuesY, calcValuesZ);
                CheckXPosFace(calcValuesX, calcValuesY, calcValuesZ);
                CheckXNegFace(calcValuesX, calcValuesY, calcValuesZ);
            }

            calcCounter++;
            calProgress = (float)calcCounter / (float)maxLength;
            calcValuesZ++;
            if (calcValuesZ == zDir)
            {
                calcValuesZ = 0;
                calcValuesY++;
            }
            if (calcValuesY == yDir)
            {
                calcValuesY = 0;
                calcValuesX++;
            }
            if (calcValuesX == xDir)
            {
                hasCalculateVoxels = true;
                return;
            }
        }

        

        /*int counter = 0;
        for (int x = 0; x < xDir; x++)
        {
            for (int y = 0; y < yDir; y++)
            {
                for (int z = 0; z < zDir; z++)
                {
                    if (voxelArray[counter]) //only add mesh if it is
                    {
                        CheckZPosFace(x, y, z);
                        CheckZNegFace(x, y, z);
                        CheckYPosFace(x, y, z);
                        CheckYNegFace(x, y, z);
                        CheckXPosFace(x, y, z);
                        CheckXNegFace(x, y, z);
                    }
                    counter++;
                }
            }
        }*/
    }

    

    void SetVoxelValue(int x, int y, int z, bool value)
    {
        voxelArray[z + zDir * y + zDir * yDir * x] = value;
    }

    bool GetVoxelValue(int x, int y, int z)
    {
        return voxelArray[z + zDir * y + zDir * yDir * x];
    }

    

    void AddFaceZpos(int x, int y, int z)
    {
        if (y == yDir - 1) //top so be grass
        {
            AddQuad(
                   new Vector3(x + 0.5f, y - 0.5f, z + 0.5f),
                   new Vector3(x + 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x - 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x - 0.5f, y - 0.5f, z + 0.5f),
                   new Vector2(0f, 0.33f),
                   new Vector2(0f, 0.66f),
                   new Vector2(1f, 0.66f),
                   new Vector2(1f, 0.33f)
               );
        }
        else
        {
            if (GetVoxelValue(x, y + 1, z)) //block ontop so ground
            {
                AddQuad(
                   new Vector3(x + 0.5f, y - 0.5f, z + 0.5f),
                   new Vector3(x + 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x - 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x - 0.5f, y - 0.5f, z + 0.5f),
                   new Vector2(0f, 0f),
                   new Vector2(0f, 0.33f),
                   new Vector2(1f, 0.33f),
                   new Vector2(1f, 0f)
               );
            }
            else //top so be grass
            {
                AddQuad(
                   new Vector3(x + 0.5f, y - 0.5f, z + 0.5f),
                   new Vector3(x + 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x - 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x - 0.5f, y - 0.5f, z + 0.5f),
                   new Vector2(0f, 0.33f),
                   new Vector2(0f, 0.66f),
                   new Vector2(1f, 0.66f),
                   new Vector2(1f, 0.33f)
               );
            }
        }
    }

    void AddFaceZneg(int x, int y, int z)
    {
        if (y == yDir - 1) //top so be grass
        {
            AddQuad(
                    new Vector3(x - 0.5f, y - 0.5f, z - 0.5f),
                    new Vector3(x - 0.5f, y + 0.5f, z - 0.5f),
                    new Vector3(x + 0.5f, y + 0.5f, z - 0.5f),
                    new Vector3(x + 0.5f, y - 0.5f, z -0.5f),
                   new Vector2(0f, 0.33f),
                   new Vector2(0f, 0.66f),
                   new Vector2(1f, 0.66f),
                   new Vector2(1f, 0.33f)
               );
        }
        else
        {
            if (GetVoxelValue(x, y + 1, z)) //block ontop so ground
            {
                AddQuad(
                    new Vector3(x - 0.5f, y - 0.5f, z - 0.5f),
                    new Vector3(x - 0.5f, y + 0.5f, z - 0.5f),
                    new Vector3(x + 0.5f, y + 0.5f, z - 0.5f),
                    new Vector3(x + 0.5f, y - 0.5f, z - 0.5f),
                   new Vector2(0f, 0f),
                   new Vector2(0f, 0.33f),
                   new Vector2(1f, 0.33f),
                   new Vector2(1f, 0f)
               );
            }
            else //top so be grass
            {
                AddQuad(
                    new Vector3(x - 0.5f, y - 0.5f, z - 0.5f),
                    new Vector3(x - 0.5f, y + 0.5f, z - 0.5f),
                    new Vector3(x + 0.5f, y + 0.5f, z - 0.5f),
                    new Vector3(x + 0.5f, y - 0.5f, z - 0.5f),
                   new Vector2(0f, 0.33f),
                   new Vector2(0f, 0.66f),
                   new Vector2(1f, 0.66f),
                   new Vector2(1f, 0.33f)
               );
            }
        }
    }

    void AddFaceYpos(int x, int y, int z)
    {
        flatsurfaces[flatSurfaceCounter] = transform.TransformPoint(x, y, z);
        flatSurfaceCounter++;
        AddQuad(
                   new Vector3(x - 0.5f, y + 0.5f, z - 0.5f),
                   new Vector3(x - 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x + 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x + 0.5f, y + 0.5f, z - 0.5f),
                   new Vector2(0f, 0.66f),
                   new Vector2(0f, 1f),
                   new Vector2(1f, 1f),
                   new Vector2(1f, 0.66f)
               );
    }

    void AddFaceYneg(int x, int y, int z)
    {
        AddQuad(
            new Vector3(x + 0.5f, y - 0.5f, z - 0.5f),
            new Vector3(x + 0.5f, y - 0.5f, z + 0.5f),
            new Vector3(x - 0.5f, y - 0.5f, z + 0.5f),
            new Vector3(x - 0.5f, y - 0.5f, z - 0.5f),

            new Vector2(0f, 0f),
            new Vector2(0f, 0.33f),
            new Vector2(1f, 0.33f),
            new Vector2(1f, 0f)
       );

    }

    void AddFaceXpos(int x, int y, int z)
    {
        if (y == yDir - 1) //top so be grass
        {
            AddQuad(
                new Vector3(x + 0.5f, y - 0.5f, z - 0.5f),
                new Vector3(x + 0.5f, y + 0.5f, z - 0.5f),
                new Vector3(x + 0.5f, y + 0.5f, z + 0.5f),
                new Vector3(x + 0.5f, y - 0.5f, z + 0.5f),
                new Vector2(0f, 0.33f),
                new Vector2(0f, 0.66f),
                new Vector2(1f, 0.66f),
                new Vector2(1f, 0.33f)
               );
        }
        else
        {
            if (GetVoxelValue(x, y + 1, z)) //block ontop so ground
            {
                AddQuad(
                   new Vector3(x + 0.5f, y - 0.5f, z - 0.5f),
                   new Vector3(x + 0.5f, y + 0.5f, z - 0.5f),
                   new Vector3(x + 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x + 0.5f, y - 0.5f, z + 0.5f),
                   new Vector2(0f, 0f),
                   new Vector2(0f, 0.33f),
                   new Vector2(1f, 0.33f),
                   new Vector2(1f, 0f)
               );
            }
            else //top so be grass
            {
                AddQuad(
                   new Vector3(x + 0.5f, y - 0.5f, z - 0.5f),
                   new Vector3(x + 0.5f, y + 0.5f, z - 0.5f),
                   new Vector3(x + 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x + 0.5f, y - 0.5f, z + 0.5f),
                   new Vector2(0f, 0.33f),
                   new Vector2(0f, 0.66f),
                   new Vector2(1f, 0.66f),
                   new Vector2(1f, 0.33f)
               );
            }
        }
    }

    void AddFaceXneg(int x, int y, int z)
    {
        if (y == yDir - 1) //top so be grass
        {
            AddQuad(
                   new Vector3(x - 0.5f, y - 0.5f, z + 0.5f),
                   new Vector3(x - 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x - 0.5f, y + 0.5f, z - 0.5f),
                   new Vector3(x - 0.5f, y - 0.5f, z - 0.5f),
                   new Vector2(0f, 0.33f),
                   new Vector2(0f, 0.66f),
                   new Vector2(1f, 0.66f),
                   new Vector2(1f, 0.33f)
               );
        }
        else
        {
            if (GetVoxelValue(x, y + 1, z)) //block ontop so ground
            {
                AddQuad(
                   new Vector3(x - 0.5f, y - 0.5f, z + 0.5f),
                   new Vector3(x - 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x - 0.5f, y + 0.5f, z - 0.5f),
                   new Vector3(x - 0.5f, y - 0.5f, z - 0.5f),
                   new Vector2(0f, 0f),
                   new Vector2(0f, 0.33f),
                   new Vector2(1f, 0.33f),
                   new Vector2(1f, 0f)
               );
            }
            else //top so be grass
            {
                AddQuad(
                   new Vector3(x - 0.5f, y - 0.5f, z + 0.5f),
                   new Vector3(x - 0.5f, y + 0.5f, z + 0.5f),
                   new Vector3(x - 0.5f, y + 0.5f, z - 0.5f),
                   new Vector3(x - 0.5f, y - 0.5f, z - 0.5f),
                   new Vector2(0f, 0.33f),
                   new Vector2(0f, 0.66f),
                   new Vector2(1f, 0.66f),
                   new Vector2(1f, 0.33f)
               );
            }
        }

    }


    void AddQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
    {
        if (VerticeCounter >= newVertices.Length - 5)  //failed world too big for arrays
        {
            gameManager.worldToBigToMake = true;
            gameManager.playerManager.LocalPlayerWings.playerSpecific.meshDrawed = true;
            return;
        }
        newVertices[VerticeCounter] = p1; //1
        newUV[VerticeCounter] = v1;
        VerticeCounter++;
        newVertices[VerticeCounter] = p2; //2
        newUV[VerticeCounter] = v2;
        VerticeCounter++;
        newVertices[VerticeCounter] = p3; //3
        newUV[VerticeCounter] = v3;
        VerticeCounter++;
        newVertices[VerticeCounter] = p4; //4
        newUV[VerticeCounter] = v4;
        VerticeCounter++;

        newTriangles[TriangleCounter] = VerticeCounter - 4; //1
        TriangleCounter++;
        newTriangles[TriangleCounter] = VerticeCounter - 3; //2
        TriangleCounter++;
        newTriangles[TriangleCounter] = VerticeCounter - 2; //3
        TriangleCounter++;
        newTriangles[TriangleCounter] = VerticeCounter - 2; //3 
        TriangleCounter++;
        newTriangles[TriangleCounter] = VerticeCounter - 1; //4
        TriangleCounter++;
        newTriangles[TriangleCounter] = VerticeCounter - 4; //1
        TriangleCounter++;
    }


    void CheckZPosFace(int x, int y, int z)
    {
        if (z == zDir - 1) //if at edge add face
        {
            AddFaceZpos(x, y, z);
        }
        else
        {
            if (!GetVoxelValue(x, y, z + 1)) //if not voxel next to addface
            {
                AddFaceZpos(x, y, z);
            }
        }
    }

    void CheckZNegFace(int x, int y, int z)
    {
        if (z == 0) //if at edge add face
        {
            AddFaceZneg(x, y, z);
        }
        else
        {
            if (!GetVoxelValue(x, y, z - 1)) //if not voxel next to addface
            {
                AddFaceZneg(x, y, z);
            }
        }
    }

    void CheckYPosFace(int x, int y, int z)
    {
        if (y == yDir - 1) //if at edge add face
        {
            AddFaceYpos(x, y, z);
        }
        else
        {
            if (!GetVoxelValue(x, y + 1, z)) //if not voxel next to addface
            {
                AddFaceYpos(x, y, z);
            }
        }
    }

    void CheckYNegFace(int x, int y, int z)
    {
        if (y == 0) //if at edge add face
        {
            AddFaceYneg(x, y, z);
        }
        else
        {
            if (!GetVoxelValue(x, y - 1, z)) //if not voxel next to addface
            {
                AddFaceYneg(x, y, z);
            }
        }
    }

    void CheckXPosFace(int x, int y, int z)
    {
        if (x == xDir - 1) //if at edge add face
        {
            AddFaceXpos(x, y, z);
        }
        else
        {
            if (!GetVoxelValue(x + 1, y, z)) //if not voxel next to addface
            {
                AddFaceXpos(x, y, z);
            }
        }
    }

    void CheckXNegFace(int x, int y, int z)
    {
        if (x == 0) //if at edge add face
        {
            AddFaceXneg(x, y, z);
        }
        else
        {
            if (!GetVoxelValue(x - 1, y, z)) //if not voxel next to addface
            {
                AddFaceXneg(x, y, z);
            }
        }
    }

    void DrawMesh()
    {
        Vector3[] tmpVertices = new Vector3[VerticeCounter];
        Array.Copy(newVertices, 0, tmpVertices, 0, VerticeCounter);
        Vector2[] tmpUVS = new Vector2[VerticeCounter];
        Array.Copy(newUV, 0, tmpUVS, 0, VerticeCounter);
        int[] tmpTriangles = new int[TriangleCounter];
        Array.Copy(newTriangles, 0, tmpTriangles, 0, TriangleCounter);
        mesh.vertices = tmpVertices;
        mesh.uv = tmpUVS;
        mesh.triangles = tmpTriangles;
        //mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
        gameManager.playerManager.LocalPlayerWings.playerSpecific.meshDrawed = true;
        gameManager.MeshHasBeenDrawn();
    }
}
