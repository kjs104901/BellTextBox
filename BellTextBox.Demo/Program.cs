namespace BellTextBox.Demo;

internal static class Program
{
    private static void Main(string[] args)
    {
        var imGuiThread = ImGuiDemo.ThreadStart();
        imGuiThread.Join();
    }
}