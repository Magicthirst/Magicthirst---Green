using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Web.Util
{
    public static class JsonContent
    {
        public static HttpContent Create<T>(T body) => new StringContent
        (
            content: JsonConvert.SerializeObject(body),
            encoding: Encoding.UTF8,
            mediaType: MediaTypeNames.Application.Json
        );

        public static async Task<T> ReadJsonContentBy<T>(this HttpContent content, T value)
        {
            var body = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeAnonymousType(body, value);
        }
    }
}