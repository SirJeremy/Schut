using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreenController : MonoBehaviour
{
    public void OnPlayClick() {
        GameManager.Instance.IsPaused = false;
    }
    public void OnQuitClick() {
        GameManager.Instance.Quit();
	}
}
