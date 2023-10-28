using Bell.Data;

namespace Bell.Utils;


internal static class Singleton
{
    private static readonly ThreadLocal<TextBox> ThreadLocalTextBox = new();
    internal static TextBox TextBox
    {
        get => ThreadLocalTextBox.Value ?? throw new Exception("No TextBox set");
        set => ThreadLocalTextBox.Value = value;
    }

    internal static CaretManager CaretManager => TextBox.CaretManager;
    internal static FontManager FontManager => TextBox.FontManager;
    internal static LineManager LineManager => TextBox.LineManager;
    internal static RowManager RowManager => TextBox.RowManager;
    internal static FoldingManager FoldingManager => TextBox.FoldingManager;
}