namespace API.Seguridad.Application.Core
{
    public static class ExtensionPlaceHolder
    {
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return value == null || string.IsNullOrEmpty(value.Trim()) ? true : false;
        }   
    }
}
