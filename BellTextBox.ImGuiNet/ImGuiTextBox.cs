using System.Numerics;
using Bell.Utils;
using ImGuiNET;

namespace Bell.ImGuiNet;

public class ImGuiTextBox
{
    private readonly TextBox _textBox = new(new ImGuiBackend());

    public string Text
    {
        get => _textBox.GetText();
        set => _textBox.SetText(value);
    }

    public void Render(Vector2 size)
    {
        if (DevHelper.IsDebugMode)
        {
            if (ImGui.BeginTable("##ImGuiTextBoxDebugTable", 3, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("##ImGuiTextBoxDebugTable_Column1", ImGuiTableColumnFlags.None, 100);
                ImGui.TableSetupColumn("##ImGuiTextBoxDebugTable_Column2", ImGuiTableColumnFlags.None, 100);
                ImGui.TableSetupColumn("##ImGuiTextBoxDebugTable_Column3", ImGuiTableColumnFlags.None, 200);

                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                string debugString = _textBox.GetDebugString();
                ImGui.InputTextMultiline("##Debug", ref debugString, (uint)debugString.Length, new Vector2(-1, -1));
                
                ImGui.TableNextColumn();
                string logString = string.Join("\n", _textBox.GetLogs());
                ImGui.InputTextMultiline("##Logs", ref logString, (uint)logString.Length, new Vector2(-1, -1));
                
                ImGui.TableNextColumn();
                RenderTextBox(size);
                ImGui.EndTable();
            }
        }
        else
        {
            RenderTextBox(size);
        }
    }

    private void RenderTextBox(Vector2 size)
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

        Vector2 viewPos = new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());
        Vector2 viewSize = new Vector2(contentSize.X - ImGui.GetStyle().ScrollbarSize, contentSize.Y);

        if (DevHelper.IsDebugMode)
        {
            _textBox.Render(viewPos, viewSize);
        }
        else
        {
            try
            {
                _textBox.Render(viewPos, viewSize);
            }
            catch (Exception e)
            {
                // TODO color
                ImGui.TextColored(new Vector4(1.0f, 0.0f, 0.0f, 1.0f), e.ToString());
            }
        }

        ImGui.End();
        ImGui.EndChild();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar(5);
    }
}