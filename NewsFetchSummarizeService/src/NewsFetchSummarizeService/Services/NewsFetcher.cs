namespace NewsFetchSummarizeService.Services
{
    public class NewsFetcher
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly string _newsApiKey;
        private readonly string _huggingFaceApiKey;

        public NewsFetcher(IConfiguration configuration)
        {
            _newsApiKey = configuration["NewsApiKey"];
            _huggingFaceApiKey = configuration["HuggingFaceApiKey"];
        }
    public async Task<string> FetchAndStripHtmlAsync(string url)
    {
                        Console.WriteLine("html Parsing");

        using (HttpClient client = new HttpClient())
        {
            try
            {
                Console.WriteLine(url);
                // Fetch the content from the URL
                var transcoder = new NReadabilityWebTranscoder();
                bool success;

                string transcodedContent = transcoder.Transcode(url, out success);

                if (!success)
                {
                    Console.WriteLine("Transcoding failed.");
                    return null;
                }

                // Load the HTML into HtmlAgilityPack
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(transcodedContent);

                var plainTextNode = document.DocumentNode.SelectSingleNode("//div[@id='readInner']");
                
                if (plainTextNode == null)
                {
                    Console.WriteLine("Unable to find the node with id 'readInner'.");
                    return null;
                }

                string plainText = plainTextNode.InnerText;

                return plainText;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request error: " + e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("General error: " + e.Message);
                return null;
            }
        }
    }
        public async Task<dynamic> GetNewsArticlesAsync(User user)
        {
            try
            {
                Console.WriteLine("Fetching News");
                string url = $"https://newsdata.io/api/1/news?apikey={_newsApiKey}&category={user.Preferences}&language=en";
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                dynamic newsResponse = JsonConvert.DeserializeObject(responseBody);
                return newsResponse;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }
        }
        public async Task<string> SummarizeContentAsync(string content)
        {
            try
            {
                string apiUrl = "https://api-inference.huggingface.co/models/facebook/bart-large-cnn";
                string contentToSummarize = content;

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(apiUrl),
                    Content = new StringContent(JsonConvert.SerializeObject(new { inputs = contentToSummarize }))
                };

                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _huggingFaceApiKey);

                HttpResponseMessage response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                dynamic responseJson = JsonConvert.DeserializeObject(responseBody);
                string summary = responseJson[0].summary_text;

                return summary;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Request error: " + e.Message);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("General error: " + e.Message);
                return null;
            }
        }
    }
}

