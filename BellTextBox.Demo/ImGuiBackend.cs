using Bell.Render;
using Bell;
using ImGuiNET;
using System.Numerics;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;

namespace BellTextBox.Demo;

public class ImGuiBackend : TextBoxBackend
{
    public Vector2 Size;

    private Vector2 _contentSize;
    private Vector2 _scroll;
    private ImDrawListPtr _drawList;

    private Vector2 _screenPos;

    public override void SetClipboard(string text)
    {
        throw new NotImplementedException();
    }

    public override string GetClipboard()
    {
        throw new NotImplementedException();
    }

    public override RectSize GetCharRenderSize(char c)
    {
        Vector2 fontSize = ImGui.CalcTextSize(c.ToString());
        return new RectSize { Width = fontSize.X, Height = fontSize.Y };
    }

    private readonly List<ValueTuple<ImGuiKey, HotKeys>> _keyboardMapping = new()
    {
        (ImGuiKey.A, HotKeys.A),
        (ImGuiKey.C, HotKeys.C),
        (ImGuiKey.V, HotKeys.V),
        (ImGuiKey.X, HotKeys.X),
        (ImGuiKey.Y, HotKeys.Y),
        (ImGuiKey.Z, HotKeys.Z),

        (ImGuiKey.UpArrow, HotKeys.UpArrow),
        (ImGuiKey.DownArrow, HotKeys.DownArrow),
        (ImGuiKey.LeftArrow, HotKeys.LeftArrow),
        (ImGuiKey.RightArrow, HotKeys.RightArrow),

        (ImGuiKey.PageUp, HotKeys.PageUp),
        (ImGuiKey.PageDown, HotKeys.PageDown),
        (ImGuiKey.Home, HotKeys.Home),
        (ImGuiKey.End, HotKeys.End),
        (ImGuiKey.Insert, HotKeys.Insert),

        (ImGuiKey.Delete, HotKeys.Delete),
        (ImGuiKey.Backspace, HotKeys.Backspace),
        (ImGuiKey.Enter, HotKeys.Enter),
        (ImGuiKey.Tab, HotKeys.Tab),
    };

    public ImGuiBackend()
    {
        KeyboardInput.Chars = new();
    }

    public override void Input()
    {
        // TODO make reset func
        KeyboardInput.HotKeys = HotKeys.None;
        KeyboardInput.Chars.Clear();

        MouseInput.X = 0.0f;
        MouseInput.Y = 0.0f;
        MouseInput.MouseKey = MouseKey.None;

        if (ImGui.IsWindowFocused())
        {
            var io = ImGui.GetIO();
            if (io.KeyShift)
                KeyboardInput.HotKeys |= HotKeys.Shift;
            if (io.KeyCtrl)
                KeyboardInput.HotKeys |= HotKeys.Ctrl;
            if (io.KeyAlt)
                KeyboardInput.HotKeys |= HotKeys.Alt;
            foreach (var keyMap in _keyboardMapping)
            {
                if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(keyMap.Item1)))
                    KeyboardInput.HotKeys |= keyMap.Item2;
            }

            for (int i = 0; i < io.InputQueueCharacters.Size; i++)
            {
                KeyboardInput.Chars.Add((char)io.InputQueueCharacters[i]);
            }

            var mouse = ImGui.GetMousePos();
            MouseInput.X = mouse.X - _screenPos.X;
            MouseInput.Y = mouse.Y - _screenPos.Y;

            ImGui.SetMouseCursor(ImGuiMouseCursor.TextInput);

            if (ImGui.IsMouseClicked(0))
                MouseInput.MouseKey = MouseKey.Click;
            if (ImGui.IsMouseDoubleClicked(0))
                MouseInput.MouseKey = MouseKey.DoubleClick;
            if (ImGui.IsMouseDragging(0) && ImGui.IsMouseDown(0))
                MouseInput.MouseKey = MouseKey.Dragging;
        }

        ViewInput.X = _scroll.X;
        ViewInput.Y = _scroll.Y;
        ViewInput.W = _contentSize.X;
        ViewInput.H = _contentSize.Y;
    }

    public override void StartTextBox()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, new Vector2(0, 0));

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(0, 0));

        ImGui.BeginChild("##TextBox", Size, true, ImGuiWindowFlags.HorizontalScrollbar);
        _contentSize = ImGui.GetWindowContentRegionMax();

        ImGui.SetNextWindowSize(new Vector2(_contentSize.X, _contentSize.Y));
        ImGui.Begin("##TextBoxWindow",
            ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.HorizontalScrollbar | ImGuiWindowFlags.NoNavFocus | ImGuiWindowFlags.NoNavInputs |
            ImGuiWindowFlags.ChildWindow);

        _scroll.X = ImGui.GetScrollX();
        _scroll.Y = ImGui.GetScrollY();

        _drawList = ImGui.GetWindowDrawList();
    }

    public override void EndTextBox()
    {
        ImGui.End();
        ImGui.PopStyleVar(5);
        ImGui.EndChild();
    }

    public override void StartPage()
    {
        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.2f, 0.1f, 0.1f, 1.0f));
        ImGui.BeginChild("##Page", new Vector2(PageRender.Size.Width, PageRender.Size.Height), false,
            ImGuiWindowFlags.NoScrollbar);

        _screenPos = ImGui.GetCursorScreenPos();
    }

    public override void EndPage()
    {
        ImGui.EndChild();
        ImGui.PopStyleColor();
    }

    public override void RenderText(Vector2 pos, string text, FontStyle fontStyle)
    {
        var color = new Vector4(fontStyle.R, fontStyle.G, fontStyle.B, fontStyle.A);
        _drawList.AddText(new Vector2(_screenPos.X + pos.X, _screenPos.Y + pos.Y),
            ImGui.ColorConvertFloat4ToU32(color), text);
    }
}