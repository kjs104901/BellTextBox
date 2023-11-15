using System.Text.RegularExpressions;
using Bell.Data;

namespace Bell.Languages;

public partial class Language
{
    public static Language Sql()
    {
        Language language = new()
        {
            DefaultStyle = new ColorStyle(0.4f, 0.8f, 0.2f),
            CommentStyle = new ColorStyle(0.3f, 0.5f, 0.2f),
            StringStyle = new ColorStyle(0.2f, 0.3f, 0.3f)
        };

        language.AddLineComment("--");
        language.AddLineComment("#");
        
        language.AddBlockComment("/*", "*/");
        
        language.AddString("\"");
        language.AddString("'");
        
        // Keyword
        language.AddPattern(@"\b(SELECT|CREATE|DATABASE|TABLE|ALTER|INDEX|DROP|VIEW|INSERT|UPDATE|DELETE|MERGE|GRANT|REVOKE|BEGIN|COMMIT|ROLLBACK|SAVEPOINT|TRANSACTION|USE|TRUNCATE|EXPLAIN|DESCRIBE|EXEC|EXECUTE|SET|SHOW|DECLARE|IF|ELSE|WHILE|CASE|ADD|ALL|AND|ANY|AS|ASC|BACKUP|BETWEEN|CHECK|COLUMN|CONSTRAINT|DEFAULT|DESC|DISTINCT|END|EXISTS|FOREIGN|FROM|FULL|GROUP|HAVING|IN|INNER|INTO|IS|JOIN|KEY|LEFT|LIKE|LIMIT|NOT|NULL|OR|ORDER|OUTER|PRIMARY|PROCEDURE|RIGHT|ROWNUM|TOP|UNION|UNIQUE|VALUES|WHERE)\b",
            new ColorStyle(0.5f, 0.4f, 0.8f), RegexOptions.IgnoreCase);
        
        // Function
        language.AddPattern(@"\b(CONCAT|LENGTH|LOWER|UPPER|SUBSTRING|TRIM|REPLACE|ABS|CEIL|FLOOR|ROUND|RAND|CURDATE|CURTIME|NOW|DATE_ADD|DATE_SUB|DATEDIFF|DAY|MONTH|YEAR|AVG|COUNT|MAX|MIN|SUM|USER|VERSION|LEN|LTRIM|RTRIM|CEILING|GETDATE|GETUTCDATE|DATEADD)\b",
            new ColorStyle(0.5f, 0.4f, 0.8f), RegexOptions.IgnoreCase);
        
        // Type
        language.AddPattern(@"\b(TINYINT|SMALLINT|MEDIUMINT|INT|INTEGER|BIGINT|DECIMAL|FLOAT|DOUBLE|BIT|DATE|DATETIME|TIMESTAMP|TIME|YEAR|CHAR|VARCHAR|BINARY|VARBINARY|TINYBLOB|BLOB|MEDIUMBLOB|LONGBLOB|TINYTEXT|TEXT|MEDIUMTEXT|LONGTEXT|ENUM|DATETIME2|SMALLDATETIME|DATETIMEOFFSET|NCHAR|NVARCHAR|NTEXT|IMAGE|SQL_VARIANT)\b",
            new ColorStyle(0.5f, 0.4f, 0.8f), RegexOptions.IgnoreCase);
        
        return language;
    }
}
    