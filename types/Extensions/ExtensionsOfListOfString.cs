public static partial class ExtensionsOfListOfString
{
    public static void ToFile(this List<string> lines, string file)
    {
        var sw = new StreamWriter(file);
        foreach (var l in lines)
            sw.WriteLine(l);
        sw.Close();
    }
}

