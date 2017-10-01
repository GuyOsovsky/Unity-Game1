using UnityEngine;
using System.Collections;

public class PickUpItem : MonoBehaviour {
    private int id;
    GameObject Player;
    bool PickUp;
    float Force;
    public void ChangeID(int ID)
    {
        id = ID;
    }
    public int GetID()
    {
        return id;
    }
	// Use this for initialization
	void Start () {
        PickUp = false;
	}
	
    public void StartPickUp()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        PickUp = true;
        Force = 0;
    }
	// Update is called once per frame
	void Update () {
        if (PickUp)
        {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            gameObject.transform.LookAt(Player.transform);
            gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * 600);
            if (Vector3.Distance(Player.transform.position, gameObject.transform.position) <= 1f)
            {
                Destroy(gameObject);
            }
        }
	}
}
