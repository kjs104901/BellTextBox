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
    private List<char> _buffers = new();
    
    public string String => _stringCache.Get();
    private readonly Cache<string> _stringCache;

    public Dictionary<int, FontStyle> Styles => _stylesCache.Get();
    private readonly Cache<Dictionary<int, FontStyle>> _stylesCache;
    
    public bool Foldable => _foldableCache.Get();
    private readonly Cache<bool> _foldableCache;

    public HashSet<int> Cutoffs => _cutoffsCache.Get();
    private readonly Cache<HashSet<int>> _cutoffsCache;
    
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

    private Dictionary<int, FontStyle> UpdateStyles(Dictionary<int, FontStyle> styles)
    {
        //TODO
        return styles;
    }

    private HashSet<int> UpdateCutoff(HashSet<int> cutoffs)
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

        LineRender lineRender = new LineRender();
        _buffers.Clear();
        
        FontStyle fontStyle = FontStyle.DefaultFontStyle;
        for (int i = 0; i < _chars.Count; i++)
        {
            _buffers.Add(_chars[i]);

            if (false == Styles.TryGetValue(i, out var charFontStyle))
                charFontStyle = FontStyle.DefaultFontStyle;
            
            bool nextStyle = fontStyle != charFontStyle;
            fontStyle = charFontStyle;

            if (Cutoffs.Contains(i))
            {
                lineRender.Text = String.Concat(_buffers);
                lineRenders.Add(lineRender);
                
                lineRender = new LineRender();
                _buffers.Clear();
            }
        }
        ;
        lineRender.Text = String.Concat(_buffers);
        lineRenders.Add(lineRender);

        return lineRenders;
    }
}