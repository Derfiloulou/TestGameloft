﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    static GameManager mInst;
    static public GameManager instance { get { return mInst; } }

    public GameObject[] Enemies;
    
    void Awake()
    {
        if (mInst == null) mInst = this;
        //DontDestroyOnLoad(this);
    }

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
