    public class PersonalizedNewsArticle
    {
        public string? ArticleId { get; set; }
        public string? Title { get; set; }
        public string? Link { get; set; }
        public string? VideoUrl { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public string? PubDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? SourceId { get; set; }
        public int? SourcePriority { get; set; }
        public string? SourceUrl { get; set; }
        public string? SourceIcon { get; set; }
        public string? Language { get; set; }
        public string? Category { get; set; }
        public string? Summary { get; set; }

        public override string ToString()
        {
            return $"ArticleId: {ArticleId}\n" +
                   $"Title: {Title}\n" +
                   $"Link: {Link}\n" +
                   $"VideoUrl: {VideoUrl}\n" +
                   $"Description: {Description}\n" +
                   $"Content: {Content}\n" +
                   $"PubDate: {PubDate}\n" +
                   $"ImageUrl: {ImageUrl}\n" +
                   $"SourceId: {SourceId}\n" +
                   $"SourcePriority: {SourcePriority}\n" +
                   $"SourceUrl: {SourceUrl}\n" +
                   $"SourceIcon: {SourceIcon}\n" +
                   $"Language: {Language}\n" +
                   $"Category: {Category}\n" +
                   $"Summary: {Summary}";
        }
    }