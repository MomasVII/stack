using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class WB_Vector3 {

	private float x;
	private float y;
	private float z;

	public WB_Vector3() { }
	public WB_Vector3(Vector3 vec3) {
		this.x = vec3.x;
		this.y = vec3.y;
		this.z = vec3.z;
	}

	public static implicit operator WB_Vector3(Vector3 vec3) {
		return new WB_Vector3(vec3);
	}
	public static explicit operator Vector3(WB_Vector3 wb_vec3) {
		return new Vector3(wb_vec3.x, wb_vec3.y, wb_vec3.z);
	}
}

[System.Serializable]
public class WB_Quaternion {

    private float w;
	private float x;
	private float y;
	private float z;

	public WB_Quaternion() { }
	public WB_Quaternion(Quaternion quat3) {
		this.x = quat3.x;
		this.y = quat3.y;
		this.z = quat3.z;
    this.w = quat3.w;
	}

	public static implicit operator WB_Quaternion(Quaternion quat3) {
		return new WB_Quaternion(quat3);
	}
	public static explicit operator Quaternion(WB_Quaternion wb_quat3) {
		return new Quaternion(wb_quat3.x, wb_quat3.y, wb_quat3.z, wb_quat3.w);
	}
}

[System.Serializable]
public class GhostShot
{
    public float timeMark = 0.0f;       // mark at which the position and rotation are of af a given shot
		public float frameCount = 0.0f;

    private WB_Vector3 _posMark;
    public Vector3 posMark {
		get {
			if (_posMark == null) {
				return Vector3.zero;
			} else {
				return (Vector3)_posMark;
			}
		}
		set {
			_posMark = (WB_Vector3)value;
		}
	}

    private WB_Quaternion _rotMark;
    public Quaternion rotMark {
		get {
			if (_rotMark == null) {
				return Quaternion.identity;
			} else {
				return (Quaternion)_rotMark;
			}
		}
		set {
			_rotMark = (WB_Quaternion)value;
		}
	}

}

public class Ghost : MonoBehaviour {

  private List<GhostShot> framesList;
  private List<GhostShot> lastReplayList = null;

	GameObject theGhost;

  private float replayTimescale = 1;
  private int replayIndex = 0;
  private float recordTime = 0.0f;
  private float replayTime = 0.0f;

	private int myFrameCount = 0;

  //Check whether we should be recording or not
  bool startRecording = true, recordingFrame = false, playRecording = false;

	void Start() {
		PlayerPrefs.SetString("Name", "Thomas");
	}

	void FixedUpdate () {
    if (startRecording) {
      startRecording = false;
			Debug.Log("Recording Started");
      StartRecording();
    } else if (recordingFrame) {
    	RecordFrame();
    }

    if (lastReplayList != null && playRecording) {
			myFrameCount++;
			if (this.GetComponent<BlockScript> () != null) {
				this.GetComponent<BlockScript>().enabled = false;
			}
      MoveGhost();
    }
	}

	private void RecordFrame() {
		recordTime += Time.smoothDeltaTime * 1000;
    GhostShot newFrame = new GhostShot()
    {
      timeMark = recordTime,
			frameCount = Time.frameCount,
			posMark = this.transform.position,
			rotMark = this.transform.rotation
    };

    framesList.Add(newFrame);
	}

	public void StartRecording() {
    framesList = new List<GhostShot>();
    replayIndex = 0;
    recordTime = Time.time * 1000;
    recordingFrame = true;
		playRecording = false;
  }

	public void StopRecordingGhost() {
		Debug.Log("Stop Recording");
		recordingFrame = false;
		lastReplayList = new List<GhostShot>(framesList);

		//This will overwrite any previous Save
		//Run function if new highscore achieved or change filename in function
    SaveGhostToFile(); //Save Ghost to file on device/computer
	}

	public void SaveGhostToFile()
	{
		 int timestamp = GameObject.FindWithTag("Record").GetComponent<RunGhosts>().getCurrentTime();
		 int x = 1;
		 string filename = "";
		 bool found = true;
		 while(found) {
			 filename = this.name.Replace("(Clone)", "")+"-"+x;
			 if(System.IO.File.Exists(Application.persistentDataPath + "/" + timestamp + "/" + filename)) {
				 x++;
			 } else {
				 found = false;
			 }
		 }
			// Prepare to write
			BinaryFormatter bf = new BinaryFormatter();
			Directory.CreateDirectory(Application.persistentDataPath + "/" + timestamp);
			FileStream file = File.Create(Application.persistentDataPath + "/" + timestamp + "/" + filename);
			Debug.Log("File Location: " + Application.persistentDataPath + "/" + timestamp + "/" + filename);
			// Write data to disk
			bf.Serialize(file, lastReplayList);
			file.Close();
	}

	public void runGhost(List<GhostShot> lastReplayListPass) {
		Debug.Log("Running Ghost");
		lastReplayList = lastReplayListPass;
		startRecording = false;
		playRecording = true;
	}


	  public void MoveGhost()
	  {

	      if (replayIndex + 1 < lastReplayList.Count)
	      {
					//Debug.Log("mfc: "+myFrameCount);
	        if(lastReplayList[replayIndex].frameCount <= Time.frameCount){
						Debug.Log(replayIndex+" "+lastReplayList.Count);
						//Debug.Log("frameCount: "+lastReplayList[replayIndex].frameCount);
						replayIndex++;
	          GhostShot frame = lastReplayList[replayIndex];
						//Debug.Log(frame.posMark+" "+lastReplayList[replayIndex].posMark);
	          DoLerp(lastReplayList[replayIndex - 1], frame);
	          replayTime += Time.smoothDeltaTime * 1000 * 0.1f;
					}
	      } else {
	        playRecording = false;
	      }
	  }

	  private void DoLerp(GhostShot a, GhostShot b) {
		   this.transform.position = Vector3.Slerp(a.posMark, b.posMark, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
		   this.transform.rotation = Quaternion.Slerp(a.rotMark, b.rotMark, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
	  }

	/*public void playGhostRecording() {
		CreateGhost();
		replayIndex = 0;
		playRecording = true;
	}*/

  /*public void StartRecordingGhost() {
    startRecording = true;
  }*/

  /*public void MoveGhost()
  {
      replayIndex++;
      if (replayIndex < lastReplayList.Count)
      {
          GhostShot frame = lastReplayList[replayIndex];
          DoLerp(lastReplayList[replayIndex - 1], frame);
          replayTime += Time.smoothDeltaTime * 1000 * replayTimescale;
      }
  }*/

    /*private void DoLerp(GhostShot a, GhostShot b)
    {
		if(GameObject.FindWithTag("Ghost") != null) {
	        theGhost.transform.position = Vector3.Slerp(a.posMark, b.posMark, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
	        theGhost.transform.rotation = Quaternion.Slerp(a.rotMark, b.rotMark, Mathf.Clamp(replayTime, a.timeMark, b.timeMark));
		}
    }*/

    /*public void CreateGhost()
    {
		//Check if ghost exists or not, no reason to destroy and create it everytime.
		if(GameObject.FindWithTag("Ghost") == null) {
	        theGhost = Instantiate(Resources.Load("GhostPrefab", typeof(GameObject))) as GameObject;
	        theGhost.gameObject.tag = "Ghost";

	        //Disable RigidBody
	        //theGhost.GetComponent<Rigidbody>().isKinematic = true;

            MeshRenderer mr = theGhost.gameObject.GetComponent<MeshRenderer>();
            mr.material = Resources.Load("Ghost_Shader", typeof(Material)) as Material;
		}
    }*/
}
