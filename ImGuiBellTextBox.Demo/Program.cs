using Bell;
using ImGuiNET;
using System.Numerics;
using Veldrid.StartupUtilities;
using Veldrid;

namespace ImGuiBellTextBox.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            unsafe
            {
                var windowInfo = new WindowCreateInfo
                {
                    X = 100,
                    Y = 100,
                    WindowWidth = 480,
                    WindowHeight = 640,
                    WindowInitialState = WindowState.Normal,
                    WindowTitle = "TextEdit.Test"
                };

                var gdOptions = new GraphicsDeviceOptions(
                    true,
                    PixelFormat.D24_UNorm_S8_UInt,
                    true,
                    ResourceBindingModel.Improved,
                    true,
                    true,
                    false);

                var window = VeldridStartup.CreateWindow(ref windowInfo);
                var gd = VeldridStartup.CreateGraphicsDevice(window, gdOptions, GraphicsBackend.Direct3D11);

                var imguiRenderer = new ImGuiRenderer(
                    gd,
                    gd.MainSwapchain.Framebuffer.OutputDescription,
                    (int)gd.MainSwapchain.Framebuffer.Width,
                    (int)gd.MainSwapchain.Framebuffer.Height);

                var cl = gd.ResourceFactory.CreateCommandList();
                window.Resized += () =>
                {
                    gd.ResizeMainWindow((uint)window.Width, (uint)window.Height);
                    imguiRenderer.WindowResized(window.Width, window.Height);
                };

                var editor = new TextBox(new ImGuiTextBoxBackend());
                editor.Page.Text.Set("Hello World\nNext Line\n한글");

                var config = ImGuiNative.ImFontConfig_ImFontConfig();
                var imFontPtr = ImGui.GetIO().Fonts.AddFontFromFileTTF(@"gulim.ttc", 13.0f, config, ImGui.GetIO().Fonts.GetGlyphRangesKorean());
                ImGuiNative.ImFontConfig_destroy(config);

                imguiRenderer.RecreateFontDeviceTexture(gd);

                DateTime lastFrame = DateTime.Now;
                while (window.Exists)
                {
                    var input = window.PumpEvents();
                    if (!window.Exists)
                        break;

                    var thisFrame = DateTime.Now;
                    imguiRenderer.Update((float)(thisFrame - lastFrame).TotalSeconds, input);
                    lastFrame = thisFrame;

                    ImGui.SetNextWindowPos(new Vector2(0, 0));
                    ImGui.SetNextWindowSize(new Vector2(window.Width, window.Height));
                    ImGui.Begin("Demo");
                    ImGui.PushFont(imFontPtr);

                    if (ImGui.Button("Reset"))
                    {
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("err line"))
                    {

                    }

                    ImGui.Text($"Test2");

                    editor.Render();

                    ImGui.PopFont();
                    ImGui.End();

                    cl.Begin();
                    cl.SetFramebuffer(gd.MainSwapchain.Framebuffer);
                    cl.ClearColorTarget(0, RgbaFloat.Black);
                    imguiRenderer.Render(gd, cl);
                    cl.End();
                    gd.SubmitCommands(cl);
                    gd.SwapBuffers(gd.MainSwapchain);
                }
            }
        }
    }
}