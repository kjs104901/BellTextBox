namespace Bell.Data;

public class Folding
{
    public int Start;
    public int End;
    
    public bool Folded;

    public void Switch()
    {
        Folded = !Folded;
    }
    
    public static readonly Folding None = new Folding() { Start = -1, End = -1, Folded = false };
}