using System.Linq;

namespace Finkit.ManicTime.WebClient.Resources
{
    public static class LinkResourceExtensions
    {
        public static string Url(this LinkResource[] links, string rel)
        {
            if (links == null)
                return null;
            LinkResource link = links.SingleOrDefault(l => l.Rel == rel);
            return link == null ? null : link.Href;
        }
    }
}
