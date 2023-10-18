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
        
        (ImGuiKey.F3, HotKeys.F3),
        (ImGuiKey.Escape, HotKeys.Escape),
    };

    public void Update(Vector2 size)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(0, 0));

        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.1f, 0.1f, 0.2f, 1.0f));
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
        ImGui.BeginChild("##Page", PageSize, false,
            ImGuiWindowFlags.NoScrollbar);

        _drawList = ImGui.GetWindowDrawList();
        _drawPos = ImGui.GetCursorScreenPos();

        _drawList.AddText(_drawPos, ImGui.ColorConvertFloat4ToU32(new Vector4(0.2f, 0.1f, 0.1f, 1.0f)),
            ImGui.IsWindowFocused().ToString());
        
        ImGui.Text(ImGui.IsWindowFocused().ToString());
        if (ImGui.IsWindowFocused() ||
            (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(0)))
        {
            ClearKeyboardInput();
            ClearMouseInput();
            
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

            if (io.InputQueueCharacters.Size > 0)
            {
                KeyboardInput.Chars = new List<char>();
                for (int i = 0; i < io.InputQueueCharacters.Size; i++)
                {
                    KeyboardInput.Chars.Add((char)io.InputQueueCharacters[i]);
                }
            }
            KeyboardInput.ImeComposition = ImeHandler.GetCompositionString();
            
            ProcessKeyboardInput();
            
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                MouseInput.LeftAction = MouseAction.Click;
            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                MouseInput.LeftAction = MouseAction.DoubleClick;
            if (ImGui.IsMouseDragging(ImGuiMouseButton.Left) && ImGui.IsMouseDown(ImGuiMouseButton.Left))
                MouseInput.LeftAction = MouseAction.Dragging;

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Middle))
                MouseInput.MiddleAction = MouseAction.Click;
            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Middle))
                MouseInput.MiddleAction = MouseAction.DoubleClick;
            if (ImGui.IsMouseDragging(ImGuiMouseButton.Middle) && ImGui.IsMouseDown(ImGuiMouseButton.Middle))
                MouseInput.MiddleAction = MouseAction.Dragging;
        }

        var mouse = ImGui.GetMousePos();
        MouseInput.Position.X = mouse.X - _drawPos.X;
        MouseInput.Position.Y = mouse.Y - _drawPos.Y;
        ProcessMouseInput();

        ClearViewInput();
        ViewInput.X = scroll.X;
        ViewInput.Y = scroll.Y;
        ViewInput.W = contentSize.X - ImGui.GetStyle().ScrollbarSize;;
        ViewInput.H = contentSize.Y;
        ProcessViewInput();

        Render();

        ImGui.EndChild();
        ImGui.PopStyleColor();

        ImGui.End();
        ImGui.EndChild();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar(5);
    }

    public override float GetCharWidth(char c)
    {
        return ImGui.GetFont().GetCharAdvance(c);
    }

    public override float GetFontSize()
    {
        return ImGui.GetFont().FontSize;
    }

    protected override void RenderText(Vector2 pos, string text, Vector4 color)
    {
        _drawList.AddText(new Vector2(_drawPos.X + pos.X, _drawPos.Y + pos.Y),
            ImGui.ColorConvertFloat4ToU32(color), text);
    }

    protected override void RenderLine(Vector2 start, Vector2 end, Vector4 color, float thickness)
    {
        var startPos = new Vector2(_drawPos.X + start.X, _drawPos.Y + start.Y);
        var endPos = new Vector2(_drawPos.X + end.X, _drawPos.Y + end.Y);

        _drawList.AddLine(startPos, endPos, ImGui.ColorConvertFloat4ToU32(color), thickness);
    }

    protected override void RenderRectangle(Vector2 start, Vector2 end, Vector4 color)
    {
        var startPos = new Vector2(_drawPos.X + start.X, _drawPos.Y + start.Y);
        var endPos = new Vector2(_drawPos.X + end.X, _drawPos.Y + end.Y);
        
        _drawList.AddRectFilled(startPos, endPos, ImGui.ColorConvertFloat4ToU32(color));
    }

    public override void SetClipboard(string text)
    {
        throw new NotImplementedException();
    }

    public override string GetClipboard()
    {
        throw new NotImplementedException();
    }

    protected override void SetMouseCursor(MouseCursor mouseCursor)
    {
        switch (mouseCursor)
        {
            case MouseCursor.Arrow:
                ImGui.SetMouseCursor(ImGuiMouseCursor.Arrow);
                CursorHandler.SetCursor(CursorHandler.ArrowCursor);
                break;
            case MouseCursor.Beam:
                ImGui.SetMouseCursor(ImGuiMouseCursor.TextInput);
                CursorHandler.SetCursor(CursorHandler.BeamCursor);
                break;
            case MouseCursor.Hand:
                ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                CursorHandler.SetCursor(CursorHandler.HandCursor);
                break;
        }
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

public static class CursorHandler
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetCursor(IntPtr hCursor);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

    private static int IDC_ARROW = 32512;
    private static int IDC_IBEAM = 32513;
    private static int IDC_HAND = 32649;

    public static IntPtr ArrowCursor = LoadCursor(IntPtr.Zero, IDC_ARROW);
    public static IntPtr BeamCursor = LoadCursor(IntPtr.Zero, IDC_IBEAM);
    public static IntPtr HandCursor = LoadCursor(IntPtr.Zero, IDC_HAND);
}