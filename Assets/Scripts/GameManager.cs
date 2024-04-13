using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum VEHICLETYPE { BOEING, PROPELLER, HELICOPTER, WATERPROPELLER, GREENBOEING ,GREENHELICOPTER, AIRBALOON, SONICPLANE }
public enum LANDZONETYPE { CIRCLEZONE, SQUAREZONE, WATERZONE, GREENSQUAREZONE, GREENCIRCLEZONE, BIGCIRCLEZONE }

public class GameManager : MonoBehaviour{
	public static GameManager Instance;
    [Header("Scene Manager")]
    public GameObject QuitMenu;

    [Header("Vehicle Manager")]
    public VEHICLETYPE veh_Type;
    public GameObject[] vehiclePrefab;
    public Vector3 spawnValue;
    private int veh_Num, veh_Count, landed_Num;
    private float waitTime = 4.5f;
    Vector3 spawnLocation;
    [HideInInspector] public bool gameOver;
    [HideInInspector] public bool gameClear;
    Coroutine makeCO;
    [HideInInspector] public List<GameObject> veh_List = new List<GameObject>();
    [HideInInspector] public List<GameObject> explosionEffect = new List<GameObject>();
    private List<GameObject> airportList = new List<GameObject>();
    public List<GameObject> airport_ItemsList = new List<GameObject>();
    public GameObject airpotrPrefab;
    [SerializeField] GameObject Sign;
    
    [Header("UI Element")]
    public Text displayNumber;
    private bool spawnLeft;
    private bool spawnUp;
    private bool up;
    public GameObject gameOverMenu;
    public GameObject victoryMenu;
    [SerializeField] private GameObject background;
    private int level;
    public Sprite[] backgroundSprite;

    [Header("Score Manager")]
    private int point = 0;
    [SerializeField] private Text pointText;
	private float duration = 0.5f;
	[SerializeField] Text[] finalScoreText;
	[SerializeField] Text[] finalHScoreText;
	[SerializeField] GameObject[] objNew;
    int finalScore = 0;
    bool doneCount;

    [Header("Audio Manager")]
	public AudioSource musicGame;
    [SerializeField] AudioSource soundGame;

    [SerializeField] AudioClip sound_Click;
    [SerializeField] AudioClip sound_Fail;
    [SerializeField] AudioClip sound_Score;


	public ADManager adManager;

	[SerializeField]
	Button soundBtn;
	int isSoundOn = 1;

	[SerializeField]
	Sprite[] spriteSound;

    private void Awake()
    {
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
		
    }

    void Start()
    {
        gameOver = false;
        gameClear = false;
		level = PlayerPrefs.GetInt ("LevelSelect", 1);
		LoadLevel(level);
		isSoundOn = PlayerPrefs.GetInt ("SoundOn", 1);
		if (isSoundOn == 1) {
			musicGame.Play ();
		}
		point = 0;
    }

    void FixedUpdate()
    {
        if (displayNumber != null)
        {
            displayNumber.text = landed_Num + "/" + veh_Num;
            pointText.text = "" + point;
        }


//		if (finalScoreText[1].IsActive())
//			finalScoreText[1].text = "" + finalScore;
//		else if (finalScoreText[0].IsActive())
//			finalScoreText[0].text = "" + finalScore;

        if (gameOver)
        {
			SoundSFX(sound_Fail);
            IsGameOver();
            gameOver = false;
        }

        if (gameClear)
        {
            IsFinish();
            gameClear = false;
        }
    }

	#region Start&Select
	public void OnClickReplay(){
		SceneManager.LoadScene("Game");
	}
	public void OnClickNext(){
		level++;
		PlayerPrefs.SetInt ("LevelSelect", level);
		PlayerPrefs.Save ();
		SceneManager.LoadScene("Game");
	}
    public void ClickQuit()
    {
		SoundSFX(sound_Click);
        QuitMenu.SetActive(true);
    }

    public void ClickComfirm(){
       SceneManager.LoadScene("Main");
    }

    public void ClickCancel()
    {
		SoundSFX(sound_Click);
        QuitMenu.SetActive(false);
    }

