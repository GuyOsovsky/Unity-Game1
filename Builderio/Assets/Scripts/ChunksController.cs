using UnityEngine;
using System.Collections;
using System.Threading;

public class ChunksController : MonoBehaviour
{
    private const int SemiUpdateFPS = 3;
    public float x;
    public float z;
    public float XCompare;
    public float ZCompare;
    public GameObject[,] chunks;
    private GameObject ChunkCreator;
    private GameObject canvas;
    private GameObject camera1;
    public int Seed;
    private GameObject Sun;
    public AnimationCurve heightmultiplier;
    private bool InGame;
    private float TimeToSemiUpdate;
    private Vector3[] ChunksToCreate;
    private int ChunksToCreatePlace;
    private Queue<GameObject> DeleteQueue;
    private bool UpdateTurn;
    // Use this for initialization
    void Start()
    {
        Seed = Random.Range(-1000000, 1000000);
        chunks = new GameObject[10, 10];
        Sun = (GameObject)Resources.Load("Sun");
        ChunkCreator = (GameObject)Resources.Load("CreateChunk");
        canvas = GameObject.Find("StartCanvas");
        camera1 = GameObject.FindGameObjectWithTag("MainCamera");
        InGame = false;
        TimeToSemiUpdate = SemiUpdateFPS;
        ChunksToCreate = new Vector3[100];
        ChunksToCreatePlace = 0;
        DeleteQueue = new Queue<GameObject>();
        UpdateTurn = true;
    }
    public bool GetInGame()
    {
        return InGame;
    }
    public void GenerateNewWorld()
    {
        canvas.SetActive(false);
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        float ChunkX = 8 + (chunks.GetLength(0) / 2 - 1) * 16 + (gameObject.transform.position.x - (gameObject.transform.position.x%16));
        float ChunkZ;
        // j == z    i == x
        for (int i = 0; i < chunks.GetLength(0); i++)
        {
            ChunkZ = 8 + (chunks.GetLength(1) / 2 - 1) * 16 + (gameObject.transform.position.z - (gameObject.transform.position.z % 16));
            for (int j = 0; j < chunks.GetLength(1); j++)
            {
                chunks[i, j] = (GameObject)Instantiate(ChunkCreator, new Vector3(ChunkX, 0, ChunkZ), Quaternion.identity);
                GenerateWorld DW = chunks[i, j].GetComponent<GenerateWorld>();
                int X = (int)chunks[i, j].transform.position.x;
                int Z = (int)chunks[i, j].transform.position.z;
                //ThreadStart myThreadDelegate = delegate
                //{
                //    DW.CreateChunk(X, Y);
                //};//new ThreadStart(chunks[i, j].GetComponent<GenerateWorld>().CreateChunk((int)chunks[i,j].transform.position.x, (int)chunks[i, j].transform.position.y));
                //Thread myThread = new Thread(myThreadDelegate);
                //myThread.Start();
                chunks[i, j].GetComponent<GenerateWorld>().CreateChunk(X, Z);
                chunks[i, j].GetComponent<GenerateWorld>().DrawChunk();
                //StartCoroutine(chunks[i, j].GetComponent<GenerateWorld>().CreateChunk());
                ChunkZ -= 16;
            }
            ChunkX -= 16;
        }
        x = gameObject.transform.position.x;
        z = gameObject.transform.position.z;
        XCompare = x;
        ZCompare = z;
        Instantiate(Sun, new Vector3(0, 0, 0), Quaternion.identity);
        InGame = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (InGame)
        {
            TimeToSemiUpdate--;
            if (TimeToSemiUpdate <= 0)
            {
                SemiUpdate();
                TimeToSemiUpdate = SemiUpdateFPS;
            }
            x = gameObject.transform.position.x;
            z = gameObject.transform.position.z;


            if (z > ZCompare + 16)
            {
                Debug.Log("move z+");
                ZCompare += 16;
                for (int i = 0; i < chunks.GetLength(0); i++)
                {
                    GameObject chunk = chunks[i, chunks.GetLength(1) - 1];
                    //chunk.GetComponent<GenerateWorld>().DestroyChunk();
                    //chunk.GetComponent<GenerateWorld>().ChangeDeleteState(true);
                    DeleteQueue.Insert(chunk);
                }
                for (int j = chunks.GetLength(1) - 1; j > 0; j--)
                {
                    for (int i = 0; i < chunks.GetLength(0); i++)
                    {
                        chunks[i, j] = chunks[i, j - 1];
                    }
                }
                for (int i = 0; i < chunks.GetLength(0); i++)
                {
                    chunks[i, 0] = (GameObject)Instantiate(ChunkCreator, new Vector3(chunks[i, 0].transform.position.x, 0, chunks[i, 0].transform.position.z + 16), Quaternion.identity);
                }
            }


            if (z < ZCompare - 16)
            {
                Debug.Log("move z-");
                ZCompare -= 16;
                for (int i = 0; i < chunks.GetLength(0); i++)
                {
                    GameObject chunk = chunks[i, 0];
                    //chunk.GetComponent<GenerateWorld>().DestroyChunk();
                    //chunk.GetComponent<GenerateWorld>().ChangeDeleteState(true);
                    DeleteQueue.Insert(chunk);
                }
                for (int j = 0; j < chunks.GetLength(1) - 1; j++)
                {
                    for (int i = 0; i < chunks.GetLength(0); i++)
                    {
                        chunks[i, j] = chunks[i, j + 1];
                    }
                }
                for (int i = 0; i < chunks.GetLength(0); i++)
                {
                    chunks[i, chunks.GetLength(1) - 1] = (GameObject)Instantiate(ChunkCreator, new Vector3(chunks[i, chunks.GetLength(1) - 1].transform.position.x, 0, chunks[i, chunks.GetLength(1) - 1].transform.position.z - 16), Quaternion.identity);
                }
            }


            if (x > XCompare + 16)
            {
                Debug.Log("move x+");
                XCompare += 16;
                for (int j = 0; j < chunks.GetLength(1); j++)
                {
                    GameObject chunk = chunks[(chunks.GetLength(0) - 1), j];
                    //chunk.GetComponent<GenerateWorld>().DestroyChunk();
                    //chunk.GetComponent<GenerateWorld>().ChangeDeleteState(true);
                    DeleteQueue.Insert(chunk);
                }
                for (int i = chunks.GetLength(0) - 1; i > 0; i--)
                {
                    for (int j = 0; j < chunks.GetLength(1); j++)
                    {
                        chunks[i, j] = chunks[i - 1, j];
                    }
                }
                for (int j = 0; j < chunks.GetLength(1); j++)
                {
                    chunks[0, j] = (GameObject)Instantiate(ChunkCreator, new Vector3(chunks[0, j].transform.position.x + 16, 0, chunks[0, j].transform.position.z), Quaternion.identity);
                }
            }


            if (x < XCompare - 16)
            {
                Debug.Log("move x-");
                XCompare -= 16;
                for (int j = 0; j < chunks.GetLength(1); j++)
                {
                    GameObject chunk = chunks[0, j];
                    //chunk.GetComponent<GenerateWorld>().DestroyChunk();
                    //chunk.GetComponent<GenerateWorld>().ChangeDeleteState(true);
                    DeleteQueue.Insert(chunk);
                }
                for (int i = 0; i < chunks.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < chunks.GetLength(1); j++)
                    {
                        chunks[i, j] = chunks[i + 1, j];
                    }
                }
                for (int j = 0; j < chunks.GetLength(1); j++)
                {
                    chunks[chunks.GetLength(0) - 1, j] = (GameObject)Instantiate(ChunkCreator, new Vector3(chunks[chunks.GetLength(0) - 1, j].transform.position.x - 16, 0, chunks[chunks.GetLength(0) - 1, j].transform.position.z), Quaternion.identity);
                }
            }
        }
    }
    private void SemiUpdate()
    {
        if (UpdateTurn)
        {
            GameObject LoadChunk = null;
            for (int i = 0; i < chunks.GetLength(0); i++)
            {
                for (int j = 0; j < chunks.GetLength(1); j++)
                {
                    if (!chunks[i, j].GetComponent<GenerateWorld>().IsChunkShowen() && !chunks[i, j].GetComponent<GenerateWorld>().IsReady())
                    {
                        //chunks[i, j].GetComponent<GenerateWorld>().CreateChunk();
                        //return;
                        if (LoadChunk == null)
                        {
                            LoadChunk = chunks[i, j];
                        }
                        else if (Vector2.Distance(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z), new Vector2(LoadChunk.transform.position.x, LoadChunk.transform.position.z)) > Vector2.Distance(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z), new Vector2(chunks[i, j].transform.position.x, chunks[i, j].transform.position.z)))
                        {
                            LoadChunk = chunks[i, j];
                        }
                    }
                }
            }
            if (LoadChunk != null)
            {
                //StartCoroutine(LoadChunk.GetComponent<GenerateWorld>().CreateChunk());
                //ThreadStart myThreadDelegate = new ThreadStart(LoadChunk.GetComponent<GenerateWorld>().CreateChunk);
                GenerateWorld DW = LoadChunk.GetComponent<GenerateWorld>();
                int X = (int)(LoadChunk.transform.position.x);
                int Z = (int)(LoadChunk.transform.position.z);
                ThreadStart myThreadDelegate = delegate
                {
                    DW.CreateChunk(X, Z);
                    //DW.DrawChunk();
                };//new ThreadStart(chunks[i, j].GetComponent<GenerateWorld>().CreateChunk((int)chunks[i,j].transform.position.x, (int)chunks[i, j].transform.position.y));
                Thread myThread = new Thread(myThreadDelegate);
                myThread.Start();
                //chunks[i, j].GetComponent<GenerateWorld>().DrawChunk();
            }
            for (int i = 0; i < chunks.GetLength(0); i++)
            {
                for (int j = 0; j < chunks.GetLength(1); j++)
                {
                    if(!chunks[i, j].GetComponent<GenerateWorld>().IsChunkShowen() && chunks[i,j].GetComponent<GenerateWorld>().IsReady())
                    {
                        chunks[i, j].GetComponent<GenerateWorld>().DrawChunk();
                        break;
                    }
                }
            }
        }
        else
        {
            if (!DeleteQueue.IsEmpty())
            {
                DeleteQueue.Remove().GetComponent<GenerateWorld>().DestroyChunk();
            }
        }
        UpdateTurn = !UpdateTurn;
    }
}
