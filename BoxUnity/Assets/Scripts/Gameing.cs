using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public enum GameStatus
{
    Wait,
    Start,
    ClickFirst,
    ClickSecond,
    Anim,
    CheckOverFirst,
    CheckOverSecond,
    End,
}

public class Gameing : MonoBehaviour
{

    [SerializeField]
    AudioSource audioSource;

    [SerializeField]
    List<AudioClip> clipList;

    [SerializeField]
    Button btnHome, btnVolume, btnHelp, btnStart;

    [SerializeField]
    Text lblCount, lblLevel;

    [SerializeField]
    List<Transform> heartList;

    [SerializeField]
    List<GameObject> boxPrefabList;

    [SerializeField]
    Transform spGou1, spGou2;

    [SerializeField]
    Transform boxRoot;

    [SerializeField]
    GameObject helpLayer;

    private List<Box> boxList = new List<Box>();

    private Vector3 firstGouPos = new Vector3(-465.7f, 755.3f, 0);
    private Vector3 secondGouPos = new Vector3(465.7f, 755.3f, 0);

    private GameStatus gameStatus;

    private int killBoxCount = 0;
    private int curLevel;
    private int heart = 20;
    private int continueKill = 0; //ﬂB¿m≈‰å¶

    private void Start()
    {
        
        btnStart.onClick.AddListener(delegate ()
        {
            onBtnClick("btnStart");
        });

        init();
    }

    private void init()
    {

        firstBox = null;
        secondBox = null;

        foreach (Transform trans in heartList)
        {
            trans.GetComponent<Image>().enabled = false;
            trans.Find("Image").gameObject.SetActive(true);
        }

        for(int i = boxList.Count - 1;i >=0;i--)
        {
            GameObject.Destroy(boxList[i].gameObject);
        }
        boxList.Clear();       
        
        heart = 20;
        updateHeart();

        curLevel = 1;
        lblLevel.text = curLevel.ToString();

        continueKill = 0;
        killBoxCount = 0;
        lblCount.text = killBoxCount.ToString();
        btnStart.gameObject.SetActive(true);

        gameStatus = GameStatus.Wait;
    }

    private void gameStart()
    {        
        boxIndex = 0;
        lblLevel.text = curLevel.ToString();
        btnStart.gameObject.SetActive(false);

        gameStatus = GameStatus.Start;

        for(int i = 0; i < curLevel; i++)
        {
            int rand = UnityEngine.Random.Range(0, 15);
            createBox(rand);

            createBox(rand);
        }

        spGou1.transform.localPosition = new Vector3(firstGouPos.x - 1000, firstGouPos.y, 0);
        spGou2.transform.localPosition = new Vector3(secondGouPos.x + 1000, secondGouPos.y, 0);
        spGou1.DOLocalMoveX(firstGouPos.x, 1);
        spGou2.DOLocalMoveX(secondGouPos.x, 1);
    }

    int boxIndex = 0;

    private void createBox(int index)
    {
        boxIndex++;

        int rand = UnityEngine.Random.Range(0, boxPrefabList.Count);
        GameObject boxGo = GameObject.Instantiate(boxPrefabList[rand]);
        boxGo.gameObject.SetActive(true);
        Transform boxTrans = boxGo.transform;
        boxTrans.SetParent(boxRoot);
        boxTrans.localPosition = new Vector3(UnityEngine.Random.Range(-500.0f, 500.0f), 600, 0);
        boxTrans.localScale = Vector3.one;
        boxTrans.localRotation = Quaternion.identity;
        Box box = boxTrans.GetComponent<Box>();
        box.setBoxIndex(boxIndex);
        boxTrans.GetComponent<Button>().onClick.AddListener(delegate()
        {
            onBoxClick(box);
        });
       
        
        box.setSprite(index);
        //box.show(true);
        boxList.Add(box);
    }

    Box firstBox, secondBox;

