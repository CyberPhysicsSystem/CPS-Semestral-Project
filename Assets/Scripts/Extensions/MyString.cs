public static class MyString
{
    public static bool IsAnyOf(this string self, string other, params string[] others)
    {
        if (self.Equals(other)) return true;
        foreach (var item in others)
            if (self.Equals(item)) return true;
        return false;
    }
}