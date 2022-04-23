using RestSharp;
using RestSharp.Authenticators;
using HtmlAgilityPack;

namespace UlyssesAPI;
public class UlyssesClient
{
    private static string AUTH_URL;
    private static string COURSES_URL;
    private string CSRF;
    
    private readonly string Token;
    
    /// <summary>
    /// Класс-клиент, через которых происходят все операции в модуле.
    /// </summary>
    /// <param name="token">Ключевое слово, которое используется для входа в УЛИСС без логина/пароля.</param>
    public UlyssesClient(string token)
    {
        Token = token;
        AUTH_URL = $"https://in.lit.msu.ru/Ulysses/login/keyword/?next=%2FUlysses%2F{GetStudyYears()}%2F";
        COURSES_URL = $"https://in.lit.msu.ru/Ulysses/{GetStudyYears()}/";
    }
    
    private static string GetStudyYears()
    {
        var currentYear = DateTime.Now.Year;
        string studyYears;
        if (DateTime.Now.Month is > 0 and <= 8)
            studyYears = $"{currentYear - 1}-{currentYear}";
        else
            studyYears = $"{currentYear}-{currentYear + 1}";
        return studyYears;
    }
    
    private async Task GetCSRF(Func<string, Task> callback)
    {
        var client = new RestClient(UlyssesClient.AUTH_URL);
        
        var getRequest = new RestRequest
        {
            AlwaysMultipartFormData = true,
            Method = Method.Get,
            Timeout = 0,
            RequestFormat = DataFormat.Json,
        };
        var getResponse = await client.GetAsync(getRequest);
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(@getResponse.Content);
        var htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//input");
        await Task.Run(() => callback(htmlBody.Attributes[2].Value)); 
    }
    
    /// <summary>
    /// Получение HTML-страницы со списком курсов
    /// </summary>
    public async Task GetCleanPage(Func<string, Task> callback)
    {
        try
        {
            await GetCSRF(async delegate(string s)
            {
                this.CSRF = s;
            });
            var client = new RestClient(UlyssesClient.COURSES_URL);
            var request = new RestRequest
            {
                AlwaysMultipartFormData = true,
                MultipartFormQuoteParameters = false,
                Method = Method.Post,
                Timeout = 0,
                RequestFormat = DataFormat.Json,
            };
            request.AddHeader("Cookie", $"csrftoken={this.CSRF}");
            request.AddParameter("password", this.Token);
            request.AddParameter("csrfmiddlewaretoken", this.CSRF);
            var response = await client.GetAsync(request);
            await Task.Run(() => callback(response.Content));
        }
        catch (Exception e)
        {
            throw new Exception(e.ToString());
        }
    }
}