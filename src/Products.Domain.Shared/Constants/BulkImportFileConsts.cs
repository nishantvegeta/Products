using System;
using System.ComponentModel;

namespace Products.Constants;

public class BulkImportFileConsts
{
    public static readonly string[] ExtensionAllowed = {
        ".csv",
        ".xlsx",
        ".json"
    };

    [Description("File Size in Bits")]
    public static readonly int FileSizeLimit = 10 * 1024 * 1024;
}
