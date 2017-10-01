using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class inventorySlot
{
    int id;
    int numberOfItem;
    public inventorySlot()
    {
        this.id = 0;
        this.numberOfItem = 0;
    }
    public void AddNewItem (int ID)
    {
        this.id = ID;
        this.numberOfItem = 1;
    }
    public void AddNewItem (int ID, int NumberOfItems)
    {
        this.id = ID;
        this.numberOfItem = NumberOfItems;
    }
    public void DeleteItem()
    {
        this.id = 0;
        this.numberOfItem = 0;
    }
    public void Add1()
    {
        this.numberOfItem++;
    }
    public void Substruct1()
    {
        this.numberOfItem--;
    }
    public int GetID()
    {
        return this.id;
    }
    public int GetMumberOfItems()
    {
        return this.numberOfItem;
    }
}
public class GameLogic : MonoBehaviour
{
    private int InventoryI;
    private int InventoryJ;
    private bool Active = false;
    string EmptyImage = "Empty";
    int HandNumber;
    inventorySlot[,] inventory;
    private Sprite[] ItemsGUI;
    private GameObject[] invectoryItemsUI;
    private bool cursorLock;
    private GameObject CursorUI;
    private bool CursorUIBusy;
    private int CursorID;
    private int CursorNumberOfItems;
    private GameObject CraftingPanel;
    private GameObject InventoryPanel;

