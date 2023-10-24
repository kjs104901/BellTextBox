using System.Diagnostics;
using System.Numerics;
using Bell.ImGuiNet;
using ImGuiNET;
using Veldrid;
using Veldrid.StartupUtilities;

namespace Bell.Demo;

class ImGuiDemo
{
    public static Thread ThreadStart()
    {
        var thread = new Thread(ThreadMain)
        {
            Name = "ImGui"
        };
        thread.Start();
        return thread;
    }

    private static void ThreadMain()
    {
        Vector3 clearColor = new(0.45f, 0.55f, 0.6f);
        string textInput = @"--- START ---
}}}}}}}}}
||||||||
aaaaaaaa

namespace Bell.Languages;

public class FontStyle : IComparable<FontStyle>
{
    private readonly string _id;
    
    // 한글 주석 테스트
" + '\t' + @"
    public float R;
    public float G;
    public float B;
    public float A;
    
    public FontStyle(string id)
    {
        _id = id;
    }
    
    // 한글 주석 테스트

    public static readonly FontStyle DefaultFontStyle = new(""Default"") { R = 0.4f, G = 0.8f, B = 0.2f, A = 1.0f };
    public static readonly FontStyle LineCommentFontStyle = new(""LineComment"") { R = 0.3f, G = 0.4f, B = 0.8f, A = 1.0f };
    public static readonly FontStyle BlockCommentFontStyle = new(""BlockComment"") { R = 0.9f, G = 0.1f, B = 0.1f, A = 1.0f };

    public int CompareTo(FontStyle? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(_id, other._id, StringComparison.Ordinal);
    }
}
--- END ---";

        VeldridStartup.CreateWindowAndGraphicsDevice(
            new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "ImGui.NET Sample Program"),
            new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
            out var sdl2Window,
            out var graphicsDevice);

        var commandList = graphicsDevice.ResourceFactory.CreateCommandList();
        var imGuiRenderer = new ImGuiRenderer(graphicsDevice,
            graphicsDevice.MainSwapchain.Framebuffer.OutputDescription, sdl2Window.Width,
            sdl2Window.Height);

        sdl2Window.Resized += () =>
        {
            graphicsDevice.MainSwapchain.Resize((uint)sdl2Window.Width, (uint)sdl2Window.Height);
            imGuiRenderer.WindowResized(sdl2Window.Width, sdl2Window.Height);
        };

        var stopwatch = Stopwatch.StartNew();

        var imFontPtr = ImGui.GetIO().Fonts
            .AddFontFromFileTTF(@"Fonts\MaruBuri.ttf", 26.0f, null, ImGui.GetIO().Fonts.GetGlyphRangesKorean());
        imGuiRenderer.RecreateFontDeviceTexture(graphicsDevice);

        var imGuiBellTextBox = new ImGuiTextBox();
        imGuiBellTextBox.Text = textInput;

        while (sdl2Window.Exists)
        {
            var deltaTime = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
            stopwatch.Restart();
            var snapshot = sdl2Window.PumpEvents();
            if (!sdl2Window.Exists)
                break;
            imGuiRenderer.Update(deltaTime,
                snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(sdl2Window.Width, sdl2Window.Height));
            ImGui.Begin("Demo", ImGuiWindowFlags.NoResize);

            if (ImGui.BeginTable("table2", 2, ImGuiTableFlags.Resizable))
            {
                ImGui.TableSetupColumn("C1", ImGuiTableColumnFlags.None, 100);
                ImGui.TableSetupColumn("C2", ImGuiTableColumnFlags.None, 200);

                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                string debugString = imGuiBellTextBox.DebugString;
                ImGui.InputTextMultiline("##Debug", ref debugString, (uint)debugString.Length, new Vector2(-1, -1));

                ImGui.TableNextColumn();
                
                ImGui.PushFont(imFontPtr);
                imGuiBellTextBox.Render(new Vector2(-1, -1));
                ImGui.PopFont();

                ImGui.EndTable();
            }
            
            ImGui.End();

            commandList.Begin();
            commandList.SetFramebuffer(graphicsDevice.MainSwapchain.Framebuffer);
            commandList.ClearColorTarget(0, new RgbaFloat(clearColor.X, clearColor.Y, clearColor.Z, 1f));
            imGuiRenderer.Render(graphicsDevice, commandList);
            commandList.End();
            graphicsDevice.SubmitCommands(commandList);
            graphicsDevice.SwapBuffers(graphicsDevice.MainSwapchain);
        }

        // Clean up Veldrid resources
        graphicsDevice.WaitForIdle();
        imGuiRenderer.Dispose();
        commandList.Dispose();
        graphicsDevice.Dispose();
    }
}