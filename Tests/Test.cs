using System.IO;

namespace UlyssesAPI;

class Test
{
    public static void Main()
    {
        var client = new UlyssesClient("meandmylit2021");
        client.GetCleanPage((async delegate(string s)
        {
            await File.WriteAllTextAsync(@"C:\Users\Дмитрий\RiderProjects\Ulysses-API\Tests\Test.html", s);
        }));
        Console.ReadKey();
    }
}