    IEnumerator pickUpItems()
    {
        while (true)
        {
            GameObject[] Entitys = GameObject.FindGameObjectsWithTag("Entity");
            for (int p = 0; p < Entitys.GetLength(0); p++)
            {
                if (Vector3.Distance(gameObject.transform.position, Entitys[p].transform.position) <= 3)
                {
                    bool isBreak = false;
                    int I = -1;
                    int J = -1;
                    for (int j = inventory.GetLength(1) - 1; j >= 0; j--)
                    {

                        for (int i = 0; i < inventory.GetLength(0); i++)
                        {
                            if (inventory[i, j].GetID() == Entitys[p].GetComponent<PickUpItem>().GetID() && inventory[i,j].GetMumberOfItems() < 64)
                            {
                                if (inventory[i, j].GetMumberOfItems() <= 99)
                                {
                                    Entitys[p].GetComponent<PickUpItem>().StartPickUp();
                                    Entitys[p].transform.tag = "Untagged";
                                    inventory[i, j].Add1();
                                    HandUpdate();
                                    isBreak = true;
                                    break;
                                }
                            }
                            else if(inventory[i, j].GetID() == 0 && I == -1 && J == -1)
                            {
                                I = i;
                                J = j;
                            }
                        }
                        if (isBreak)
                            break;
                    }
                    if (!isBreak)
                    {
                        if (I != -1 && J != -1)
                        {
                            Entitys[p].GetComponent<PickUpItem>().StartPickUp();
                            Entitys[p].transform.tag = "Untagged";
                            inventory[I, J].AddNewItem(Entitys[p].GetComponent<PickUpItem>().GetID());
                            HandUpdate();
                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void HandUpdate()
    {
        for(int i = 0; i < 10; i++)
        {
            GameObject hand = GameObject.Find("Hand Items").transform.GetChild(i).gameObject;
            if (inventory[9 - i, inventory.GetLength(1) - 1].GetID() != 0)
            {
                Sprite Pic = ItemsGUI[inventory[ 9 - i, inventory.GetLength(1) - 1].GetID() - 1];//(Sprite)Resources.Load(Path + (inventory[i - 1, inventory.GetLength(1) - 1].GetID() - 1), typeof(Sprite));//"GrassBlockGUI", typeof(Sprite));
                hand.GetComponent<Image>().sprite = Pic;
                hand.transform.GetChild(0).gameObject.GetComponent<Text>().text = inventory[ 9 - i, inventory.GetLength(1) - 1].GetMumberOfItems() + "";
            }
            else
            {
                hand.GetComponent<Image>().sprite = (Sprite)Resources.Load(EmptyImage, typeof(Sprite));
                hand.transform.GetChild(0).gameObject.GetComponent<Text>().text = "0";
            }
        }
        for (int j = inventory.GetLength(1) - 1; j > 0; j--)
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject hand = invectoryItemsUI[j - 1].transform.GetChild(i).gameObject;//GameObject.Find("Inventory Items " + (j)).transform.GetChild(i).gameObject;
                if (inventory[9 - i, j - 1].GetID() != 0)
                {
                    Sprite Pic = ItemsGUI[inventory[9 - i, j - 1].GetID() - 1];
                    hand.GetComponent<Image>().sprite = Pic;
                    hand.transform.GetChild(0).gameObject.GetComponent<Text>().text = inventory[9 - i, j - 1].GetMumberOfItems() + "";
                }
                else
                {
                    hand.GetComponent<Image>().sprite = (Sprite)Resources.Load(EmptyImage, typeof(Sprite));
                    hand.transform.GetChild(0).gameObject.GetComponent<Text>().text = "0";
                }
            }
        }
    }
    private void HandMarkerUpdate()
    {
        GameObject Marker = GameObject.Find("HandMarkerUI");
        Marker.GetComponent<RectTransform>().localPosition = new Vector3((-180 + (HandNumber - 1) * 40), 25 ,0);
    }
    
    // Use this for initialization
    void Start()
    {
        StartCoroutine(pickUpItems());
        AnimationCurve heightMultiplier = gameObject.GetComponent<ChunksController>().heightmultiplier;
        int X = Random.Range(-100, 100);
        int Z = Random.Range(-100, 100);
        int Y = Noise.NoiseCaculate(X, Z, heightMultiplier, GameObject.FindGameObjectWithTag("Player").GetComponent<ChunksController>().Seed) + 9;
        gameObject.transform.position = new Vector3(X, Y, Z);
        inventory = new inventorySlot[10, 4];
        for(int i = 0; i < inventory.GetLength(0); i++)
        {
            for (int j = 0; j < inventory.GetLength(1); j++)
            {
                inventory[i, j] = new inventorySlot();
            }
        }
        ItemsGUI = Resources.LoadAll<Sprite>("inventory items");
        HandNumber = 1;
        InventoryPanel = GameObject.Find("inventory Panel");
        invectoryItemsUI = new GameObject[inventory.GetLength(1) - 1];
        for (int j = inventory.GetLength(1) - 1; j > 0; j--)
        {
            invectoryItemsUI[j - 1] = GameObject.Find("Inventory Items " + (j));
        }
        InventoryPanel.SetActive(false);
        cursorLock = false;
        CursorUI = GameObject.Find("CursorUI");
        CursorUI.SetActive(false);
        CursorUIBusy = false;
        CursorID = 0;
        CursorNumberOfItems = 0;
        CraftingPanel = GameObject.Find("Crafting Panel");
        ShowPanel(0);
        CraftingPanel.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        CursorUI.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        CursorUI.SetActive(CursorUIBusy);
        if (Input.GetKeyDown(KeyCode.E) && !CraftingPanel.activeInHierarchy)
        {
            CraftingPanel.SetActive(false);
            InventoryPanel.SetActive(!InventoryPanel.activeInHierarchy);
            cursorLock = InventoryPanel.activeInHierarchy;
        }
        if(Input.GetKeyDown(KeyCode.Q) && !InventoryPanel.activeInHierarchy)
        {
            InventoryPanel.SetActive(false);
            CraftingPanel.SetActive(!CraftingPanel.activeInHierarchy);
            cursorLock = CraftingPanel.activeInHierarchy;
            ShowPanel(0);
        }
        
            if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            {
                HandNumber++;
                if (HandNumber == 11)
                    HandNumber = 1;
                HandMarkerUpdate();
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            {
                HandNumber--;
                if (HandNumber == 0)
                    HandNumber = 10;
                HandMarkerUpdate();
                Debug.Log(HandNumber + "");
            }
        if (!CraftingPanel.activeInHierarchy && !InventoryPanel.activeInHierarchy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                int Mask = 1 << 8;
                Mask = ~Mask;
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
                if (Physics.Raycast(ray, out hit, 7f, Mask))
                {
                    if (hit.transform.gameObject.tag != "Block") return;
                    Vector3 BlockPos = hit.transform.position;
                    if (BlockPos.y == 0) return;
                    GameObject chunk = hit.transform.parent.gameObject;
                    int IndexZ = (int)hit.transform.localPosition.z;
                    int IndexX = (int)hit.transform.localPosition.x;
                    chunk.GetComponent<GenerateWorld>().ChunkBlocks[IndexX, (int)hit.transform.position.y, IndexZ] = null;
                    if (hit.transform.name != "CoalOreBlock" && hit.transform.name != "IronOreBlock" && hit.transform.name != "GoldOreBlock")
                    {
                        GameObject Block = (GameObject)Instantiate(Resources.Load(hit.transform.name + "Entity"), hit.transform.position, Quaternion.identity);
                        Block.transform.name = hit.transform.name + "Entity";
                        Block.GetComponent<PickUpItem>().ChangeID(NameToId(Block.transform.name));
                        Block.GetComponent<Rigidbody>().AddForce(Random.Range(-4, 4) * 10, Random.Range(8, 11) * 12, Random.Range(-4, 4) * 10);
                    }
                    else
                    {
                        for (int j = 0; j < Random.Range(3, 7); j++)
                        {
                            GameObject Block = (GameObject)Instantiate(Resources.Load(hit.transform.name + "Entity"), hit.transform.position, Quaternion.identity);
                            Block.transform.name = hit.transform.name + "Entity";
                            Block.GetComponent<PickUpItem>().ChangeID(NameToId(Block.transform.name));
                            Block.GetComponent<Rigidbody>().AddForce(Random.Range(-4, 4) * 10, Random.Range(8, 11) * 12, Random.Range(-4, 4) * 10);
                        }
                    }
                    Destroy(hit.transform.gameObject);
                    for (int x = -1; x < 2; x++)
                    {
                        for (int y = -1; y < 2; y++)
                        {
                            for (int z = -1; z < 2; z++)
                            {
                                if (!(x == 0 && y == 0 && z == 0))
                                {
                                    Vector3 newChunk = chunk.transform.position;
                                    int XAdd = 0;
                                    int ZAdd = 0;
                                    if (IndexX + x == -1)
                                    {
                                        newChunk.x -= 16;
                                        XAdd = 16;
                                    }
                                    else if (IndexX + x == 16)
                                    {
                                        newChunk.x += 16;
                                        XAdd = -16;
                                    }
                                    if (IndexZ + z == -1)
                                    {
                                        newChunk.z -= 16;
                                        ZAdd = 16;
                                    }
                                    else if (IndexZ + z == 16)
                                    {
                                        newChunk.z += 16;
                                        ZAdd = -16;
                                    }
                                    GameObject BlockChunk = chunk;
                                    GameObject[] AllChunks = GameObject.FindGameObjectsWithTag("Chunk");
                                    for (int p = 0; p < AllChunks.GetLength(0); p++)
                                    {
                                        if (AllChunks[p].transform.position == newChunk)
                                        {
                                            BlockChunk = AllChunks[p];
                                            break;
                                        }
                                    }
                                    Vector3 NewBlockPos = new Vector3(IndexX + x + XAdd, (int)hit.transform.localPosition.y + y, IndexZ + z + ZAdd);
                                    BlockChunk.GetComponent<GenerateWorld>().DrawBlock(NewBlockPos);
                                }
                            }
                        }
                    }
                    return;
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                int Mask = 1 << 8;
                Mask = ~Mask;
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 0));
                if (Physics.Raycast(ray, out hit, 7f, Mask))
                {
                    if (hit.transform.gameObject.tag != "Block")
                        return;
                    //Debug.Log((hit.point - hit.transform.gameObject.transform.position) + "");
                    if (inventory[HandNumber - 1, inventory.GetLength(1) - 1].GetID() == 0)
                        return;
                    //string BlockName =IdToName(inventory[HandNumber -1, 4].GetID());
                    Vector3 BlockPos = new Vector3((int)((hit.point.x - hit.transform.gameObject.transform.position.x) * 2), (int)((hit.point.y - hit.transform.gameObject.transform.position.y) * 2), (int)((hit.point.z - hit.transform.gameObject.transform.position.z) * 2));
                    //Debug.Log(BlockPos + "");

                    GameObject Chunk = hit.transform.gameObject.transform.parent.gameObject;
                    BlockPos = BlockPos + hit.transform.gameObject.transform.position - Chunk.transform.position;
                    if (BlockPos.x <= -1 || BlockPos.x >= 16 || BlockPos.z <= -1 || BlockPos.z >= 16)
                    {
                        int X = (int)Chunk.transform.position.x;
                        int Z = (int)Chunk.transform.position.z;
                        if (BlockPos.x <= -1)
                        {
                            BlockPos.x = 15;
                            X -= 16;
                        }
                        if (BlockPos.x >= 16)
                        {
                            BlockPos.x = 0;
                            X += 16;
                        }
                        if (BlockPos.z <= -1)
                        {
                            BlockPos.z = 15;
                            Z -= 16;
                        }
                        if (BlockPos.z >= 16)
                        {
                            BlockPos.z = 0;
                            Z += 16;
                        }
                        GameObject[] AllChunks = GameObject.FindGameObjectsWithTag("Chunk");
                        for (int p = 0; p < AllChunks.GetLength(0); p++)
                        {
                            if (AllChunks[p].transform.position == new Vector3(X, 0, Z))
                            {
                                Chunk = AllChunks[p];
                                break;
                            }
                        }
                    }

                    Chunk.GetComponent<GenerateWorld>().CreateBlock(BlockPos, inventory[HandNumber - 1, inventory.GetLength(1) - 1].GetID());
                    inventory[HandNumber - 1, inventory.GetLength(1) - 1].Substruct1();
                    if (inventory[HandNumber - 1, inventory.GetLength(1) - 1].GetMumberOfItems() == 0)
                    {
                        inventory[HandNumber - 1, inventory.GetLength(1) - 1].DeleteItem();
                    }
                    HandUpdate();
                }
            }
        }
    }
    public void ClickInventory(int addup)
    {
        InventoryI = (addup % 10);
        InventoryJ = (addup / 10);
        if (invectoryItemsUI[0].activeInHierarchy)
        {
            Debug.Log(InventoryI + " " + InventoryJ);
            if (!CursorUIBusy)
            {
                if (inventory[InventoryI, InventoryJ].GetID() != 0)
                {
                    CursorUI.GetComponent<Image>().sprite = ItemsGUI[inventory[InventoryI, InventoryJ].GetID() - 1];
                    CursorID = inventory[InventoryI, InventoryJ].GetID();
                    CursorUI.transform.GetChild(0).GetComponent<Text>().text = inventory[InventoryI, InventoryJ].GetMumberOfItems() + "";
                    CursorNumberOfItems = inventory[InventoryI, InventoryJ].GetMumberOfItems();
                    inventory[InventoryI, InventoryJ].DeleteItem();
                    HandUpdate();
                    CursorUIBusy = true;
                }
            }
            else
            {
                if(inventory[InventoryI, InventoryJ].GetID() == 0)
                {
                    inventory[InventoryI, InventoryJ].AddNewItem(CursorID, CursorNumberOfItems);
                    CursorUI.GetComponent<Image>().sprite = (Sprite)Resources.Load(EmptyImage, typeof(Sprite));
                    CursorID = 0;
                    CursorUI.transform.GetChild(0).GetComponent<Text>().text = 0 + "";
                    CursorNumberOfItems = 0;
                    HandUpdate();
                    CursorUIBusy = false;
                }
            }
        }
    }
    public void ChangeJ0()
    {
        InventoryJ = 0;
    }
    public void ChangeJ1()
    {
        InventoryJ = 1;
    }
    public void ChangeJ2()
    {
        InventoryJ = 2;
    }
    public void ChangeJ3()
    {
        InventoryJ = 3;
    }
    public void ChangeI0()
    {
        InventoryI = 0;
    }
    public void ChangeI1()
    {
        InventoryI = 1;
    }
    public void ChangeI2()
    {
        InventoryI = 2;
    }
    public void ChangeI3()
    {
        InventoryI = 3;
    }
    public void ChangeI4()
    {
        InventoryI = 4;
    }
    public void ChangeI5()
    {
        InventoryI = 5;
    }
    public void ChangeI6()
    {
        InventoryI = 6;
    }
    public void ChangeI7()
    {
        InventoryI = 7;
    }
    public void ChangeI8()
    {
        InventoryI = 8;
    }
    public void ChangeI9()
    {
        InventoryI = 9;
    }
    private int NameToId(string Name)
    {
        int ID = 0;
        if (Name == "SnowBlock" + "Entity")
            ID = 1;
        if (Name == "GrassBlock" + "Entity")
            ID = 2;
        if (Name == "DirtBlock" + "Entity")
            ID = 3;
        if (Name == "StoneBlock" + "Entity")
            ID = 4;


        return ID;
    }
    private string IdToName(int ID)
    {
        string Name = "";
        if (ID == 1)
            Name = "SnowBlock";
        if (ID == 2)
            Name = "GrassBlock";
        if (ID == 3)
            Name = "DirtBlock";
        if (ID == 4)
            Name = "StoneBlock";
        return Name;
    }
    public bool GetCursorLock()
    {
        return cursorLock;
    }
    public void ShowPanel(int Num)
    {
        for(int i = 0; i < 4; i++)
        {
            CraftingPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
        if(Num == 0)
        {
            CraftingPanel.transform.GetChild(Num).gameObject.SetActive(true);
            return;
        }
        else
        {
            CraftingPanel.transform.GetChild(Num - 1).gameObject.SetActive(true);
            return;
        }
    }
}
