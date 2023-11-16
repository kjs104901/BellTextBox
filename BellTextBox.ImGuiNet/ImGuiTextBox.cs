﻿using System.Numerics;
using System.Runtime.InteropServices;
using Bell.Utils;
using ImGuiNET;

namespace Bell.ImGuiNet;

public class ImGuiTextBox
{
    private readonly TextBox _textBox = new(new ImGuiBackend());

    private ImFontPtr _fontPtr;
    private readonly Action _onFontLoaded;
    
    public string Text
    {
        get => _textBox.GetText();
        set => _textBox.SetText(value);
    }
    
    public ImGuiTextBox(Action onFontLoaded)
    {
        _onFontLoaded = onFontLoaded;
        SetDefaultFont();
    }

    public void SetDefaultFont()
    {
        _fontPtr = ImGui.GetIO().Fonts.AddFontDefault(null);
        LoadFontAwesome(13.0f);
        ImGui.GetIO().Fonts.Build();
        _onFontLoaded();
    }
    
    public void SetFont(string fontFile, float fontSize, IntPtr glyphRanges)
    {
        _fontPtr = ImGui.GetIO().Fonts.AddFontFromFileTTF(fontFile, fontSize, null, glyphRanges);
        LoadFontAwesome(fontSize);
        ImGui.GetIO().Fonts.Build();
        _onFontLoaded();
    }

    public void SetFont(byte[] fontBytes, float fontSize, IntPtr glyphRanges)
    {
        GCHandle fontHandle = GCHandle.Alloc(fontBytes, GCHandleType.Pinned);
        try
        {
            IntPtr fontPtr = fontHandle.AddrOfPinnedObject();
            _fontPtr = ImGui.GetIO().Fonts.AddFontFromMemoryTTF(fontPtr, fontBytes.Length, fontSize, null, glyphRanges);
            LoadFontAwesome(fontSize);
            ImGui.GetIO().Fonts.Build();
        }
        finally
        {
            fontHandle.Free();
        }
        _onFontLoaded();
    }

    private void LoadFontAwesome(float fontSize)
    {
        GCHandle glyphHandle = GCHandle.Alloc(new ushort[] { 0xe005, 0xf8ff, 0x0000 }, GCHandleType.Pinned);
        GCHandle fontHandle = GCHandle.Alloc(Fonts.fa_solid_900, GCHandleType.Pinned);
        try
        {
            IntPtr glyphPtr = glyphHandle.AddrOfPinnedObject();
            IntPtr fontPtr = fontHandle.AddrOfPinnedObject();
            unsafe
            {
                ImFontConfigPtr nativeConfig = ImGuiNative.ImFontConfig_ImFontConfig();
                nativeConfig.MergeMode = true;

                ImGui.GetIO().Fonts.AddFontFromMemoryTTF( fontPtr, Fonts.fa_solid_900.Length,
                    fontSize / 2.0f, nativeConfig, glyphPtr );
            }
        }
        finally
        {
            glyphHandle.Free();
            fontHandle.Free();
        }
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
        ImGui.PushFont(_fontPtr);
        
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, new Vector2(0, 0));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(0, 0));
        
        ImGui.PushStyleColor(ImGuiCol.ChildBg, _textBox.Theme.Background.ToVector());
        ImGui.BeginChild("##TextBox", size, true, ImGuiWindowFlags.HorizontalScrollbar);
        Vector2 contentSize = ImGui.GetWindowContentRegionMax();

        ImGui.SetNextWindowSize(new Vector2(contentSize.X, contentSize.Y));
        ImGui.Begin("##TextBoxWindow",
            ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.HorizontalScrollbar | ImGuiWindowFlags.NoNavFocus | ImGuiWindowFlags.NoNavInputs |
            ImGuiWindowFlags.ChildWindow);

        Vector2 viewPos = new Vector2(ImGui.GetScrollX(), ImGui.GetScrollY());
        Vector2 viewSize = new Vector2(contentSize.X - ImGui.GetStyle().ScrollbarSize, contentSize.Y);

        _textBox.Render(viewPos, viewSize);

        ImGui.End();
        ImGui.EndChild();
        ImGui.PopStyleColor();
        ImGui.PopStyleVar(5);
            
        ImGui.PopFont();
    }
}