namespace TaskManager.API.Utils;

public static class EnvHelper
{
    public static string Get(string key)
    {
        return Environment.GetEnvironmentVariable(key) ?? string.Empty;
    }
}