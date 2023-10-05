using System.Diagnostics;
using System.Numerics;
using Bell;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ImGuiBellTextBox.Demo;

internal class Program
{
    private static Sdl2Window? _window;
    private static GraphicsDevice? _gd;
    private static CommandList? _cl;
    private static ImGuiRenderer? _guiRenderer;

    private static readonly Vector3 ClearColor = new(0.45f, 0.55f, 0.6f);
    private static string _textInput = "";

    private static void Main(string[] args)
    {
        // Create window, GraphicsDevice, and all resources necessary for the demo.
        VeldridStartup.CreateWindowAndGraphicsDevice(
            new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "ImGui.NET Sample Program"),
            new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
            out _window,
            out _gd);
        
        _window.Resized += () =>
        {
            _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
            _guiRenderer.WindowResized(_window.Width, _window.Height);
        };
        
        _cl = _gd.ResourceFactory.CreateCommandList();
        _guiRenderer = new ImGuiRenderer(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width,
            _window.Height);

        var stopwatch = Stopwatch.StartNew();

        var imFontPtr = ImGui.GetIO().Fonts
            .AddFontFromFileTTF(@"gulim.ttc", 13.0f, null, ImGui.GetIO().Fonts.GetGlyphRangesKorean());
        _guiRenderer.RecreateFontDeviceTexture(_gd);
        
        var editor = new TextBox(new ImGuiTextBoxBackend());
        
        while (_window.Exists)
        {
            var deltaTime = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
            stopwatch.Restart();
            var snapshot = _window.PumpEvents();
            if (!_window.Exists)
                break;
            _guiRenderer.Update(deltaTime,
                snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(_window.Width, _window.Height));
            ImGui.Begin("Demo");
            ImGui.PushFont(imFontPtr);

            if (ImGui.Button("Reset"))
            {
            }
            
            ImGui.InputTextMultiline("Test", ref _textInput, 1024, new Vector2(-1, 300));

            editor.Page.Text.Set(_textInput);
            editor.Render();

            ImGui.PopFont();
            ImGui.End();

            _cl.Begin();
            _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
            _cl.ClearColorTarget(0, new RgbaFloat(ClearColor.X, ClearColor.Y, ClearColor.Z, 1f));
            _guiRenderer.Render(_gd, _cl);
            _cl.End();
            _gd.SubmitCommands(_cl);
            _gd.SwapBuffers(_gd.MainSwapchain);
        }

        // Clean up Veldrid resources
        _gd.WaitForIdle();
        _guiRenderer.Dispose();
        _cl.Dispose();
        _gd.Dispose();
    }
}