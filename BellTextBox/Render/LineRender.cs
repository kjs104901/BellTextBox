namespace Bell.Render;

public struct LineRender
{
    //uint/*line*/,uint/*wrap*/
    //(위치, 줄, 선택 여부, 마커, highlighter (범위 + 스타일 index), 자동완성 리스트, wrapped)
    // 커서, 선택 영역, find 결과 영역
    // 그릴 w, h 정보도 포함
    public string Text;
}