namespace BggDataRetriever
{
    public static class Base64
    {
        public static string Decode(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}