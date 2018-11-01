using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class exitStage : MonoBehaviour {

    // door logic
    public BoxCollider2D bc;
    public bool canExit;

    // level loading
    int level;

    // fade out timing
    WaitForSeconds fadeOut = new WaitForSeconds(0.05f);

    // aButton to advance cue
    public SpriteRenderer aButtonSR;

    // space button    
    public SpriteRenderer spaceButtonSR;

    // overlay
    public SpriteRenderer blackOverlaySR;

    public GameObject advanceContainer;

    GameObject keycardReaderGO;
    keycardReader kReader;

    public bool lockedDoorSettings;

    AsyncOperation asyncLoad;

    // Use this for initialization
    void Start() {
        hideObjects();
        canExit = false;
        if (lockedDoorSettings)
        {            
            lockedDoorSetup();            
        }
    }    

    void lockedDoorSetup()
    {
        keycardReaderGO = GameObject.Find("keycardReaderCollider");
        kReader = keycardReaderGO.GetComponent<keycardReader>();
    }

    void hideObjects()
    {
        advanceContainer.SetActive(false);
        aButtonSR.enabled = false;
        spaceButtonSR.enabled = false;
        blackOverlaySR.enabled = false;
    }
    int i = 0;
    void Update()
    {
        
        if (lockedDoorSettings)
        {

            if (Input.GetKeyDown(KeyCode.Space) && canExit && kReader.doorUnlocked || Input.GetAxis("SubmitJS") > 0 && canExit && kReader.doorUnlocked)
            {

                GameObject j = GameObject.Find("JohnnyChainsaw");
                moveJohnnyUpdated mJU = j.GetComponent<moveJohnnyUpdated>();
                mJU.canMove = false;

                //stop from firing again
                canExit = false;

                //get levelname
                string sceneName = SceneManager.GetActiveScene().name;
                sceneName = sceneName.Substring(5);
                level = Int32.Parse(sceneName);

                //add one so we can just go right to it
                level += 1;

                //fadeOut and load it
                StartCoroutine("levelFadeOut");

            }
        }
        else
        {            
            if (Input.GetKeyDown(KeyCode.Space) && canExit || Input.GetAxis("SubmitJS") > 0 && canExit)
            {

                GameObject j = GameObject.Find("JohnnyChainsaw");
                moveJohnnyUpdated mJU = j.GetComponent<moveJohnnyUpdated>();
                mJU.canMove = false;

                //stop from firing again
                canExit = false;

                //get levelname
                string sceneName = SceneManager.GetActiveScene().name;
                sceneName = sceneName.Substring(5);
                level = Int32.Parse(sceneName);

                //add one so we can just go right to it
                level += 1;

                //fadeOut and load it
                StartCoroutine("levelFadeOut");
            }
        }
        
    }

    IEnumerator levelFadeOut()
    {                

        //save completed level to memory for continue script        
        PlayerPrefs.SetInt("continueLevel", level);        
        PlayerPrefs.Save();
                
        //begin fade in
        blackOverlaySR.enabled = true;
        while(blackOverlaySR.color.a < 1)
        {
            blackOverlaySR.color = new Color(1, 1, 1, blackOverlaySR.color.a + 0.04f);
            yield return fadeOut;
        }

        // load scene
        asyncLoad = SceneManager.LoadSceneAsync("level" + level);

        //Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "JohnnyHitBox")
        {
            canExit = true;            

            advanceContainer.SetActive(true);
            if (controllerUsed.controller)
            {
                aButtonSR.enabled = true;
                spaceButtonSR.enabled = false;
            }
            else
            {
                aButtonSR.enabled = false;
                spaceButtonSR.enabled = true;
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "JohnnyHitBox")
        {
            canExit = false;

            advanceContainer.SetActive(false);
            aButtonSR.enabled = false;
            spaceButtonSR.enabled = false;
        }
    }

}
