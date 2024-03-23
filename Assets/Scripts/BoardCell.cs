public enum eSign
{
    X,
    O,
    Empty
}

public class BoardCell
{
    public bool IsAvailable { get; private set; }
    public eSign CurrentSign { get; private set; }

    public BoardCell()
    {
        Unsign();
    }

    public BoardCell(eSign sign)
    {
        SetSign(sign);

        if (sign == eSign.Empty)
        {
            IsAvailable = true;
        }
        else
        {
            IsAvailable = false;
        }
    }

    public void SetSign(eSign sign)
    {
        CurrentSign = sign;
        IsAvailable = false;
    }

    public void Unsign()
    {
        CurrentSign = eSign.Empty;
        IsAvailable = true;
    }

    public override string ToString()
    {
        return "IsAvailable " + IsAvailable + " \n" +
            "CurrentSign " + CurrentSign + " \n";
    }
}
