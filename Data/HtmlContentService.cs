using System.Collections.Generic;

public class HtmlContentService
{
    private readonly List<string> _htmlContents;

    public HtmlContentService()
    {
        _htmlContents = new List<string>();
    }

    public void AddHtmlContent(string htmlContent)
    {
        _htmlContents.Add(htmlContent);
    }

    public List<string> GetHtmlContents()
    {
        return _htmlContents;
    }

    public void ClearHtmlContents()
    {
        _htmlContents.Clear();
    }
}
