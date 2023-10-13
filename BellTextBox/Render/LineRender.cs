using Bell.Data;
using Bell.Languages;

namespace Bell.Render;

public class LineRender
{
    //선택 여부, 자동완성 리스트, wrapped)
    // 커서, 선택 영역, find 결과 영역
    // 그릴 w, h 정보도 포함
    // -> 라인과 관련 없는 데이터는 포함하지 않도록 주의

    public int LineIndex;
    public int WrapIndex;
    public int RenderIndex;

    public readonly List<TextBlockRender> TextBlockRenders = new();
    
    public LineRender(int lineIndex, int wrapIndex)
    {
        LineIndex = lineIndex;
        WrapIndex = wrapIndex;
    }
}

public struct TextBlockRender
{
    public string Text;
    public ColorStyle ColorStyle;
    public float Width;
}