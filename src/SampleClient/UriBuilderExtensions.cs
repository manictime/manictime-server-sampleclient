using System;

namespace Finkit.ManicTime.Server.SampleClient
{
    public static class UriBuilderExtensions
    {
        public static UriBuilder WithQueryParameter(this UriBuilder builder, string key, string value)
        {
            if (value != null)
            {
                string existingQuery = builder.Query.TrimStart('?');
                string newParameter = string.Format("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(value));
                builder.Query = existingQuery == string.Empty
                    ? newParameter
                    : string.Format("{0}&{1}", existingQuery, newParameter);
            }
            return builder;
        }
    }
}
