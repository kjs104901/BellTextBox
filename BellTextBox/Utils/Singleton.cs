using System.Text;
using Bell.Actions;
using Bell.Data;

namespace Bell.Utils;

public static class Singleton
{
    private static readonly ThreadLocal<TextBox> ThreadLocalTextBox = new();
    public static TextBox TextBox
    {
        get => ThreadLocalTextBox.Value ?? throw new Exception("No TextBox set");
        set => ThreadLocalTextBox.Value = value;
    }

    public static ActionManager ActionManager => TextBox.ActionManager;
    public static CaretManager CaretManager => TextBox.CaretManager;
    public static FontManager FontManager => TextBox.FontManager;
    public static LineManager LineManager => TextBox.LineManager;
    public static RowManager RowManager => TextBox.RowManager;
    public static FoldingManager FoldingManager => TextBox.FoldingManager;
    public static Logger Logger => TextBox.Logger;
}