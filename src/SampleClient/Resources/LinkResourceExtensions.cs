using System.Linq;

namespace Finkit.ManicTime.Server.SampleClient.Resources
{
    public static class LinkResourceExtensions
    {
        public static string Url(this LinkResource[] links, string rel)
        {
            return links?.SingleOrDefault(l => l.Rel == rel)?.Href;
        }
    }
}
