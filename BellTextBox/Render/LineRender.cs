using Bell.Data;
using Bell.Languages;

namespace Bell.Render;

public struct LineRender
{
    //선택 여부, 자동완성 리스트, wrapped)
    // 커서, 선택 영역, find 결과 영역
    // 그릴 w, h 정보도 포함
    // -> 라인과 관련 없는 데이터는 포함하지 않도록 주의

    // 텍스트 라인 줄
    public int LineIndex; 

    // 위치
    public float PosX;
    public float PosY;

    public List<TextBlockRender> TextBlockRenders;

    public static LineRender NullLineRender = new()
        { TextBlockRenders = new List<TextBlockRender>() { new() { Text = "!!ERROR!!" } } };

    public static LineRender Create()
    {
        return new LineRender()
        {
            TextBlockRenders = new()
        };
    }
}

public struct TextBlockRender
{
    public string Text;
    public FontStyle FontStyle;
    public float Width;
}