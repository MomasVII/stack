using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class RunGhosts : MonoBehaviour
{

  private List<GhostShot> framesList;
  private List<GhostShot> lastReplayList = null;

	GameObject theGhost;

  private float replayTimescale = 1;
  private int replayIndex = 0;
  private float recordTime = 0.0f;
  private float replayTime = 0.0f;

  public bool playGhostRecording = false;
  bool playRecording = false;

  public GameObject[] objectsRecorded;
  public int cur_time = 0;
  public string playFolder = "0";

  public Canvas mycanvas;

  void Start() {

		//Check if Ghost file exists. If it does load it
    if(playGhostRecording) {
      mycanvas.enabled = false;

      foreach(GameObject or in objectsRecorded) {
        Debug.Log("Getting File - "+ or.name);
        int x = 1;
    		while(File.Exists(Application.persistentDataPath + "/" + playFolder + "/" + or.name + "-" + x)) {
    			BinaryFormatter bf = new BinaryFormatter();
    			FileStream file = File.Open(Application.persistentDataPath + "/" + playFolder + "/" + or.name + "-" + x, FileMode.Open);
    			lastReplayList = (List<GhostShot>)bf.Deserialize(file);
    			file.Close();
          Debug.Log("Creating File - "+ or.name);
          if(or.name != "Main Camera") {
            CreateGhost(or.name);
          } else {
            GameObject.Find("Main Camera").GetComponent<Ghost>().runGhost(lastReplayList);
            Debug.Log("hello");
          }
          x++;
    		}
      }
    } else {
      System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
      cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
    }
	}

  public int getCurrentTime() {
    return cur_time;
  }

  /*void Update() {
    if(playGhostRecording) {
      Debug.Log("Playing");
  		CreateGhost();
  		replayIndex = 0;
  		playRecording = true;
      playGhostRecording = false;
    }
    if(playRecording) {
      MoveGhost();
    }
	}*/


  public void CreateGhost(string orName)
  {
    theGhost = Instantiate(Resources.Load(orName, typeof(GameObject))) as GameObject;
    theGhost.GetComponent<Ghost>().runGhost(lastReplayList);

    //Disable RigidBody
    //theGhost.GetComponent<Rigidbody>().isKinematic = true;
  }
}
