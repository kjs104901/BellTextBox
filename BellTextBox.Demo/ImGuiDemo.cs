using System.Diagnostics;
using System.Numerics;
using Bell;
using ImGuiNET;
using Veldrid;
using Veldrid.StartupUtilities;

namespace BellTextBox.Demo;

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

hello world
헬로우 월드
abc 123s
윷?? 

hello world
헬로우 월드
abc 123s
윷?? 

hello world
헬로우 월드
abc 123s
윷?? 

hello world
헬로우 월드
abc 123s
윷?? 

hello world
헬로우 월드
abc 123s
윷?? 

hello world
헬로우 월드
abc 123s
윷?? 

hello world
헬로우 월드
abc 123s
윷?? 

hello world
헬로우 월드
abc 123s
윷?? 

hello world
헬로우 월드
abc 123s
윷??

--- END ---";
    
        VeldridStartup.CreateWindowAndGraphicsDevice(
            new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "ImGui.NET Sample Program"),
            new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true),
            out var sdl2Window,
            out var graphicsDevice);
        
        var commandList = graphicsDevice.ResourceFactory.CreateCommandList();
        var imGuiRenderer = new ImGuiRenderer(graphicsDevice, graphicsDevice.MainSwapchain.Framebuffer.OutputDescription, sdl2Window.Width,
            sdl2Window.Height);
        
        sdl2Window.Resized += () =>
        {
            graphicsDevice.MainSwapchain.Resize((uint)sdl2Window.Width, (uint)sdl2Window.Height);
            imGuiRenderer.WindowResized(sdl2Window.Width, sdl2Window.Height);
        };

        var stopwatch = Stopwatch.StartNew();

        var imFontPtr = ImGui.GetIO().Fonts
            .AddFontFromFileTTF(@"gulim.ttc", 13.0f, null, ImGui.GetIO().Fonts.GetGlyphRangesKorean());
        imGuiRenderer.RecreateFontDeviceTexture(graphicsDevice);

        var imGuiBellTextBox = new ImGuiBellTextBox(new Vector2(-1, -1));
        
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
            ImGui.Begin("Demo");
            ImGui.PushFont(imFontPtr);

            if (ImGui.Button("Reset"))
            {
            }
            
            ImGui.InputTextMultiline("Test", ref textInput, 1024, new Vector2(-1, 300));

            imGuiBellTextBox.SetText(textInput);
            imGuiBellTextBox.Update();

            //ImGui.SameLine();
            ImGui.InputText("Test2", ref textInput, 1024);

            ImGui.PopFont();
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