	public void ClickMute()
	{
		if (isSoundOn == 1) {
			isSoundOn = 0;
			soundBtn.image.sprite = spriteSound [0];
			musicGame.Stop ();
		} else {
			isSoundOn = 1;
			soundBtn.image.sprite = spriteSound [1];
			musicGame.Play ();
		}

		PlayerPrefs.GetInt ("SoundOn", isSoundOn);
		PlayerPrefs.Save ();
	}
    
	void LoadLevel(int lv)
    {
        switch (lv)
        {
            case (1):
                {
                    background.GetComponent<Image>().sprite = backgroundSprite[0];
                    veh_Num = 10;
                    for (int i = 0; i < 3; i++)
                    {
                        GameObject newAirPort = Instantiate(airpotrPrefab);
                        airportList.Add(newAirPort);
                    }
                    if (airportList.Count > 0)
                    {
                        airportList[0].transform.position = new Vector3(1.32f, -1.28f, 0);
                        airportList[0].GetComponent<AirPort>().MakeLandingZone(0);
                        airportList[0].GetComponent<AirPort>().allowedType = VEHICLETYPE.HELICOPTER;

                        airportList[1].transform.position = new Vector3(-2.24f, .3f, 0);
                        airportList[1].GetComponent<AirPort>().allowedType = VEHICLETYPE.BOEING;
                        airportList[1].GetComponent<AirPort>().MakeLandingZone(1);

                        airportList[2].transform.position = new Vector3(-2.24f, .3f, 0);
                        airportList[2].GetComponent<AirPort>().allowedType = VEHICLETYPE.PROPELLER;
                        airportList[2].GetComponent<AirPort>().MakeLandingZone(1);
                    }
                    break;
                }
            case (2):
                {
                    background.GetComponent<Image>().sprite = backgroundSprite[1];
                    veh_Num = 20;
                    for (int i = 0; i < 3; i++)
                    {
                        GameObject newAirPort = Instantiate(airpotrPrefab);
                        airportList.Add(newAirPort);
                    }
                    if (airportList != null)
                    {
                        // circle zone
                        airportList[0].transform.position = new Vector3(4.12f, -0.39f, 0);
                        airportList[0].GetComponent<AirPort>().allowedType = VEHICLETYPE.HELICOPTER;
                        airportList[0].GetComponent<AirPort>().MakeLandingZone(0);

                        // square zone for Boeing
                        airportList[1].transform.position = new Vector3(-1.22f, 1.11f, 0);
                        airportList[1].GetComponent<AirPort>().allowedType = VEHICLETYPE.BOEING;
                        airportList[1].GetComponent<AirPort>().MakeLandingZone(1);

                        // circle zone for Propeller
                        airportList[2].transform.position = new Vector3(1.81f, -0.55f, 0);
                        airportList[2].GetComponent<AirPort>().allowedType = VEHICLETYPE.PROPELLER;
                        airportList[2].GetComponent<AirPort>().MakeLandingZone(1);
                        airportList[2].transform.rotation = Quaternion.Euler(0, 0, 55);
                    }
                    break;
                }
            case (3):
                {
                    background.GetComponent<Image>().sprite = backgroundSprite[2];
                    veh_Num = 30;
                    for (int i = 0; i < 4; i++)
                    {
                        GameObject newAirPort = Instantiate(airpotrPrefab);
                        airportList.Add(newAirPort);
                    }
                    if (airportList.Count > 0)
                    {
                        // circle zone
                        airportList[0].transform.position = new Vector3(0.85f, -1.55f, 0);
                        airportList[0].GetComponent<AirPort>().allowedType = VEHICLETYPE.HELICOPTER;
                        airportList[0].GetComponent<AirPort>().MakeLandingZone(0);

                        // square zone for Boeing
                        airportList[1].transform.position = new Vector3(-2.9f, -0.15f, 0);
                        airportList[1].GetComponent<AirPort>().allowedType = VEHICLETYPE.BOEING;
                        airportList[1].GetComponent<AirPort>().MakeLandingZone(1);

                        // square zone for Propeller
                        airportList[2].transform.position = new Vector3(-2.9f, -0.15f, 0);
                        airportList[2].GetComponent<AirPort>().allowedType = VEHICLETYPE.PROPELLER;
                        airportList[2].GetComponent<AirPort>().MakeLandingZone(1);

                        // square zone for WaterPropeller
                        airportList[3].transform.position = new Vector3(3.3f, 1.1f, 0);
                        airportList[3].GetComponent<AirPort>().allowedType = VEHICLETYPE.WATERPROPELLER;
                        airportList[3].GetComponent<AirPort>().MakeLandingZone(1);
                        airportList[3].transform.rotation = Quaternion.Euler(0, 0, 180);
                    }
                    break;
                }
            case (4):
                {
                    background.GetComponent<Image>().sprite = backgroundSprite[3];
                    veh_Num = 40;
                    for (int i = 0; i < 6; i++)
                    {
                        GameObject newAirPort = Instantiate(airpotrPrefab);
                        airportList.Add(newAirPort);
                    }
                    if (airportList.Count > 0)
                    {
                        // circle zone
                        airportList[0].transform.position = new Vector3(-2.8f, -2.4f, 0);
                        airportList[0].GetComponent<AirPort>().allowedType = VEHICLETYPE.HELICOPTER;
                        airportList[0].GetComponent<AirPort>().MakeLandingZone(0);

                        // square zone for Boeing
                        airportList[1].transform.position = new Vector3(-3.7f, 0.6f, 0);
                        airportList[1].GetComponent<AirPort>().allowedType = VEHICLETYPE.BOEING;
                        airportList[1].GetComponent<AirPort>().MakeLandingZone(1);
                        airportList[1].transform.rotation = Quaternion.Euler(0, 0, 12);

                        // square zone for Propeller
                        airportList[2].transform.position = new Vector3(-3.7f, 0.6f, 0);
                        airportList[2].GetComponent<AirPort>().allowedType = VEHICLETYPE.PROPELLER;
                        airportList[2].GetComponent<AirPort>().MakeLandingZone(1);
                        airportList[2].transform.rotation = Quaternion.Euler(0, 0, 12);

                        // square zone for WaterPropeller
                        airportList[3].transform.position = new Vector3(3.6f, -2.06f, 0);
                        airportList[3].GetComponent<AirPort>().allowedType = VEHICLETYPE.WATERPROPELLER;
                        airportList[3].GetComponent<AirPort>().MakeLandingZone(1);
                        airportList[3].transform.rotation = Quaternion.Euler(0, 0, 180);

                        // circle zone for GreenHelicopter
                        airportList[4].transform.position = new Vector3(2.9f, -.27f, 0);
                        airportList[4].GetComponent<AirPort>().allowedType = VEHICLETYPE.GREENHELICOPTER;
                        airportList[4].GetComponent<AirPort>().MakeLandingZone(0);

                        // square zone for GreenBoeing
                        airportList[5].transform.position = new Vector3(3.7f, 0.79f, 0);
                        airportList[5].GetComponent<AirPort>().allowedType = VEHICLETYPE.GREENBOEING;
                        airportList[5].GetComponent<AirPort>().MakeLandingZone(1);
                        airportList[5].transform.rotation = Quaternion.Euler(0, 0, 164);

                    }
                    break;
                }
            case (5):
                {
                    background.GetComponent<Image>().sprite = backgroundSprite[4];
                    veh_Num = 50;
                    for (int i = 0; i < 8; i++)
                    {
                        GameObject newAirPort = Instantiate(airpotrPrefab);
                        airportList.Add(newAirPort);
                    }
                    if (airportList.Count > 0)
                    {
                        // circle zone
                        airportList[0].transform.position = new Vector3(-2.5f, -2.37f, 0);
                        airportList[0].GetComponent<AirPort>().allowedType = VEHICLETYPE.HELICOPTER;
                        airportList[0].GetComponent<AirPort>().MakeLandingZone(0);

                        // square zone for Boeing
                        airportList[1].transform.position = new Vector3(-5.52f, -0.873f, 0);
                        airportList[1].GetComponent<AirPort>().allowedType = VEHICLETYPE.BOEING;
                        airportList[1].GetComponent<AirPort>().MakeLandingZone(1);

                        // square zone for Propeller
                        airportList[2].transform.position = new Vector3(-2.52f, 0.8f, 0);
                        airportList[2].GetComponent<AirPort>().allowedType = VEHICLETYPE.PROPELLER;
                        airportList[2].GetComponent<AirPort>().MakeLandingZone(1);
                        airportList[2].transform.rotation = Quaternion.Euler(0, 0, -56);

                        // square zone for WaterPropeller
                        airportList[3].transform.position = new Vector3(1.8f, 3.55f, 0);
                        airportList[3].GetComponent<AirPort>().allowedType = VEHICLETYPE.WATERPROPELLER;
                        airportList[3].GetComponent<AirPort>().MakeLandingZone(1);
                        airportList[3].transform.rotation = Quaternion.Euler(0, 0, 180);

                        // circle zone for GreenHelicopter
                        airportList[4].transform.position = new Vector3(4.75f, 0.61f, 0);
                        airportList[4].GetComponent<AirPort>().allowedType = VEHICLETYPE.GREENHELICOPTER;
                        airportList[4].GetComponent<AirPort>().MakeLandingZone(0);

                        // square zone for GreenBoeing
                        airportList[5].transform.position = new Vector3(3.6f, 1.37f, 0);
                        airportList[5].GetComponent<AirPort>().allowedType = VEHICLETYPE.GREENBOEING;
                        airportList[5].GetComponent<AirPort>().MakeLandingZone(1);
                        airportList[5].transform.rotation = Quaternion.Euler(0, 0, -107);

                        // square zone for SonicPlane
                        airportList[6].transform.position = new Vector3(3.6f, 1.37f, 0);
                        airportList[6].GetComponent<AirPort>().allowedType = VEHICLETYPE.SONICPLANE;
                        airportList[6].GetComponent<AirPort>().MakeLandingZone(1);
                        airportList[6].transform.rotation = Quaternion.Euler(0, 0, -107);

                        //circle zone for AirBallon
                        airportList[7].transform.position = new Vector3(0.43f, 1.06f, 0);
                        airportList[7].GetComponent<AirPort>().allowedType = VEHICLETYPE.AIRBALOON;
                        airportList[7].GetComponent<AirPort>().MakeLandingZone(0);
                    }
                    break;
                }
        }
        makeCO = StartCoroutine(MakeVehicle(veh_Num));
    }
    #endregion

