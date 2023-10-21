using System.Text;

namespace Bell.Utils;

public static class ThreadLocal
{
    private static readonly ThreadLocal<TextBox> ThreadLocalTextBox = new();

    public static TextBox TextBox
    {
        get => ThreadLocalTextBox.Value ?? throw new Exception("No TextBox set");
        set => ThreadLocalTextBox.Value = value;
    }
}