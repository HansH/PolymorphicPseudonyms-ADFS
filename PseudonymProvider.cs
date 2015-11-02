using System;
using nl.surfnet.PolymorphicPseudonyms;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace nl.surfnet.PolymorphicPseudonyms.IdentityServer
{
    /**
     * Class to connect with the pseudonym provider
     */
    public class PseudonymProvider
    {
        private readonly string url;

        /**
         * Create a new pseudonym provider.
         * @param url The url for the pseudonym provider, as a format string with {0} being the polymorphic pseudonym and {1} being the SP ID.
         */
        public PseudonymProvider(string url)
        {
            this.url = url;
        }

        /**
         * Get an encrypted pseudonym asynchronously.
         * @param pp The polymorphic pseudonym to get an encrypted pseudonym for.
         * @param sp The SP ID to get an encrypted pseudonym for.
         * @return A task that will result in the Encrypted Pseudonym.
         */
        public async Task<Pseudonym> GetEPAsync(Pseudonym pp, string sp)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync(string.Format(url,
                WebUtility.UrlEncode(pp.Encode()), WebUtility.UrlEncode(sp)));
            string epString = await message.Content.ReadAsStringAsync();
            return Pseudonym.Decode(epString);
        }
    }
}