namespace Project.Services;

public class ActorLinkService
{
    private readonly string _baseUrl;

    public ActorLinkService(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public string GetLink(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return string.Empty;
        }

        string formattedName = name.Replace(".", "").Trim();
        formattedName = Uri.EscapeDataString(formattedName);

        string link = $"{_baseUrl}/actor/{formattedName}";

        return link;
    }
}