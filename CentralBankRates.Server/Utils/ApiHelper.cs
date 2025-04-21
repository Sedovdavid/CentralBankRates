using System.Text;
using System.Xml.Serialization;

namespace CentralBankRates.Server.Utils;

public static class ApiHelper
{
    public static async Task<T> GetApi<T>(string route)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var client = new HttpClient();
        var response = await client.GetAsync(route);
        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync();

        var encoding = Encoding.GetEncoding("windows-1251");

        var xml = encoding.GetString(bytes);

        var serializer = new XmlSerializer(typeof(T));
        var reader = new StringReader(xml);
        var data = (T)serializer.Deserialize(reader)!;

        return data;
    }
}