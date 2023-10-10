using Bell.Render;
using Bell;
using ImGuiNET;
using System.Numerics;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;

namespace ImGuiBellTextBox;

public class ImGuiTextBoxBackend : ITextBoxBackend
{
    public Vector2 ImGuiSize { get; set; }

    public void SetClipboard(string text)
    {
        throw new NotImplementedException();
    }

    public string GetClipboard()
    {
        throw new NotImplementedException();
    }

    private readonly List<char> _keyboardInput = new();

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

    public void Render(Action<KeyboardInput, MouseInput, ViewInput> inputAction, PageRender pageRender,
        List<LineRender> lineRenders)
    {
        var drawList = ImGui.GetWindowDrawList();
        
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, new Vector2(0, 0));

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(0, 0));

        ImGui.BeginChild("Editor", ImGuiSize, true, ImGuiWindowFlags.HorizontalScrollbar);
        var contentRegion = ImGui.GetWindowContentRegionMax();
        {
            ImGui.SetNextWindowSize(new Vector2(contentRegion.X, contentRegion.Y));
            ImGui.Begin("TEST",
                ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.HorizontalScrollbar | ImGuiWindowFlags.NoNavFocus | ImGuiWindowFlags.NoNavInputs |
                ImGuiWindowFlags.ChildWindow);

            {
                ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(1.0f, 0.4f, 0.0f, 1.0f));
                ImGui.BeginChild("Page", new Vector2(pageRender.Size.Width, pageRender.Size.Height + 300), false,
                    ImGuiWindowFlags.NoScrollbar);

                KeyboardInput keyboardInput = new();
                MouseInput mouseInput = new();
                ViewInput viewInput = new();

                keyboardInput.Chars = _keyboardInput;

                if (ImGui.IsWindowFocused())
                {
                    var io = ImGui.GetIO();
                    if (io.KeyShift)
                        keyboardInput.HotKeys |= HotKeys.Shift;
                    if (io.KeyCtrl)
                        keyboardInput.HotKeys |= HotKeys.Ctrl;
                    if (io.KeyAlt)
                        keyboardInput.HotKeys |= HotKeys.Alt;
                    foreach (var keyMap in _keyboardMapping)
                    {
                        if (ImGui.IsKeyPressed(ImGui.GetKeyIndex(keyMap.Item1)))
                            keyboardInput.HotKeys |= keyMap.Item2;
                    }

                    _keyboardInput.Clear();
                    for (int i = 0; i < io.InputQueueCharacters.Size; i++)
                    {
                        _keyboardInput.Add((char)io.InputQueueCharacters[i]);
                    }

                    keyboardInput.Chars = _keyboardInput;

                    var mouse = ImGui.GetMousePos();
                    var cursor = ImGui.GetCursorScreenPos();
                    mouseInput.X = mouse.X - cursor.X;
                    mouseInput.Y = mouse.Y - cursor.Y;


                    ImGui.SetMouseCursor(ImGuiMouseCursor.TextInput);

                    if (ImGui.IsMouseClicked(0))
                        mouseInput.MouseKey = MouseKey.Click;
                    if (ImGui.IsMouseDoubleClicked(0))
                        mouseInput.MouseKey = MouseKey.DoubleClick;
                    if (ImGui.IsMouseDragging(0) && ImGui.IsMouseDown(0))
                        mouseInput.MouseKey = MouseKey.Dragging;

                    viewInput.X = ImGui.GetScrollX();
                    viewInput.Y = ImGui.GetScrollY();
                    viewInput.W = contentRegion.X;
                    viewInput.H = contentRegion.Y;

                    inputAction(keyboardInput, mouseInput, viewInput);
                }

                ImGui.Text("----- textEditor start -----");
                ImGui.Text($"keyboardInput: {keyboardInput.HotKeys} {string.Join(',', keyboardInput.Chars)}");
                ImGui.Text($"mouseInput: {mouseInput.X} {mouseInput.Y} {mouseInput.MouseKey}");
                ImGui.Text($"viewInput: {viewInput.X} {viewInput.Y} {viewInput.W} {viewInput.H}");
                ImGui.Text($"IsWindowFocused: {ImGui.IsWindowFocused()}");
                foreach (LineRender lineRender in lineRenders)
                {
                    foreach (TextRender textRender in lineRender.TextRenders)
                    {
                        if (textRender.FontStyle == FontStyle.BlockCommentFontStyle)
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.3f, 0.4f, 0.8f, 1.0f));
                        }
                        else if (textRender.FontStyle == FontStyle.LineCommentFontStyle)
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.9f, 0.1f, 0.1f, 1.0f));
                        }
                        else
                        {
                            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.4f, 0.8f, 0.2f, 1.0f));
                        }

                        ImGui.Text(textRender.Text);
                        //drawList.AddText();
                        
                        ImGui.SameLine();
                        ImGui.PopStyleColor();
                    }

                    ImGui.Text(" "); //Next line
                }

                ImGui.Text("----- textEditor end -----");
                ImGui.EndChild();
                
                ImGui.PopStyleColor();
            }


            ImGui.End();
            ImGui.PopStyleVar();
            ImGui.PopStyleVar();
            ImGui.PopStyleVar();
            ImGui.PopStyleVar();
            ImGui.PopStyleVar();
        }

        ImGui.EndChild();
    }

    RectSize ITextBoxBackend.GetRenderSize(char c)
    {
        Vector2 fontSize = ImGui.CalcTextSize(c.ToString());
        return new RectSize { Width = fontSize.X, Height = fontSize.Y };
    }
}