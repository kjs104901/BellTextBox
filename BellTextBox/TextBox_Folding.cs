namespace Bell;

public struct Folding
{
    public int Start;
    public int End;
    
    public bool Folded;
}

public partial class TextBox
{
    private List<Folding> _foldings = new();
    
    
}