using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eSign
{
    X, 
    O, 
    Empty
}

public class BoardCell : MonoBehaviour
{
    public SpriteRenderer Image;
    public bool IsAvailable;
    public eSign CurrentSign;
    public Sprite Cross;
    public Sprite Circle;

 /*   private void OnMouseDown()
    {
        if (GameManager.Instance.GameOver) return;

        if (!GameManager.Instance.IsAITurn)
        {
            if (IsAvailable)
            {
                SetSign(GameManager.Instance.Player);
                GameManager.Instance.IsAITurn = true;
            }
        }
    }*/

    public void SetSign(eSign Sign)
    {
        switch (Sign)
        {
            case eSign.X:
                Image.sprite = Cross;
                CurrentSign = eSign.X;
                break;
            case eSign.O:
                Image.sprite = Circle;
                CurrentSign = eSign.O;
                break;
            case eSign.Empty:
                Image.sprite = null;
                CurrentSign = eSign.Empty;
                break;
            default:
                Image.sprite = null;
                CurrentSign = eSign.Empty;
                break;
        }
        IsAvailable = false;
    }

    public void Unsign()
    {
        Image.sprite = null;
        CurrentSign = eSign.Empty; 
        IsAvailable = true;
    }
}
