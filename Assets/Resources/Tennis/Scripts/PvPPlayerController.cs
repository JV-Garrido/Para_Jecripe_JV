using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab.DataModels;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
public class PvpPlayerController : MonoBehaviour
{
    public bool serveAnim;
    public bool serveHeld;
    public bool isServer;
    public bool turnFlag;
    public bool serveAnimLock;
    public bool rebatendo;
    public bool timeOut;
    
    private int moveSpeed;
    public int player;
    
    private float px, pz, nx, nz;
    private float rebatePower;
    public float saquePower;
    public float lastPower;
    private float red;
    private float green;
    
    private string hori, vert, serve;
    
    private Vector3 negativeV3;
    private Vector3 relForward;
    private Vector3 startingPoint;
    
    private Animator animator;
    public BallPhysics ball;
    public GameObject plane;
    private Collider cPlane;
    public Slider slider;
    public Image fill;

    void Start()
    {
        cPlane = plane.GetComponent<Collider>();
        ball = GameObject.Find("ballpvp").GetComponent<BallPhysics>();
        animator = GetComponent<Animator>();
        
        px = cPlane.bounds.max[0];
        pz = cPlane.bounds.max[2];
        nx = cPlane.bounds.min[0];
        nz = cPlane.bounds.min[2];
        
        moveSpeed = 6;
        rebatePower = 0f;
        saquePower = 0f;
        serveHeld = false;
        serveAnimLock = false;
        rebatendo = false;
        timeOut = false;
        negativeV3 = new Vector3(-1, -1, -1);
        slider.value = 0f;
        
        if (player == 1)
        {
            hori = "Horizontal";
            vert = "Vertical";
            serve = "ServeP1";
            relForward = new Vector3(0, 0, 1);
            turnFlag = true;
            serveAnim = false;
            startingPoint = new Vector3(2f, 0.519f, -12.393f);
        }
        else if (player == 2)
        {
            hori = "HorizontalP2";
            vert = "VerticalP2";
            serve = "ServeP2";
            relForward = new Vector3(0, 0, -1);
            turnFlag = false;
            serveAnim = true;
            startingPoint = new Vector3(-2f, 0.519f, 12.393f);
        }
    }

    void Update()
    {
        if (transform.position.x > px) 
        { 
            transform.position = new Vector3(px, transform.position.y, transform.position.z); 
        }
        if (transform.position.x < nx) 
        { 
            transform.position = new Vector3(nx, transform.position.y, transform.position.z); 
        }
        
        if (!serveAnimLock && serveAnim) 
        {
            Mover();
        }
        
        if (Input.GetAxisRaw(serve) != 0 && turnFlag && !serveAnimLock && !timeOut)
        {
            if (serveAnim == true)
            {
                serveHeld = true;
                rebatePower += Time.deltaTime;
                slider.value = Math.Min(slider.value + Time.deltaTime, 1.5f);
                // slider.value = slider.value + Time.deltaTime;
                // if (slider.value > 1.5f)
                // {
                //     slider.value = 1.5f;
                // }
            }
            
            // serveHeld = !(serveHeld && !serveAnim);
            if (serveHeld && !serveAnim)
            // if (serveHeld == true && serveAnim == false)
            {
                serveHeld = false;
            }
            
            if (!serveAnim && !serveHeld)
            // if (serveAnim == false && serveHeld == false)
            {
                saquePower += Time.deltaTime;
                slider.value += Time.deltaTime;
            }
        }

        if (!serveAnim && !serveHeld && Input.GetAxisRaw(serve) == 0 && turnFlag && !serveAnimLock && saquePower != 0)
        {
            saquePower = Math.Min(saquePower, 1.5f);
            // if (saquePower > 1.5f) 
            // { 
            //     saquePower = 1.5f; 
            // }
            serveAnimLock = true;
            Sacar();
            serveAnim = true;
            slider.value = 0f;
        }

        if ((Input.GetAxisRaw(serve) == 0 || timeOut) && turnFlag && !serveAnimLock && serveAnim && serveHeld)
        {
            RebatePrep();
        }

        if (slider.value > 0.75f)
        {
            red = 255f;
            green = 255f * (1f - ((slider.value - 0.75f) / 0.75f));
        }
        else
        {
            green = 255f;
            red = 255f * 1.333f * slider.value;
        }
        fill.color = new Color32(Convert.ToByte(red), Convert.ToByte(green), 0, 255);
    }

    private void Mover()
    {
        Vector3 movement = new Vector3(Input.GetAxisRaw(hori), 0, 0).normalized;
        
        if (player == 2) 
        { 
            movement = Vector3.Scale(movement, negativeV3); 
        }
        
        if (movement != Vector3.zero && !timeOut) 
        { 
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(movement, negativeV3)), 0.15F); 
        }
        
        if (!timeOut) 
        { 
            transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World); 
        }
        
        if (movement != Vector3.zero && !timeOut)
        {
            animator.SetTrigger("Move");
            animator.SetFloat("speed", 1);
        }
        else
        {
            animator.SetFloat("speed", 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.Scale(relForward, negativeV3), Vector3.up), 0.05f);
        }
    }

    private void Sacar()
    {
        animator.SetBool("isServing", true);
        animator.SetTrigger("Serve");
        Invoke(nameof(NoMoreServe), 1.6f);
    }

    private void NoMoreServe()
    {
        animator.SetBool("isServing", false);
        serveAnimLock = false;
    }

    public void RebatePrep()
    {
        Rebate();
        serveHeld = false;
        slider.value = 0f;
    }

    private void Rebate()
    {
        if (!rebatendo){
            if (rebatePower >= 1.5f)
            {
                lastPower = 1.5f;
            }
            else
            {
                lastPower = 0.6f + (rebatePower * 0.6f);
            }
            animator.SetTrigger("Forehand_front");
            Invoke(nameof(NoMoreRebate), 1f);
        }

        rebatendo = true;
        rebatePower = 0f;
    }

    private void NoMoreRebate()
    {
        rebatendo = false;
    }

    public void NewPoint(bool serveC)
    {
        serveAnim = false;
        serveHeld = false;
        serveAnimLock = false;
        rebatendo = false;
        timeOut = false;
        
        startingPoint.x *= -1f;
        transform.position = startingPoint;
        
        if (player == 1)
        {
            turnFlag = serveC;
            if (!serveC) serveAnim = true;
            // if (serveC)
            // {
            //     turnFlag = true;
            // }
            // else
            // {
            //     turnFlag = false;
            //     serveAnim = true;
            // }
        }
        else if (player == 2)
        {
            turnFlag = !serveC;
            if (serveC) serveAnim = true;
            // if (serveC)
            // {
            //     turnFlag = false;
            //     serveAnim = true;
            // }
            // else
            // {
            //     turnFlag = true;
            // }
        }
    }
}