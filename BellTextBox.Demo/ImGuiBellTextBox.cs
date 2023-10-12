using Bell.Render;
using Bell;
using ImGuiNET;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Bell.Data;
using Bell.Inputs;
using Bell.Languages;

namespace BellTextBox.Demo;


public class ImGuiBellTextBox : TextBox
{
    private ImDrawListPtr _drawList;
    private Vector2 _drawPos;
    
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
    
    public void Update(Vector2 size)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(0, 0));

        ImGui.BeginChild("##TextBox", size, true, ImGuiWindowFlags.HorizontalScrollbar);
        Vector2 contentSize = ImGui.GetWindowContentRegionMax();

        ImGui.SetNextWindowSize(new Vector2(contentSize.X, contentSize.Y));
        ImGui.Begin("##TextBoxWindow",
            ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.HorizontalScrollbar | ImGuiWindowFlags.NoNavFocus | ImGuiWindowFlags.NoNavInputs |
            ImGuiWindowFlags.ChildWindow);

        Vector2 scroll = new Vector2()
        {
            X = ImGui.GetScrollX(),
            Y = ImGui.GetScrollY(),
        };

        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.2f, 0.1f, 0.1f, 1.0f));
        ImGui.BeginChild("##Page", Page.Render.Size, false,
            ImGuiWindowFlags.NoScrollbar);

        _drawList = ImGui.GetWindowDrawList();
        _drawPos = ImGui.GetCursorScreenPos();
        
        InputStart();
        _drawList.AddText(_drawPos, ImGui.ColorConvertFloat4ToU32(new Vector4(0.2f, 0.1f, 0.1f, 1.0f)),
            ImGui.IsWindowFocused().ToString());

        ImGui.Text(ImGui.IsWindowFocused().ToString());
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
            KeyboardInput.ImeComposition = ImeHandler.GetCompositionString();

            var mouse = ImGui.GetMousePos();
            MouseInput.X = mouse.X - _drawPos.X;
            MouseInput.Y = mouse.Y - _drawPos.Y;

            ImGui.SetMouseCursor(ImGuiMouseCursor.TextInput);

            if (ImGui.IsMouseClicked(0))
                MouseInput.MouseKey = MouseKey.Click;
            if (ImGui.IsMouseDoubleClicked(0))
                MouseInput.MouseKey = MouseKey.DoubleClick;
            if (ImGui.IsMouseDragging(0) && ImGui.IsMouseDown(0))
                MouseInput.MouseKey = MouseKey.Dragging;
        }

        ViewInput.X = scroll.X;
        ViewInput.Y = scroll.Y;
        ViewInput.W = contentSize.X;
        ViewInput.H = contentSize.Y;
        InputEnd();
        
        Render();
        
        ImGui.EndChild();
        ImGui.PopStyleColor();
        
        ImGui.End();
        ImGui.EndChild();
        ImGui.PopStyleVar(5);
    }

    public override void SetClipboard(string text)
    {
        throw new NotImplementedException();
    }

    public override string GetClipboard()
    {
        throw new NotImplementedException();
    }

    public override float GetCharWidth(char c)
    {
        return ImGui.GetFont().GetCharAdvance(c);
    }

    public override float GetCharHeight(char c)
    {
        return ImGui.CalcTextSize(c.ToString()).Y;
    }

    protected override void RenderText(Vector2 pos, string text, FontStyle fontStyle)
    {
        var color = new Vector4(fontStyle.R, fontStyle.G, fontStyle.B, fontStyle.A);
        _drawList.AddText(new Vector2(_drawPos.X + pos.X, _drawPos.Y + pos.Y),
            ImGui.ColorConvertFloat4ToU32(color), text);
    }
}


public static class ImeHandler
{
    [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr ImmGetContext(IntPtr hWnd);

    [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
    private static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

    [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
    private static extern int ImmGetCompositionString(IntPtr hIMC, uint dwIndex, byte[]? lpBuf, int dwBufLen);
    
    [DllImport("user32.dll")]
    private static extern IntPtr GetFocus();

    private const uint GCS_COMPSTR = 0x0008;
    
    public static string GetCompositionString()
    {
        IntPtr hWnd = GetFocus(); // Get the handle to the active window
        IntPtr hIMC = ImmGetContext(hWnd); // Get the Input Context
        try
        {
            int strLen = ImmGetCompositionString(hIMC, GCS_COMPSTR, null, 0);
            if (strLen > 0)
            {
                byte[]? buffer = new byte[strLen];
                ImmGetCompositionString(hIMC, GCS_COMPSTR, buffer, strLen);
                return Encoding.Unicode.GetString(buffer);
            }
            return string.Empty;
        }
        finally
        {
            ImmReleaseContext(hWnd, hIMC);
        }
    }
}