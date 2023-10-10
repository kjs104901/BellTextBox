using Bell.Data;
using Bell.Languages;

namespace Bell.Render;

public struct LineRender
{
    //선택 여부, 자동완성 리스트, wrapped)
    // 커서, 선택 영역, find 결과 영역
    // 그릴 w, h 정보도 포함

    public int Index; // 실제 줄
    public int LineIndex; // 텍스트 라인 줄
    public int RenderIndex; // 텍스트 라인 wrap 된 줄

    // 위치
    public float PosX;
    public float PosY;

    // 마커
    public Marker Marker;

    public List<TextBlockRender> TextBlockRenders;

    public static LineRender NullLineRender = new()
        { TextBlockRenders = new List<TextBlockRender>() { new() { Text = "!!ERROR!!" } } };

    public static LineRender Create()
    {
        return new LineRender()
        {
            Marker = Marker.None,
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