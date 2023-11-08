using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gou : MonoBehaviour
{

    [SerializeField]
    List<Sprite> spriteList;

    Image imgGou;

    bool isMove;
    Rigidbody2D rb;

    EdgeCollider2D eCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "box" && isDown)
        {
            Box box = collision.GetComponent<Box>();
            //Debug.LogError(box.getBoxIndex() + "   " + this.boxIndex);
            if (box.getBoxIndex() != this.boxIndex)
                return;

            imgGou.sprite = spriteList[2];
            isDown = false;
            isMove = true;

            rb = collision.transform.GetComponent<Rigidbody2D>();
            //collision.transform.SetParent(this.transform);
            //collision.transform.GetComponent<Rigidbody2D>().simulated = false;
            // collision.transform.GetComponent<Rigidbody2D>().MovePosition(this.transform.position);



            eCollider.enabled = false;
            transform.DOLocalMoveY(sourcePosY, 0.5f).OnComplete(() =>
            {
                // eCollider.enabled = true;
                imgGou.sprite = spriteList[0];
                SendMessageUpwards("CheckOver", box, SendMessageOptions.RequireReceiver);
                isMove = false;
            });

            SendMessageUpwards("Check", box, SendMessageOptions.RequireReceiver);
        }        
    }

    float sourcePosY;

    private void Start()
    {
        sourcePosY = this.transform.localPosition.y;

        eCollider = this.transform.GetComponent<EdgeCollider2D>();
        imgGou = this.transform.Find("Image").GetComponent<Image>();
    }

    bool isDown = false;
    float x, y;

    private void Update()
    {
        if(isDown)
        {
            y -= 13;
            transform.localPosition = new Vector3(x, y, 0);
            if(y <= 220)
            {
                isDown = false;
                transform.DOLocalMoveY(sourcePosY, 0.5f).OnComplete(() =>
                {
                    
                });
                SendMessageUpwards("GouFailed", null, SendMessageOptions.RequireReceiver);
                SendMessageUpwards("CheckOver", null, SendMessageOptions.RequireReceiver);
            }
        }
        if(isMove)
        {
            rb.transform.GetComponent<Rigidbody2D>().MovePosition(this.transform.Find("pos").position);
        }
    }

    private int boxIndex;

    public void down(int index)
    {
        isMove = false;
        boxIndex = index;
        isDown = true;
        y = this.transform.localPosition.y;
        x = this.transform.localPosition.x;
        eCollider.enabled = true;
    }
}
