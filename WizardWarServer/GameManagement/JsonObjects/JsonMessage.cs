public class JsonMessage
{
    public JsonMessage()
    {
    }

    public JsonMessage(string type, object content)
    {
        Type = type;
        Content = content;
    }

    public string Type { get; set; } = string.Empty;
    public object? Content { get; set; } = null;

    
}