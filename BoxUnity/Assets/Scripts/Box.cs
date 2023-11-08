using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    [SerializeField]
    List<Sprite> spList;
    [SerializeField]
    Image img;

    private int boxIndex;

    private int curIndex;
    private bool isSelect;

    void Start()
    {
       // img = this.transform.Find("Image").GetComponent<Image>();
    }

    public void setBoxIndex(int boxIndex)
    {
        this.boxIndex = boxIndex;
    }

    public int getBoxIndex()
    {
        return boxIndex;
    }

    public void select(bool isSelect)
    {
        this.isSelect = isSelect;
    }

    public bool IsSelect()
    {
        return isSelect;
    }

    public void setSprite(int index)
    {
        curIndex = index;
        img.sprite = spList[index];
        img.gameObject.SetActive(false);
    }

    public void show(bool value)
    {
        img.gameObject.SetActive(value);
    }

    public int getIndex()
    {
        return curIndex;
    }

    public void destroy()
    {
        GameObject.Destroy(this.gameObject);
    }
}
