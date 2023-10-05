using System.Runtime.InteropServices;
using System.Text;
using Bell.Languages;
using Bell.Render;

namespace Bell.Data;

[Flags]
public enum Marker
{
    None = 0,
    
    Fold = 1 << 0,
    Unfold = 1 << 1
}

public class Line
{
    private readonly TextBox _textBox;
    
    public uint Index = 0;
    
    private List<char> _chars = new();
    
    public string String => _stringCache.Get();
    private readonly Cache<string> _stringCache;

    public Dictionary<uint, Style> Styles => _stylesCache.Get();
    private readonly Cache<Dictionary<uint, Style>> _stylesCache;
    
    public bool Foldable => _foldableCache.Get();
    private readonly Cache<bool> _foldableCache;

    public List<uint> Cutoffs => _cutoffsCache.Get();
    private readonly Cache<List<uint>> _cutoffsCache;
    
    public List<LineRender> LineRenders => _lineRendersCache.Get();
    private readonly Cache<List<LineRender>> _lineRendersCache;

    public bool Visible = false;
    public bool Folded = false;
    
    public int ViewCount => Visible ? Cutoffs.Count + 1 : 0;

    private Marker Marker
    {
        get
        {
            if (Visible)
            {
                if (Folded)
                    return Marker.Unfold;
                if (Foldable)
                    return Marker.Fold;
            }
            return Marker.None;
        }
    }

    public Line(TextBox textBox)
    {
        _textBox = textBox;
        _stylesCache = new(new(), UpdateStyles);
        _cutoffsCache = new(new(), UpdateCutoff);
        _foldableCache = new(false, UpdateFoldable);
        _stringCache = new(string.Empty, UpdateString);
        _lineRendersCache = new(new List<LineRender>(), UpdateLineRenders);
    }

    public void SetString(string line)
    {
        _chars.Clear();
        _chars.AddRange(line);
        
        _foldableCache.SetDirty();
        _stringCache.SetDirty();
        _lineRendersCache.SetDirty();
    }

    private Dictionary<uint, Style> UpdateStyles(Dictionary<uint, Style> styles)
    {
        //TODO
        return styles;
    }

    private List<uint> UpdateCutoff(List<uint> cutoffs)
    {
        //TODO calculate cutoff
        cutoffs.Clear();
        return cutoffs;
    }

    private bool UpdateFoldable(bool _)
    {
        var trimmedString = String.TrimStart();
        foreach (Block folding in _textBox.Language.Foldings)
        {
            if (trimmedString.StartsWith(folding.Start))
                return true;
        }
        return false;
    }

    private string UpdateString(string _)
    {
        _textBox.StringBuilder.Clear();
        _textBox.StringBuilder.Append(CollectionsMarshal.AsSpan(_chars));
        return _textBox.StringBuilder.ToString();
    }

    public LineRender GetLineRender(int wrapIndex)
    {
        //TODO check range?
        return LineRenders[wrapIndex];
    }
    
    private List<LineRender> UpdateLineRenders(List<LineRender> lineRenders)
    {
        lineRenders.Clear();
        lineRenders.Add(new LineRender
        {
            Text = String
        });
        return lineRenders;
    }
}