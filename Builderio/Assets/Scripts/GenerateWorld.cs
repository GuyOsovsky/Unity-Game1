using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
[Serializable]
public class Block
{
    //type:
    // 1 == snow
    // 2 == grass
    // 3 == dirt
    // 4 == stone
    // 5 == iron
    // 6 == gold
    // 7 == coal
    private int type;
    private bool vis;
    public Block(int T, bool V)
    {
        this.type = T;
        this.vis = V;
    }
    public void SetType(int t)
    {
        this.type = t;
    }
    public int getType()
    {
        return this.type;
    }
    public void SetVis(bool v)
    {
        this.vis = v;
    }
    public bool GetVis()
    {
        return this.vis;
    }
}
public static class Noise
{
    private static int AddHeight = 80;
    private static float DetailScale = 200.0f;
    private static int HeightScale = 90;
    private static int octaves = 3;
    private static float persistance = 0.5f;
    private static float lacunarity = 2f;
    public static int NoiseCaculate(float x, float z, AnimationCurve heightMultiplier, int seed)
    {
        float Amplitude = 1;
        float Frequency = 1;
        float NoiseHeight = 0;
        int Seed = seed;// GameObject.FindGameObjectWithTag("Player").GetComponent<ChunksController>().Seed;
        for(int i = 0; i < octaves; i++)
        {
            NoiseHeight += Mathf.PerlinNoise((x + Seed) / DetailScale * Frequency, (z + Seed) / DetailScale * Frequency);
            NoiseHeight = (NoiseHeight * Amplitude);
            Amplitude *= persistance;
            Frequency *= lacunarity;
        }
        //NoiseHeight = NoiseHeight * 2 - 1;
        NoiseHeight = heightMultiplier.Evaluate(NoiseHeight);
        NoiseHeight *= HeightScale;
        NoiseHeight += AddHeight;
        return (int)NoiseHeight;
    }
}
public class GenerateWorld : MonoBehaviour
{
    private string WorldName;
    private int width;
    private int length;
    private int height;
    private int AddHeight;
    private bool ReadyStatus;
    private string SavePath;

    private int Seed;
    private string GrassBlock;
    private string DirtBlock;
    private string StoneBlock;
    private string IronOreBlock;
    private string GoldOreBlock;
    private string CoalOreBlock;
    private string SnowBlock;

    private UnityEngine.Object GrassBlockObject;
    private UnityEngine.Object DirtBlockObject;
    private UnityEngine.Object StoneBlockObject;
    private UnityEngine.Object IronOreBlockObject;
    private UnityEngine.Object GoldOreBlockObject;
    private UnityEngine.Object CoalOreBlockObject;
    private UnityEngine.Object SnowBlockObject;

    private bool IsShowen;
    private bool IsDelete;

    public Block[,,] ChunkBlocks;

    private bool IsExited;
    private Transform ObjectsTrasform;
    private AnimationCurve heightMultiplier;

    void Awake()
    {
        WorldName = CanvasController.GetWorldName();
        width = 16;
        length = 16;
        height = 150;
        AddHeight = 80;
        ReadyStatus = false;
        SavePath = Application.persistentDataPath;

        SnowBlock = "SnowBlock";
        CoalOreBlock = "CoalOreBlock";
        GoldOreBlock = "GoldOreBlock";
        IronOreBlock = "IronOreBlock";
        StoneBlock = "StoneBlock";
        DirtBlock = "DirtBlock";
        GrassBlock = "GrassBlock";

        SnowBlockObject = Resources.Load("SnowBlock");
        CoalOreBlockObject = Resources.Load("CoalOreBlock");
        GoldOreBlockObject = Resources.Load("GoldOreBlock");
        IronOreBlockObject = Resources.Load("IronOreBlock");
        StoneBlockObject = Resources.Load("StoneBlock");
        DirtBlockObject = Resources.Load("DirtBlock");
        GrassBlockObject = Resources.Load("GrassBlock");

        Seed = GameObject.FindGameObjectWithTag("Player").GetComponent<ChunksController>().Seed;
        ChunkBlocks = new Block[width, height, length];
        IsShowen = false;
        IsDelete = false;
        IsExited = File.Exists(Application.persistentDataPath + "/Saves/" + WorldName + "/chunks/" + gameObject.transform.position.x + "/" + gameObject.transform.position.z + ".dat");
        heightMultiplier = GameObject.FindGameObjectWithTag("Player").GetComponent<ChunksController>().heightmultiplier;
        ObjectsTrasform = gameObject.transform;
        //CreateChunk();
    }

    public void DestroyChunk()
    {
        SaveChunk();
        Destroy(gameObject);
    }

