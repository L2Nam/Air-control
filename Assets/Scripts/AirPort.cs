using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPort : MonoBehaviour {

    private LANDZONETYPE lANDZONETYPE;
    public VEHICLETYPE allowedType;
    [SerializeField] GameObject circleWayroad;
    [SerializeField] GameObject squareWayroad;

    public void MakeLandingZone(int type)
    {
        lANDZONETYPE = (LANDZONETYPE)type;
        switch (lANDZONETYPE)
        {
            case (LANDZONETYPE.CIRCLEZONE):
                CircelType();
                break;
            case (LANDZONETYPE.SQUAREZONE):
                SquareType();
                break;
            case (LANDZONETYPE.WATERZONE):
                SquareType();
                break;
            case (LANDZONETYPE.GREENSQUAREZONE):
                SquareType();
                break;
            case (LANDZONETYPE.GREENCIRCLEZONE):
                CircelType();
                break;
            case (LANDZONETYPE.BIGCIRCLEZONE):
                CircelType();
                GetComponent<CircleCollider2D>().radius = 0.6f;
                break;
        }
    }

    void SquareType()
    {
        this.gameObject.AddComponent<BoxCollider2D>();
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(0.05f, 0.25f);
        transform.GetChild(2).position = new Vector3(transform.position.x + 1f, transform.position.y, 0);
        gameObject.layer = 9;
        transform.GetChild(0).position = new Vector3(transform.position.x + 3, transform.position.y, 0);
    }
    void CircelType()
    {
        this.gameObject.AddComponent<CircleCollider2D>();
        gameObject.layer = 8;
        transform.GetChild(0).position = transform.position;
    }
}