    #region Instantiate
    public IEnumerator MakeVehicle(int veh_Num)
    {
        yield return new WaitForSeconds(1.5f);
        while (veh_Count < veh_Num)
        {
            
            if (up)
            {
                if (spawnLeft)
                {
                    spawnLocation = new Vector3(spawnValue.x, Random.Range(-spawnValue.y, spawnValue.y));
                    spawnLeft = false;
                }
                else
                {
                    spawnLocation = new Vector3(-spawnValue.x, Random.Range(-spawnValue.y, spawnValue.y));
                    spawnLeft = true;
                }
                up = false;
            }
            else
            {
                if (spawnUp)
                {
                    spawnLocation = new Vector3(Random.Range(-spawnValue.x, spawnValue.x), spawnValue.y);
                    spawnUp = false;
                }
                else
                {
                    spawnLocation = new Vector3(Random.Range(-spawnValue.x, spawnValue.x), -spawnValue.y);
                    spawnUp = false;
                }
                up = true;
            }
            spawnLocation.z = 0;
            veh_Type = (VEHICLETYPE)VEH_TYPE();
            if (spawnLocation != Vector3.zero)
                StartCoroutine(WarnningSign(spawnLocation));
            yield return new WaitForSeconds(2.5f);
            GameObject newVehicle = Instantiate(vehiclePrefab[(int)veh_Type], spawnLocation, Quaternion.identity);
            if (newVehicle != null)
            {
                
                newVehicle.GetComponent<Vehicle>().vEHICLETYPE = veh_Type;
                veh_List.Add(newVehicle);
                veh_Count++;
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator WarnningSign(Vector3 pos)
    {
        Vector3 sign_Pos = pos;
        Vector3 center = Vector3.zero;

        if (sign_Pos.x == 9 || sign_Pos.x == -9)
        {
            if (sign_Pos.y >= 4.5f || sign_Pos.y <= -4.5f)
                sign_Pos = Vector3.Lerp(sign_Pos, center, 0.2f);
            else
                sign_Pos = Vector3.Lerp(sign_Pos, center, 0.08f);
        }
        else
            sign_Pos = Vector3.Lerp(sign_Pos, center, 0.25f);
        GameObject newSign = Instantiate(Sign);
        newSign.transform.position = sign_Pos;
        yield return new WaitForSeconds(2.3f);
        Destroy(newSign);
    }

    int VEH_TYPE()
    {
        int rand;
        switch (level)
        {
            case (1):
                rand = Random.Range(0, 3);
                return rand;
            case (2):
                rand = Random.Range(0, 3);
                return rand;
            case (3):
                rand = Random.Range(0, 4);
                return rand;
            case (4):
                rand = Random.Range(0, 5);
                return rand;
            case (5):
                rand = Random.Range(0, 7);
                return rand;
            default:
                return rand = 0;
        }

    }
    #endregion

    #region Score&GameEnd
	public void ScorePointCheck(int sco)
    {
		Debug.Log ("------>   " + sco + "   Total:  " + point);
		SoundSFX(sound_Score);
        landed_Num++;
		point += sco;
        if (landed_Num >= veh_Num)
        {
            gameClear = true;
        }
            
    }

    void IsGameOver()
    {
        foreach (GameObject vehicle in veh_List)
        {
            if (vehicle != null)
            {
                vehicle.GetComponent<Collider2D>().enabled = false;
                vehicle.GetComponent<Vehicle>().allowToMove = false;
            }
        }

        StartCoroutine(GameOver());
        
    }

    void IsFinish()
    {
        StartCoroutine(GameClear());
        ClearAll();
    }

    // support method when game end
    void ClearAll()
    {
        ClearAllVehicle();
        foreach (var ap in airportList)
        {
            Destroy(ap);
        }
        ClearAllList();
        StopCoroutine(makeCO);
    }

    void ClearAllList()
    {
        airportList.Clear();
        airport_ItemsList.Clear();
        veh_List.Clear();
        explosionEffect.Clear();
    }

    void ClearAllVehicle()
    {
        for (int i = 0; i < veh_List.Count; i++)
        {
            Destroy(veh_List[i]);
            if (explosionEffect.Count > 0)
            {
                if (i <= 1)
                    Destroy(explosionEffect[i]);
            }
        }
    }

    IEnumerator GameOver()
    {
        yield return new WaitForSeconds(2f);
        gameOverMenu.SetActive(true);
        StartCoroutine(CountTo(point));
        ClearAll();
		adManager.showInterstital ();
    }

    IEnumerator GameClear()
    {
        yield return new WaitForSeconds(1.3f);
		victoryMenu.SetActive(true);
		adManager.showInterstital ();
        StartCoroutine(CountTo(point));

		int level_Unlock = PlayerPrefs.GetInt("LevelUnlock", 1);
		if (level >= level_Unlock) {
			PlayerPrefs.SetInt ("LevelUnlock", level_Unlock + 1);
			PlayerPrefs.Save ();
		}
    }

    IEnumerator CountTo(int target)
    {
        yield return null;
        float start = finalScore;
        for (float timer = 0; timer < duration; timer += Time.deltaTime)
        {
            float progress = timer / duration;
            finalScore = (int)Mathf.Lerp(start, target, progress);
            yield return null;
        }
        finalScore = target;
		int hscore = PlayerPrefs.GetInt ("HighScore_" + level, 0);
		if (finalScore > hscore) {
			hscore = finalScore;
			PlayerPrefs.SetInt ("HighScore_" + level, hscore);

			foreach (GameObject lb in objNew) {
				lb.SetActive (true);
			}
		}
		foreach (Text lb in finalScoreText) {
			lb.text = "Score: " + finalScore;
		}

		foreach (Text lb in finalHScoreText) {
			lb.text = "High Score: " + hscore;
		}
    }
    #endregion

    public void ShowWay(VEHICLETYPE vehType)
    {
        foreach (GameObject airport in airportList)
        {
            if (airport.GetComponent<AirPort>().allowedType == vehType)
            {
                if (airport.layer == 8)
                {
                    airport.transform.GetChild(1).gameObject.SetActive(true);
                }
                else if (airport.layer == 9)
                    airport.transform.GetChild(2).gameObject.SetActive(true);
            }
        }
    }

    public void DeactiveShowWay()
    {
        foreach (GameObject airport in airportList)
        {
            airport.transform.GetChild(1).gameObject.SetActive(false);
            airport.transform.GetChild(2).gameObject.SetActive(false);
        }
    }


	private void SoundSFX(AudioClip soundClip){
		if (isSoundOn == 1) {
//			gameSoundSource.PlayOneShot(soundClip);
		}
	}
}