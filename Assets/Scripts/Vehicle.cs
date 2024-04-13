using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Vehicle : MonoBehaviour
{
    private List<Vector3> targetPosList = new List<Vector3>();
    private List<Vector3> preTargetList = new List<Vector3>();
    
    private float speed;
    private Vector3 currentHeading;
    private const float checkRadius = 2.4f;
    [SerializeField] private LayerMask vehicleLayer;
    public GameObject outerRing;
    public GameObject explode;
    
    private float timer;
    private float scaleDownDuration = 0.3f;
    Vector3 oldPos;
    public bool allowToMove = true;
    private bool preLine;
    LineRenderer lineRenderer;

    [Header("Vehicle Info")]
    private const float minDistance = 0.05f;
    private int targetIndex = 0;
    [SerializeField] private bool selected;
    [SerializeField] private bool setFlyPath;
    [SerializeField] private bool landing;
    private Vector3 target_Landing;
    public VEHICLETYPE vEHICLETYPE;

    [Header("Sound")]
    public AudioSource vehicle_Sound;
    public AudioClip sound_Boom;
    public AudioClip sound_Collide;
    public AudioClip sound_Sellect;
    bool playOnce = false;
    bool playOnce2 = false;


	[Header("Score")]
	int score = 100;
	float timeCount = 0;
	float timeMaxCount = 2;
//	[SerializeField] Text textScore;
	bool isCalScore = true;

    void Start()
    {
        lineRenderer = this.gameObject.GetComponent<LineRenderer>();
        landing = false;
        SetSpeed();
        RandomTarget();
        SetNewDir();
		score = 100;
		updateTextScore ();
    }

    void FixedUpdate()
    {
        NearbyObj();
        OutOfBoundCheck();
        VehicleRotation();
        if (allowToMove)
        {
            Move();
        }
        else if (landing)
        {
            currentHeading = (target_Landing - transform.position).normalized;
            VehicleRotation();
            transform.position = Vector3.MoveTowards(transform.position, target_Landing, Time.deltaTime);
        }
        if (selected)
        {
            if (!playOnce2)
            {
                vehicle_Sound.PlayOneShot(sound_Sellect);
                playOnce2 = true;
            }
            DrawPreLine();
//			isCalScore = true;
        }
        else if (!selected)
        {
            if (preTargetList.Count != 0)
                preTargetList.Clear();
            playOnce2 = false;
        }
		if (isCalScore && score > 0) {
			timeCount += Time.deltaTime;
			if (timeCount >= timeMaxCount) {
				timeCount = 0;
				score--;
				updateTextScore ();
			}
			//		textScore.transform.LookAt (Vector3.zero);
		}
    }

    #region MakeStart&UpdateLookBetter
    void OutOfBoundCheck()
    {
        timer += Time.deltaTime;
        if (timer >= 2f)
        {
            if (!(transform.position.x >= -8f && transform.position.x <= 8f && transform.position.y >= -5 && transform.position.y <= 5))
            {
                SetNewDir();
                timer = 0;
            }
        }
    }

    void Move()
    {
        if (targetPosList.Count >= 2)
        {
            ControledMovement();
        }
        else if (targetPosList.Count <= 1)
        {
            FreeMovement();
        }
    }

    void SetSpeed() 
    {
        switch (vEHICLETYPE)
        {
		case VEHICLETYPE.BOEING:
			speed = 8;
			timeMaxCount = 3;
                break;
            case VEHICLETYPE.PROPELLER:
			speed = 6;
			timeMaxCount = 4;
                break;
            case VEHICLETYPE.HELICOPTER:
                speed = 4;
			timeMaxCount = 5;
                break;
            case VEHICLETYPE.WATERPROPELLER:
			speed = 6;
			timeMaxCount = 4;
                break;
            case VEHICLETYPE.GREENBOEING:
			speed = 8;
			timeMaxCount = 3;
                break;
            case VEHICLETYPE.GREENHELICOPTER:
			speed = 5;
			timeMaxCount = 5.5f;
                break;
            case VEHICLETYPE.AIRBALOON:
			speed = 2.5f;
			timeMaxCount = 8.5f;
                break;
            case VEHICLETYPE.SONICPLANE:
			speed = 12;
			timeMaxCount = 2;
                break;

        }
    }
    #endregion

    #region AutoFly
    void SetNewDir()
    {
        currentHeading = (RandomTarget() - transform.position).normalized;
    }

    void FreeMovement()
    {
        VehicleRotation();
        transform.position += currentHeading * speed * 0.1f * Time.deltaTime;
        setFlyPath = false;
    }

    Vector3 RandomTarget()
    {
        Vector3 target = Vector3.zero;
        if (target.x < 10 && target.x > -10 && target.y > -6 && target.y < 6)
        {
			target = new Vector3(UnityEngine.Random.Range(-5, 6), UnityEngine.Random.Range(-3, 5));
            target.z = 0;
        }
        return target;
    }

    void VehicleRotation()
    {
        var angle = Mathf.Atan2(currentHeading.y, currentHeading.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 3.3f * Time.deltaTime);
    }
    #endregion

    #region ControlFly
    private void OnMouseDown()
    {
        if (targetPosList.Count != 0)
            targetPosList.Clear();

        allowToMove = false;
        preLine = true;
        selected = true;
        GameManager.Instance.ShowWay(vEHICLETYPE);
    }
    private void OnMouseUp()
    {
        preLine = false;
        selected = false;
        DrawPathLine();
        GameManager.Instance.DeactiveShowWay();
    }

    void ControledMovement()
    {
        if (targetPosList != null)
        {
            setFlyPath = true;
            if (targetIndex < targetPosList.Count)
            {
                currentHeading = (targetPosList[targetIndex] - transform.position).normalized;
                
                if (transform.position != targetPosList[targetIndex])
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosList[targetIndex], speed * 0.1f * Time.deltaTime);
                }
                if (Vector3.Distance(transform.position, targetPosList[targetIndex]) <= minDistance)
                {
                    targetPosList.RemoveAt(targetIndex);
                    GenerateLine(targetPosList);
                }
            }
        }
        else
            return;
    }
    #endregion

    #region Draw line
    void DrawPreLine()
    {
        Vector3 newPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = 0;
        if (newPos != oldPos)
        {
            SavePointMarker(newPos);
            oldPos = newPos;
            preLine = true;
            GenerateLine(preTargetList);
            allowToMove = true;
        }

    }
    void DrawPathLine()
    {
        preLine = false;
        GenerateLine(targetPosList);
    }

    void SavePointMarker(Vector3 pointPos)
    {
        if (lineRenderer != null)
        {
            preTargetList.Add(pointPos);
            targetPosList.Add(pointPos);
        }
    }

    public void GenerateLine(List<Vector3> allPointPos)
    {
        if (preLine)
        {
            lineRenderer.startWidth = .1f;
            lineRenderer.endWidth = .1f;
        }
        if (!preLine)
        {
            lineRenderer.startWidth = .05f;
            lineRenderer.endWidth = .05f;
        }
        for (int i = 0; i < allPointPos.Count; i++)
        {
            if ((i + 1) < allPointPos.Count)
            {
                if (Vector3.Distance(allPointPos[i], allPointPos[i + 1]) >= .4f)
                {
                    Vector3 newVec = Vector3.Lerp(allPointPos[i], allPointPos[i + 1], 0.5f);
                    allPointPos.Insert(i + 1, newVec);
                }
            }
        }
		lineRenderer.positionCount = allPointPos.Count;
        lineRenderer.SetPositions(allPointPos.ToArray());
		if (lineRenderer.positionCount <= 0)
        {
            selected = false;
        }
    }
    #endregion

    #region Detect nearby
    void NearbyObj()
    {
        if (landing)
            return;
        else
        {
            int oldLayer = gameObject.layer;
            gameObject.layer = 2;

            if (Physics2D.OverlapCircle(transform.position, checkRadius, vehicleLayer))
            {
                if (!playOnce)
                {
                    vehicle_Sound.PlayOneShot(sound_Collide);
                    playOnce = true;
                }
                outerRing.SetActive(true);
            }
            else if (!(Physics2D.OverlapCircle(transform.position, checkRadius, vehicleLayer)))
            {
                outerRing.SetActive(false);
                playOnce = false;
            }
            gameObject.layer = oldLayer;
        }
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.tag == "Vehicle")
        {
			GameManager.Instance.musicGame.Stop();
            vehicle_Sound.PlayOneShot(sound_Boom);
            if (GameManager.Instance.gameOver == false)
                GameManager.Instance.gameOver = true;
            Vector3 explodePos = (transform.position + target.transform.position) / 2;
            GameObject newExplosition = Instantiate(explode, explodePos, Quaternion.identity);
            GameManager.Instance.explosionEffect.Add(newExplosition);
        }
        if (target.tag == "LandingZone")
        {
            if (setFlyPath)
            {
                if (target.GetComponent<AirPort>().allowedType == vEHICLETYPE)
                {
                    if (vEHICLETYPE == VEHICLETYPE.HELICOPTER || vEHICLETYPE == VEHICLETYPE.AIRBALOON ||
                        vEHICLETYPE == VEHICLETYPE.GREENHELICOPTER)
                    {
                        target_Landing = target.transform.GetChild(0).position;
                        speed *= 0.1f;
                        VoidLandingSequence();
                    }

                    if (vEHICLETYPE == VEHICLETYPE.BOEING || vEHICLETYPE == VEHICLETYPE.GREENBOEING ||
                        vEHICLETYPE == VEHICLETYPE.PROPELLER || vEHICLETYPE == VEHICLETYPE.SONICPLANE ||
                        vEHICLETYPE == VEHICLETYPE.WATERPROPELLER)
                    {
                        if (transform.rotation.z <= target.transform.rotation.z + 0.1f &&
                            transform.rotation.z >= target.transform.rotation.z - 0.1f)
                        {
                            target_Landing = target.transform.GetChild(0).position;
                            VoidLandingSequence();
                        }
                    }
                }
            }
        }
    }

    IEnumerator LandingSequence()
    {
        for (float timer = 0; timer < scaleDownDuration; timer += Time.deltaTime)
        {
            if (transform.localScale.x > 0.7f)
            {
                transform.localScale -= new Vector3(0.01f, 0.01f, 0);
                yield return new WaitForSeconds(0.1f);
            }
        }
        GameManager.Instance.veh_List.Remove(this.gameObject);
        Destroy(gameObject);
    }

    void VoidLandingSequence()
    {
        outerRing.SetActive(false);
        lineRenderer.enabled = false;
        landing = true;
        allowToMove = false;
		isCalScore = false;
		GameManager.Instance.ScorePointCheck(score);
        if (this.gameObject.GetComponent<CircleCollider2D>() != null)
        {
            GetComponent<CircleCollider2D>().enabled = false;
        }
        else
            GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(LandingSequence());
    }

    private void OnTriggerStay2D(Collider2D target)
    {
        if (target.tag == "Vehicle")
        {
            if (outerRing.GetComponent<SpriteRenderer>().isVisible)
                outerRing.SetActive(false);
        }
    }
    #endregion

	public int getScore(){
		if (score <= 0)
			score = 0;
		return score;
	}

	void updateTextScore(){
//		textScore.text = "" + score;

	}
}
