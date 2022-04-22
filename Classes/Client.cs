namespace Ulysses_API;

public class Client
{
    private string Token;
    private static string AUTH_URL;
    private static string COURSES_URL;

    public Client(string token)
    {
        Token = token;
        AUTH_URL = $"https://in.lit.msu.ru/Ulysses/login/keyword/?next=%2FUlysses%2F{SetYearsForUrl()}%2F";
        COURSES_URL = $"https://in.lit.msu.ru/Ulysses/{SetYearsForUrl()}/";
    }

    private static string SetYearsForUrl()
    {
        var currentYear = DateTime.Now.Year;
        string studyYears;
        if (DateTime.Now.Month is > 0 and <= 8)
            studyYears = $"{currentYear - 1}-{currentYear}";
        else
            studyYears = $"{currentYear}-{currentYear + 1}";
        return studyYears;
    }
    
    
}