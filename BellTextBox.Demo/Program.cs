using System.Diagnostics;
using System.Numerics;
using Bell;
using BellTextBox.Demo.Backend;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace BellTextBox.Demo;

internal class Program
{

    private static readonly Vector3 ClearColor = new(0.45f, 0.55f, 0.6f);
    private static string _textInput = "";

    private static void Main(string[] args)
    {
        Thread imGuiThread = ImGuiDemo.ThreadStart();
        imGuiThread.Join();
    }
}