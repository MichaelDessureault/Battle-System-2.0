using System.Collections.Generic;

public static class ArrayExtensions {
    public static string[] EmptyStringArray(int length) {
        List<string> list = new List<string>();
        for (int i = 0; i < length; i++) {
            list.Add("");
        }
        return list.ToArray();
    }
}
