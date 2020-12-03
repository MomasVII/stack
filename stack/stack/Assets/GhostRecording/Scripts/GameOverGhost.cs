using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverGhost : MonoBehaviour
{

  public void GamesOver() {
    Ghost[] ghosts = (Ghost[]) GameObject.FindObjectsOfType (typeof(Ghost));
    foreach (Ghost ghost in ghosts) {
      ghost.StopRecordingGhost();
    }
  }

}