    private void onBoxClick(Box box)
    {
        audioSource.PlayOneShot(clipList[0]);
        if (box.IsSelect()) return;
        
        if (gameStatus == GameStatus.Start)
        {
            spGou1.transform.DOLocalMoveX(box.transform.localPosition.x, 0.5f).OnComplete(() =>
            {
                
            });
            firstBox = box;
            box.select(true);
            box.show(true);
            gameStatus = GameStatus.ClickFirst;
        }
        else if(gameStatus == GameStatus.ClickFirst)
        {
            secondBox = box;
            spGou2.transform.DOLocalMoveX(box.transform.localPosition.x, 0.5f).OnComplete(() =>
            {
                gameStatus = GameStatus.Anim;
                spGou1.GetComponent<Gou>().down(firstBox.getBoxIndex());
                spGou2.GetComponent<Gou>().down(secondBox.getBoxIndex());                
            });
            
            box.select(true);
            box.show(true);
            gameStatus = GameStatus.ClickSecond;
        }
    }

    private void Check(Box box)
    {
        //if (null == catchFirstBox)
        //    catchFirstBox = box;
        //else
        //    catchSecondBox = box;
        //box.show(true);
    }

    private void CheckOver()
    {
        if (gameStatus == GameStatus.Anim)
        {
            gameStatus = GameStatus.CheckOverFirst;
            return;
        }
        else
            gameStatus = GameStatus.CheckOverSecond;

        if (firstBox.getIndex() == secondBox.getIndex() && !isFailed)
        {
            audioSource.PlayOneShot(clipList[1]);
            // catchFirstBox
            boxList.Remove(firstBox);
            boxList.Remove(secondBox);
            firstBox.destroy();
            secondBox.destroy();
            killBoxCount += 2;
            lblCount.text = killBoxCount.ToString();
            continueKill++;
            if (continueKill > 1)
                heart += 2;
            else
                heart++;
            if (heart > 20)
                heart = 20;

            updateHeart();

            if (boxList.Count == 0)
            {
                levelUp();
            }
            else
            {
                gameStatus = GameStatus.Start;
            }            
        }
        else
        {
            firstBox.select(false);
            secondBox.select(false);

            firstBox.show(false);
            secondBox.show(false);

            firstBox.transform.SetParent(boxRoot);
            firstBox.transform.GetComponent<Rigidbody2D>().simulated = true;
            secondBox.transform.SetParent(boxRoot);
            secondBox.transform.GetComponent<Rigidbody2D>().simulated = true;
            isFailed = false;
            continueKill = 0;
            heart--;
            updateHeart();

            if(heart <= 0)
            {
                init();
                return;
            }
        }
        firstBox = null;
        secondBox = null;

        gameStatus = GameStatus.Start;
    }
    private bool isFailed = false;
    private void GouFailed()
    {
        isFailed = true;
    }

    private void updateHeart()
    {
        for (int i = heartList.Count - 1; i >= 0; i--)
        {
            if (i >= heart)
            {
                heartList[i].GetComponent<Image>().enabled = true;
                heartList[i].Find("Image").gameObject.SetActive(false);
            }
            else
            {
                heartList[i].GetComponent<Image>().enabled = false;
                heartList[i].Find("Image").gameObject.SetActive(true);
            }
        }
    }

    private void levelUp()
    {
        curLevel++;
        if (curLevel > 15)
            curLevel = 1;
        

        btnStart.gameObject.SetActive(true);

    }

    public void onBtnClick(string name)
    {
        audioSource.PlayOneShot(clipList[0]);
        if (name == "btnStart")
        {
            gameStart();
        }
        else if(name == "btnHome")
        {
            clearAll();
            SceneManager.LoadSceneAsync("LoginScene");
        }
        else if(name == "btnHelp")
        {
            helpLayer.gameObject.SetActive(true);
        }
        else if(name == "btnVolume")
        {
            Loading.volumOpen = !Loading.volumOpen;
            audioSource.volume = Loading.volumOpen ? 1 : 0;
            btnVolume.transform.Find("spDisable").gameObject.SetActive(!Loading.volumOpen);
        }
        else if(name == "btnOk")
        {
            helpLayer.gameObject.SetActive(false);
        }
    }

    private void clearAll()
    {
        boxList.Clear();
    }
}
