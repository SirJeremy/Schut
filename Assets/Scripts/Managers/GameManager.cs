using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
	#region Variables
	[SerializeField]
    private bool isPaused = true;
	[SerializeField]
	private GameObject pauseScreen;

	private CursorLockMode lastLockMode = CursorLockMode.Confined;
	#endregion

	#region Properties
	public bool IsPaused { 
		get => isPaused; 
		set {
			if(value != isPaused)
				DoPause(isPaused = value);
		}
	}
	#endregion

	#region MonoBehaviours
	private void Start() {
		DoPause(isPaused);
	}
	private void Update() {
		if (Input.GetButtonDown("Pause"))
			IsPaused = !isPaused;
	}
	#endregion

	#region Methods
	private void DoPause(bool value) {
		//better ways of doing this, but works for now (like events)
		if(pauseScreen != null)
			pauseScreen.SetActive(value);

		if(value) {
			//pause
			Time.timeScale = 0;
			lastLockMode = Cursor.lockState;
			Cursor.lockState = CursorLockMode.None;
		}
		else {
			//unpause
			Time.timeScale = 1;
			Cursor.lockState = lastLockMode;
		}
	}
	public void Quit() {
	#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
	#else
		Application.Quit();
	#endif
	}
	#endregion
}
