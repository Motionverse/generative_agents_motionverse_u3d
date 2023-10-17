using System.Text;

public class QueryBuilder
{
    private const string STARTING_CHARACTER = "?";
    private const string PARAMETER_SEPARATOR = "&";
    private readonly StringBuilder query = new StringBuilder();
    public string Query => query.ToString();

    public QueryBuilder()
    {
        query.Append(STARTING_CHARACTER);
    }

    public QueryBuilder(string keyName, string value)
    {
        query.Append(STARTING_CHARACTER);
        AddKeyValue(keyName, value);
    }

    public void AddKeyValue(string keyName, string value)
    {
        AddKey(keyName);
        AddValue(value);
    }

    private void AddKey(string keyName)
    {
        //if first key does not add parameter separator
        var separator = Query.Length > 1 ? PARAMETER_SEPARATOR : "";
        AppendQuery($"{separator}{keyName}=");
    }

    private void AddValue(string value)
    {
        AppendQuery(value);
    }

    private void AppendQuery(string text)
    {
        query.Append(text);
    }
}
