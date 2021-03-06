﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Behaviour : MonoBehaviour {

    [Header("Puntos para el moviento del Boss")]
    public Transform[] WayPoints = new Transform[7];
    public Transform PuntoSpawn;
    [Header("Atributos del Boss")]
    [Header("El anguloAperturaDisparo debe ser un número menor a 45")]
    [Header("La rotaciónLocalProyectil se encarga de rotar el sprite,")]
    [Header("si se pone un valor positivo el angulo se hace mas pequeño.")]
    public int anguloAperturaDisparo;
    public int rotaciónLocalProyectil;
    public float movementSpeed;
    public GameObject Proyectil, habilidad;
    public int currentWayPoint = 0;
    public int numberProyectiles;
    public float tempResting = 3f;


    [HideInInspector]
    public static bool Flip;
	[HideInInspector]
	public bool deadboss2=false;
    
    
    private Transform nextWayPoint;
    private float varSpeedUp;
    public float tempShooting = 3f;
    public float tempIdle = 3f;
    private bool IsShooting = false;
    private bool IsIdle = false;
    private Vector3 tempPos = new Vector3();
    private float amplitude = 0.01f;
    private float frequency = 0.5f;
    private float acceleration = 0.5f;
    private float angleOffset;
    private float minAngle;
    private bool MustRest;
    private GameObject player;
    private SpriteRenderer bossSprite;



    void Start ()
    {
        bossSprite = gameObject.GetComponent<SpriteRenderer>();
        player = GameObject.Find("Player");
        Collider2D colBoss = gameObject.GetComponent<Collider2D>();
        Collider2D colPro = Proyectil.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(colBoss,colPro);
        varSpeedUp = movementSpeed;
        
        if (movementSpeed == 0) movementSpeed = 5;
	}
	void Update ()
    {
		if (GetComponent<EnemyLifeSystem> ().CurrentHealth > 0)
        {
			Debug.DrawLine (PuntoSpawn.transform.position, player.transform.position, Color.red);

			if (currentWayPoint < WayPoints.Length)
            {
				if (nextWayPoint == null)
					nextWayPoint = WayPoints [currentWayPoint];
				switch (currentWayPoint)
                {
				    case 2:
				    case 6:
					    if (!IsShooting) {
						    Shoot ();
                        

						    IsShooting = true;
					    }
					    if (tempShooting > 0 && IsShooting == true) {
						    tempShooting = tempShooting - Time.deltaTime;
						    IsShooting = true;
					    } else {
						    IsShooting = false;
						    tempShooting = 3f;
					    }
					    if (tempIdle > 0) {
						    Idle ();
						    tempIdle = tempIdle - Time.deltaTime;
						    IsIdle = true;
					    } else {
						    MoveBoss (movementSpeed);
					    }                   
					    break;
				    case 5:
					    bossSprite.flipX = true;
                    
					    MoveBoss (movementSpeed);
					    RestartValues ();
					    break;
				    case 1:
					    MoveBoss (movementSpeed);
					    RestartValues ();
					    break;
				    case 0:
					    bossSprite.flipX = false;
					    if (!MustRest) {
						    MoveBoss (movementSpeed);
					    } else {
						    if (tempResting < 0) {
							    MoveBoss (movementSpeed);
						    } else {
							    MoveBoss (0.5f);
						    }
						    tempResting = tempResting - Time.deltaTime;
					    }
                    
					    break;
				    case 7:
					    if (varSpeedUp != 0f && varSpeedUp < 30f)
						    varSpeedUp = varSpeedUp + acceleration;
					    MustRest = true;
					    MoveBoss (varSpeedUp);
					    break;

				    default:
					    MoveBoss (movementSpeed);
					    break;
				}

		
			}
		}
		
	}
	void OnDestroy()
    {
		Instantiate (habilidad.gameObject);
	}
	void MoveBoss(float sp)
    {
        transform.position = Vector3.MoveTowards(transform.position, nextWayPoint.position, sp * Time.deltaTime);

        if(transform.position == nextWayPoint.position)
        {
            
            currentWayPoint = (currentWayPoint + 1) % WayPoints.Length;
            nextWayPoint = WayPoints[currentWayPoint];
        }
        
    }
    private void Shoot()
    {
        Flip = bossSprite.flipX;
        //Rotacion del boss hacia el player
        Vector3 direcc = player.transform.position - PuntoSpawn.transform.position;
        float rotz = LookAtPlayer(bossSprite.flipX,direcc);
        transform.rotation = Quaternion.AngleAxis(rotz, Vector3.forward);

        angleOffset = 90 / numberProyectiles;
        minAngle = rotz + ((numberProyectiles / 2) * angleOffset);
        
        //Hacia positivos min angulo = rotz - (numberProyectiles / 2) * angleOffset
        for (int i = 0; i < numberProyectiles; i++)
        {
            GameObject Proyectiles = (GameObject)Instantiate(Proyectil, PuntoSpawn.transform.position, Quaternion.identity);
            Proyectiles.transform.rotation = Quaternion.Euler(0, 0, minAngle - (angleOffset * i));

        }
       
    }

    private void Idle()
    {
        tempPos = transform.position;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = tempPos;
    }

    private void RestartValues()
    {
        IsShooting = false;
        IsIdle = false;
        tempIdle = 3f;
        tempShooting = 3f;
        tempResting = 3f;
        varSpeedUp = 1f;
    }

    private float LookAtPlayer(bool flip,Vector3 angBase)
    {
        
        angBase.Normalize();
        float rotz = Mathf.Atan2(angBase.y, angBase.x) * Mathf.Rad2Deg;

        if(!flip)
        {
            rotz = rotz + 180;
        }
        
        return rotz;
        
    }
    
    

}
