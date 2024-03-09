namespace Sources.Scripts.Utility
{
    public static class TextUtils
    {
        public static bool IsStringCompliant(string name, int maxLength) =>
            name != string.Empty && name.Length <= maxLength;
    }
}