    public void CreateChunk(int ChunkX, int ChunkZ)
    {
        //if (Directory.Exists(Application.persistentDataPath + "/Saves/" + WorldName + "/chunks/" + gameObject.transform.position.x))
        //{
        if (IsExited)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(SavePath + "/Saves/" + WorldName + "/chunks/" + ChunkX + "/" + ChunkZ + ".dat", FileMode.Open);
            ChunkData CD = (ChunkData)bf.Deserialize(file);
            file.Close();
            ChunkBlocks = CD.ChunkBlocks;
            //for (int x = 0; x < ChunkBlocks.GetLength(0); x++)
            //{
            //    for (int y = 0; y < ChunkBlocks.GetLength(1); y++)
            //    {
            //        for (int z = 0; z < ChunkBlocks.GetLength(2); z++)
            //        {

            //            if (CD.ChunkBlocks[x, y, z] != null)
            //            {
            //                ChunkBlocks[x, y, z] = new Block(CD.ChunkBlocks[x, y, z].getType(), CD.ChunkBlocks[x, y, z].GetVis());
            //                if (ChunkBlocks[x, y, z].GetVis())
            //                {
            //                    ChunkBlocks[x, y, z].SetVis(false);
            //                    DrawBlock(new Vector3(x, y, z));
            //                }
            //            }
            //        }
            //    }
            //}
        }
        else
        {
            int GameObjectX = ChunkX; //(int)gameObject.transform.position.x;
            int GameObjectZ = ChunkZ; //(int)gameObject.transform.position.z;
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    int y = Noise.NoiseCaculate(x + GameObjectX, z + GameObjectZ, heightMultiplier, Seed);
                    Vector3 Pos = new Vector3(x, y, z);
                    CreateBlock(ChunkX, ChunkZ, y, Pos, false);
                    ChunkBlocks[x, y, z].SetVis(true);
                    while (y > 0)
                    {
                        y--;
                        Pos = new Vector3(x, y, z);
                        CreateBlock(ChunkX, ChunkZ, y, Pos, false);
                    }
                }
            }
            System.Random r = new System.Random(DateTime.Now.Millisecond);
            //iron spawning
            for (int i = 0; i < r.Next(4, 10); i++)
            {
                int TimesToSpawn = r.Next(3, 10);
                int X = r.Next(0, 16);
                int Z = r.Next(0, 16);
                int Y = r.Next(20, AddHeight - 25);
                ChunkBlocks[X, Y, Z].SetType(5);
                for (int j = 0; j < TimesToSpawn; j++)
                {
                    int R;
                    switch (r.Next(1, 4))
                    {
                        case 1:
                            R = r.Next(0, 2);
                            if (R == 0)
                            {
                                X++;
                            }
                            else
                            {
                                X--;
                            }
                            if (X >= 16)
                            {
                                X = 15;
                            }
                            if (X < 0)
                            {
                                X = 0;
                            }
                            break;
                        case 2:
                            R = r.Next(0, 2);
                            if (R == 0)
                            {
                                Y++;
                            }
                            else
                            {
                                Y--;
                            }
                            break;
                        case 3:
                            R = r.Next(0, 2);
                            if (R == 0)
                            {
                                Z++;
                            }
                            else
                            {
                                Z--;
                            }
                            if (Z >= 16)
                            {
                                Z = 15;
                            }
                            if (Z < 0)
                            {
                                Z = 0;
                            }
                            break;
                    }
                    ChunkBlocks[X, Y, Z].SetType(5);
                }
            }
            //gold spawning
            for (int i = 0; i < r.Next(2, 6); i++)
            {
                int TimesToSpawn = r.Next(2, 6);
                int X = r.Next(0, 16);
                int Z = r.Next(0, 16);
                int Y = r.Next(10, 40);
                ChunkBlocks[X, Y, Z].SetType(6);
                for (int j = 0; j < TimesToSpawn; j++)
                {
                    int R;
                    switch (r.Next(1, 4))
                    {
                        case 1:
                            R = r.Next(0, 2);
                            if (R == 0)
                            {
                                X++;
                            }
                            else
                            {
                                X--;
                            }
                            if (X >= 16)
                            {
                                X = 15;
                            }
                            if (X < 0)
                            {
                                X = 0;
                            }
                            break;
                        case 2:
                            R = r.Next(0, 2);
                            if (R == 0)
                            {
                                Y++;
                            }
                            else
                            {
                                Y--;
                            }
                            if (Y < 0)
                            {
                                Y = 0;
                            }
                            break;
                        case 3:
                            R = r.Next(0, 2);
                            if (R == 0)
                            {
                                Z++;
                            }
                            else
                            {
                                Z--;
                            }
                            if (Z >= 16)
                            {
                                Z = 15;
                            }
                            if (Z < 0)
                            {
                                Z = 0;
                            }
                            break;
                    }
                    ChunkBlocks[X, Y, Z].SetType(6);
                }
            }
            //coal spawning
            for (int i = 0; i < r.Next(6, 17); i++)
            {
                int TimesToSpawn = r.Next(10, 21);
                int X = r.Next(0, 16);
                int Z = r.Next(0, 16);
                int Y = r.Next(0, 70);
                ChunkBlocks[X, Y, Z].SetType(7);
                for (int j = 0; j < TimesToSpawn; j++)
                {
                    int R;
                    switch (r.Next(1, 4))
                    {
                        case 1:
                            R = r.Next(0, 2);
                            if (R == 0)
                            {
                                X++;
                            }
                            else
                            {
                                X--;
                            }
                            if (X >= 16)
                            {
                                X = 15;
                            }
                            if (X < 0)
                            {
                                X = 0;
                            }
                            break;
                        case 2:
                            R = r.Next(0, 2);
                            if (R == 0)
                            {
                                Y++;
                            }
                            else
                            {
                                Y--;
                            }
                            if (Y < 0)
                            {
                                Y = 0;
                            }
                            break;
                        case 3:
                            R = r.Next(0, 2);
                            if (R == 0)
                            {
                                Z++;
                            }
                            else
                            {
                                Z--;
                            }
                            if (Z >= 16)
                            {
                                Z = 15;
                            }
                            if (Z < 0)
                            {
                                Z = 0;
                            }
                            break;
                    }
                    ChunkBlocks[X, Y, Z].SetType(7);
                }
            }
            //IsShowen = true;
        }
        ReadyStatus = true;
        //yield return new WaitForSeconds(0);
    }

    public void DrawChunk()
    {
        for (int x = 0; x < ChunkBlocks.GetLength(0); x++)
        {
            for (int z = 0; z < ChunkBlocks.GetLength(2); z++)
            {
                for (int y = Noise.NoiseCaculate(x + (int)gameObject.transform.position.x, z + (int)gameObject.transform.position.z, GameObject.FindGameObjectWithTag("Player").GetComponent<ChunksController>().heightmultiplier, Seed); y >= 0; y--)
                {
                    if (ChunkBlocks[x, y, z] != null && (CheckBlock(x, y, z) || ChunkBlocks[x,y,z].GetVis()))
                    {
                        //ChunkBlocks[x, y, z].SetType(ChunkBlocks[x, y + 1, z].getType());
                        if (ChunkBlocks[x, y, z].GetVis())
                            ChunkBlocks[x, y, z].SetVis(false);
                        DrawBlock(new Vector3(x, y, z));
                    }
                }
            }
        }
        IsShowen = true;
    }
    private bool CheckBlock(int x, int y, int z)
    {
        if (Noise.NoiseCaculate(x + gameObject.transform.position.x + 1, z + gameObject.transform.position.z, GameObject.FindGameObjectWithTag("Player").GetComponent<ChunksController>().heightmultiplier, Seed) < y)
            return true;
        if (Noise.NoiseCaculate(x + gameObject.transform.position.x - 1, z + gameObject.transform.position.z, GameObject.FindGameObjectWithTag("Player").GetComponent<ChunksController>().heightmultiplier, Seed) < y)
            return true;
        if (Noise.NoiseCaculate(x + gameObject.transform.position.x, z + gameObject.transform.position.z + 1, GameObject.FindGameObjectWithTag("Player").GetComponent<ChunksController>().heightmultiplier, Seed) < y)
            return true;
        if (Noise.NoiseCaculate(x + gameObject.transform.position.x, z + gameObject.transform.position.z - 1, GameObject.FindGameObjectWithTag("Player").GetComponent<ChunksController>().heightmultiplier, Seed) < y)
            return true;
        return false;
    }
    //private bool CheckBlock(Vector3 BlockPos)
    //{
    //    for(int x = -1; x < 2; x++)
    //    {
    //        if(x != 0)
    //        {
    //            Vector3 newChunk = gameObject.transform.position;
    //            int XAdd = 0;
    //            if (BlockPos.x + x == -1)
    //            {
    //                newChunk.x -= 16;
    //                XAdd = 16;
    //            }
    //            else if (BlockPos.x + x == 16)
    //            {
    //                newChunk.x += 16;
    //                XAdd = -16;
    //            }
    //            GameObject BlockChunk = gameObject;
    //            GameObject[] AllChunks = GameObject.FindGameObjectsWithTag("Chunk");
    //            for (int p = 0; p < AllChunks.GetLength(0); p++)
    //            {
    //                if (AllChunks[p].transform.position == newChunk)
    //                {
    //                    BlockChunk = AllChunks[p];
    //                    break;
    //                }
    //            }
    //            if (!BlockChunk.GetComponent<GenerateWorld>().IsBlock(new Vector3(BlockPos.x + XAdd + x, BlockPos.y, BlockPos.z)))
    //                return true;
    //        }
    //    }
    //    for (int z = -1; z < 2; z++)
    //    {
    //        if (z != 0)
    //        {
    //            Vector3 newChunk = gameObject.transform.position;
    //            int ZAdd = 0;
    //            if (BlockPos.x + z == -1)
    //            {
    //                newChunk.z -= 16;
    //                ZAdd = 16;
    //            }
    //            else if (BlockPos.x + z == 16)
    //            {
    //                newChunk.z += 16;
    //                ZAdd = -16;
    //            }
    //            GameObject BlockChunk = gameObject;
    //            GameObject[] AllChunks = GameObject.FindGameObjectsWithTag("Chunk");
    //            for (int p = 0; p < AllChunks.GetLength(0); p++)
    //            {
    //                if (AllChunks[p].transform.position == newChunk)
    //                {
    //                    BlockChunk = AllChunks[p];
    //                    break;
    //                }
    //            }
    //            if (!BlockChunk.GetComponent<GenerateWorld>().IsBlock(new Vector3(BlockPos.x, BlockPos.y, BlockPos.z + ZAdd + z)))
    //                return true;
    //        }
    //    }
    //    if (!IsBlock(new Vector3(BlockPos.x, BlockPos.y + 1, BlockPos.z)) || !IsBlock(new Vector3(BlockPos.x, BlockPos.y - 1, BlockPos.z)))
    //        return true;
    //    return false;
    //}
    public void CreateBlock(Vector3 BlockPos, int ID)
    {
        ChunkBlocks[(int)BlockPos.x, (int)BlockPos.y, (int)BlockPos.z] = new Block(ID, false);
        DrawBlock(BlockPos);
    }
    private void CreateBlock(int ChunkX, int ChunkZ, int y, Vector3 BlockPos, bool Create)
    {
        //AnimationCurve heightMultiplier = GameObject.FindGameObjectWithTag("Player").GetComponent<ChunksController>().heightmultiplier;
        if (y == Noise.NoiseCaculate(BlockPos.x + ChunkX, BlockPos.z + ChunkZ, heightMultiplier, Seed))
        {
            if (y > AddHeight + 20)
            {
                if (Create)
                {
                    GameObject block = (GameObject)Instantiate(SnowBlockObject, new Vector3(BlockPos.x + ChunkX, BlockPos.y, BlockPos.z + ChunkZ), Quaternion.identity);
                    block.transform.SetParent(ObjectsTrasform);
                    block.transform.name = SnowBlock;
                }
                ChunkBlocks[(int)BlockPos.x, y, (int)BlockPos.z] = new Block(1, Create);
            }
            else
            {
                if (Create)
                {
                    GameObject block = (GameObject)Instantiate(GrassBlockObject, new Vector3(BlockPos.x + ChunkX, BlockPos.y, BlockPos.z + ChunkZ), Quaternion.identity);
                    block.transform.SetParent(ObjectsTrasform);
                    block.transform.name = GrassBlock;
                }
                ChunkBlocks[(int)BlockPos.x, y, (int)BlockPos.z] = new Block(2, Create);
            }
        }
        else
        {
            if (y > Noise.NoiseCaculate(BlockPos.x + ChunkX, BlockPos.z + ChunkZ, heightMultiplier, Seed) - 5)
            {
                if (Create)
                {
                    GameObject block = (GameObject)Instantiate(DirtBlockObject, new Vector3(0, 0, 0), Quaternion.identity);
                    block.transform.SetParent(ObjectsTrasform);
                    block.transform.name = DirtBlock;
                }
                ChunkBlocks[(int)BlockPos.x, y, (int)BlockPos.z] = new Block(3, Create);
            }
            else
            {
                if (Create)
                {
                    GameObject block = (GameObject)Instantiate(StoneBlockObject, new Vector3(BlockPos.x + ChunkX, BlockPos.y, BlockPos.z + ChunkZ), Quaternion.identity);
                    block.transform.SetParent(ObjectsTrasform);
                    block.transform.name = StoneBlock;
                }
                ChunkBlocks[(int)BlockPos.x, y, (int)BlockPos.z] = new Block(4, Create);
            }
        }
    }
    public void DrawBlock(Vector3 BlockPos)
    {
        if (ChunkBlocks[(int)(BlockPos.x), (int)BlockPos.y, (int)(BlockPos.z)] == null) return;
        if (!ChunkBlocks[(int)(BlockPos.x), (int)BlockPos.y, (int)(BlockPos.z)].GetVis())
        {
            ChunkBlocks[(int)(BlockPos.x), (int)BlockPos.y, (int)(BlockPos.z)].SetVis(true);
            Vector3 position = new Vector3(BlockPos.x + gameObject.transform.position.x, BlockPos.y, BlockPos.z + gameObject.transform.position.z);
            GameObject block;
            switch (ChunkBlocks[(int)(BlockPos.x), (int)BlockPos.y, (int)(BlockPos.z)].getType())
            {
                case 1:
                    block = (GameObject)Instantiate(SnowBlockObject, new Vector3(BlockPos.x + gameObject.transform.position.x, BlockPos.y, BlockPos.z + gameObject.transform.position.z), Quaternion.identity);
                    block.transform.SetParent(gameObject.transform);
                    block.transform.name = SnowBlock;
                    break;
                case 2:
                    block = (GameObject)Instantiate(GrassBlockObject, new Vector3(BlockPos.x + gameObject.transform.position.x, BlockPos.y, BlockPos.z + gameObject.transform.position.z), Quaternion.identity);
                    block.transform.SetParent(gameObject.transform);
                    block.transform.name = GrassBlock;
                    break;
                case 3:
                    block = (GameObject)Instantiate(DirtBlockObject, new Vector3(BlockPos.x + gameObject.transform.position.x, BlockPos.y, BlockPos.z + gameObject.transform.position.z), Quaternion.identity);
                    block.transform.SetParent(gameObject.transform);
                    block.transform.name = DirtBlock;
                    break;
                case 4:
                    block = (GameObject)Instantiate(StoneBlockObject, new Vector3(BlockPos.x + gameObject.transform.position.x, BlockPos.y, BlockPos.z + gameObject.transform.position.z), Quaternion.identity);
                    block.transform.SetParent(gameObject.transform);
                    block.transform.name = StoneBlock;
                    break;
                case 5:
                    block = (GameObject)Instantiate(IronOreBlockObject, new Vector3(BlockPos.x + gameObject.transform.position.x, BlockPos.y, BlockPos.z + gameObject.transform.position.z), Quaternion.identity);
                    block.transform.SetParent(gameObject.transform);
                    block.transform.name = IronOreBlock;
                    break;
                case 6:
                    block = (GameObject)Instantiate(GoldOreBlockObject, new Vector3(BlockPos.x + gameObject.transform.position.x, BlockPos.y, BlockPos.z + gameObject.transform.position.z), Quaternion.identity);
                    block.transform.SetParent(gameObject.transform);
                    block.transform.name = GoldOreBlock;
                    break;
                case 7:
                    block = (GameObject)Instantiate(CoalOreBlockObject, new Vector3(BlockPos.x + gameObject.transform.position.x, BlockPos.y, BlockPos.z + gameObject.transform.position.z), Quaternion.identity);
                    block.transform.SetParent(gameObject.transform);
                    block.transform.name = CoalOreBlock;
                    break;
                default:
                    ChunkBlocks[(int)(BlockPos.x), (int)BlockPos.y, (int)(BlockPos.x)].SetVis(false);
                    break;
            }
        }
    }
    public bool IsBlock(Vector3 BlockPos)
    {
        if (ChunkBlocks[(int)BlockPos.x, (int)BlockPos.y, (int)BlockPos.z] == null)
            return false;
        return true;
    }
    public void SaveChunk()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves/" + WorldName + "/chunks/" + gameObject.transform.position.x))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves/" + WorldName + "/chunks/" + gameObject.transform.position.x);
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Saves/" + WorldName + "/chunks/" + gameObject.transform.position.x + "/" + gameObject.transform.position.z + ".dat");
        ChunkData CD = new ChunkData();
        CD.ChunkBlocks = ChunkBlocks;
        bf.Serialize(file, CD);
        file.Close();
    }
    public bool IsChunkShowen()
    {
        return this.IsShowen;
    }
    public bool IsChunkDelete()
    {
        return this.IsDelete;
    }
    public void ChangeDeleteState(bool Del)
    {
        this.IsDelete = Del;
    }
    public bool IsReady()
    {
        return this.ReadyStatus;
    }
}
[Serializable]
class ChunkData
{
    public Block[,,] ChunkBlocks;
}
