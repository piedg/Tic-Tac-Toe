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
}
