using Bell.Render;
using Bell;
using ImGuiNET;
using System.Numerics;
using Bell.Data;

namespace ImGuiBellTextBox;

public class ImGuiTextBoxBackend : ITextBoxBackend
{
    public void SetClipboard(string text)
    {
        throw new NotImplementedException();
    }

    public string GetClipboard()
    {
        throw new NotImplementedException();
    }

    public void Render(TextBox textBox, List<LineRender> lineRenders)
    {
        ImGui.BeginChild("Editor", new Vector2(0, 0), true, ImGuiWindowFlags.HorizontalScrollbar);
        ImGui.BeginChild("Page", new Vector2(500, 1000), false, ImGuiWindowFlags.NoScrollbar);

        ImGui.Text("----- textEditor start -----");
        foreach (LineRender lineRender in lineRenders)
        {
            ImGui.Text(lineRender.Text);
        }
        ImGui.Text("----- textEditor end -----");

        ImGui.EndChild();
        ImGui.EndChild();
    }

    RectSize ITextBoxBackend.GetRenderSize(char c)
    {
        Vector2 fontSize = ImGui.CalcTextSize(c.ToString());
        return new RectSize { Width = fontSize.X, Height = fontSize.Y };
    }
}