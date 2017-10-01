using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
public class CanvasController : MonoBehaviour {
    private static string WorldName;
	// Use this for initialization
	void Start() {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");
        }
        WorldName = "new world";
    }
    public void Exit()
    {
        Debug.Log("quit");
        Application.Quit();
    }
    public void NewGameButton()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        gameObject.transform.GetChild(2).gameObject.SetActive(true);
    }
    public void StartNewWorld()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves/" + WorldName))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves/" + WorldName);
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves/" + WorldName + "/chunks");
            File.Create(Application.persistentDataPath + "/Saves/" + WorldName + "/gameinfo.dat");
            File.Create(Application.persistentDataPath + "/Saves/" + WorldName + "/inventory.dat");
            GameObject.FindGameObjectWithTag("Player").GetComponent<ChunksController>().GenerateNewWorld();
        }
    }
    public void BackToStart()
    {
        for(int i = 2; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
    }
    public void ChangeWorldName()
    {
        string Name = GameObject.Find("WorldName").transform.GetChild(2).GetComponent<Text>().text;
        if (Name == null || Name == "")
            Name = "new world";
        WorldName = Name;
    }
    public static string GetWorldName()
    {
        return WorldName;
    }
}
