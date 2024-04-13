using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour {
	[Header("Scene Manager")]
	public GameObject SelectMenu;
	public GameObject StartMenu;
	public GameObject QuitMenu;

	[Header("Audio Manager")]
	[SerializeField] AudioSource musicMain;
	[SerializeField] AudioSource soundClick;

	[SerializeField]
	Button[] btnLevel;


	[SerializeField]
	Button soundBtn;
	int isSoundOn = 1;

	[SerializeField]
	Sprite[] spriteSound;


	private int level_Unlock;

	public ADManager adManager;

	private void Awake(){
	}

	void Start()
	{
		level_Unlock = PlayerPrefs.GetInt("LevelUnlock", 1);
		isSoundOn = PlayerPrefs.GetInt ("SoundOn", 1);
		StartMenu.SetActive(true);
		updateLockLevel();
		if (isSoundOn == 1) {
			musicMain.Play ();
		}
		adManager.showBanner ();
	}

	void updateLockLevel()
	{
		for (int i = 0; i < btnLevel.Length; i++)
		{
			btnLevel[i].interactable = (i + 1 <= level_Unlock);
		}
	}

	#region Start&Select
	public void ClickStart()
	{
		SoundClick ();
		StartMenu.SetActive(false);
		SelectMenu.SetActive(true);
	}
	public void ClickQuit()
	{
		SoundClick ();
		QuitMenu.SetActive(true);
	}
	public void ClickExit()
	{
		SoundClick ();
		if (SelectMenu.activeSelf) {
			SelectMenu.SetActive (false);
			QuitMenu.SetActive(false);
			StartMenu.SetActive(true);
		}else
			Application.Quit();
	}

	public void ClickCancel()
	{
		SoundClick ();
		QuitMenu.SetActive(false);
	}
	public void ClickMute()
	{
		if (isSoundOn == 1) {
			isSoundOn = 0;
			soundBtn.image.sprite = spriteSound [0];
			musicMain.Stop ();
		} else {
			isSoundOn = 1;
			soundBtn.image.sprite = spriteSound [1];
			musicMain.Play ();
		}
		PlayerPrefs.GetInt ("SoundOn", isSoundOn);
		PlayerPrefs.Save ();
	}
	public void ClickLevel(int lv){
		SoundClick ();
		PlayerPrefs.SetInt ("LevelSelect", lv);
		PlayerPrefs.Save ();

		adManager.hideBanner ();
		SceneManager.LoadScene ("Game");
	}
	#endregion

	private void SoundClick(){
		if (isSoundOn == 1) {
			soundClick.Play();
		}
	}
}
