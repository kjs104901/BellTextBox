using Bell.Data;

namespace Bell.Languages;

public partial class Language
{
    public static Language Proto()
    {
        Language language = new()
        {
            DefaultStyle = new ColorStyle(0.4f, 0.8f, 0.2f),
            CommentStyle = new ColorStyle(0.3f, 0.5f, 0.2f),
            StringStyle = new ColorStyle(0.2f, 0.3f, 0.3f)
        };

        language.AddFolding("{", "}");
        
        language.AddString("\"");
        
        // Message Name
        language.AddPattern(@"\b(message|enum)\s+(?<range>\w+?)\b",
            new ColorStyle(0.2f, 0.3f, 0.4f));
        
        // Keyword
        language.AddPattern(@"\b(syntax|import|package|option|message|enum|service|rpc|returns|stream|map|oneof|reserved|extend|extensions|to|max|public|weak|proto3)\b",
            new ColorStyle(0.5f, 0.4f, 0.8f));
        
        // Keyword
        language.AddPattern(@"\b(double|float|int32|int64|uint32|uint64|sint32|sint64|fixed32|fixed64|sfixed32|sfixed64|bool|string|bytes)\b",
            new ColorStyle(0.8f, 0.4f, 0.2f));
        
        // Custom Keyword
        language.AddPattern(@"\b(Double|Single|Int32|Int64|UInt32|UInt64|Boolean|String|Byte|Dictionary|List)\b",
            new ColorStyle(0.3f, 0.3f, 0.1f));
        
        // Modifiers
        language.AddPattern(@"\b(repeated|optional|required)\b",
            new ColorStyle(0.4f, 0.8f, 0.8f));
        
        // Value
        language.AddPattern(@"\b(true|false)\b",
            new ColorStyle(0.8f, 0.8f, 0.2f));
        
        // Number
        language.AddPattern(@"\b\d+[\.]?\d*\b",
            new ColorStyle(0.1f, 0.4f, 0.2f));
        
        

        return language;
    }
}