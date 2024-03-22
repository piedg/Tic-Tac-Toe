using UnityEngine;

public class BoardCellVisual : MonoBehaviour
{
    public SpriteRenderer Image;
    public Sprite Cross;
    public Sprite Circle;
    public BoardCell BoardCell;

    private void Update()
    {
        switch (BoardCell.CurrentSign)
        {
            case eSign.X:
                Image.sprite = Cross;
                break;
            case eSign.O:
                Image.sprite = Circle;
                break;
            case eSign.Empty:
                Image.sprite = null;
                break;
            default:
                Image.sprite = null;
                break;
        }
    }

    public void SetSprite(eSign sign)
    {
        switch (sign)
        {
            case eSign.X:
                Image.sprite = Cross;
                break;
            case eSign.O:
                Image.sprite = Circle;
                break;
            case eSign.Empty:
                Image.sprite = null;
                break;
            default:
                Image.sprite = null;
                break;
        }
    }

    public void ClearCell()
    {
        Image.sprite = null;
    }
}