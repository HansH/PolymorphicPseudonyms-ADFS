using nl.surfnet.PolymorphicPseudonyms;
using System;
using System.Threading.Tasks;
using System.Text;

namespace nl.surfnet.PolymorphicPseudonyms.IdentityServer
{
    /**
     * Class that does the actual work for the {@link PolyPseudStore}
     */
    internal class PolyPseudWorker
    {
        const int INDEX_UID = 0;
        const int INDEX_SP = 1;
        private string[] parameters;
        private string y_k;
        PseudonymDB db;
        PseudonymProvider pseudonymProvider;

        /**
         * Create a new PolyPseudWorker
         * @param parameters The parameters for the query.
         * @param y_k The public key used in the polymorphic pseudonym system.
         * @param connectionString The connection string for the database to store polymorphic and encrypted pseudonyms in.
         * @param pseudonymProviderUrl The url for the pseudonym provider, as a format string with {0} being the polymorphic pseudonym and {1} being the SP ID.
         */
        public PolyPseudWorker(string[] parameters, string y_k, string connectionString, string pseudonymProviderUrl)
        {
            this.parameters = parameters;
            this.y_k = y_k;
            db = new PseudonymDB(connectionString);
            pseudonymProvider = new PseudonymProvider(pseudonymProviderUrl);
        }

        /**
         * Execute the <code>randomize</code> query
         * @return 2D array <code>result</code> with <code>result[0][0]</code> being the encoded randomized pseudonym.
         */
        public string[][] Randomize()
        {
            
            string[] oneResult = new string[1];

            Pseudonym pseudonym = Pseudonym.Decode(parameters[0]);
            pseudonym = pseudonym.Randomize();
            oneResult[0] = pseudonym.Encode();

            return new string[][] { oneResult };
        }

        /**
         * Execute the <code>getEP</code> query
         * @return 2D array <code>result</code> with <code>result[0][0]</code> being the encoded encrypted pseudonym.
         */
        public string[][] GetEP()
        {
            Pseudonym ep = db.GetEncrypted(parameters[INDEX_UID], parameters[INDEX_SP]);
            if(ep == null)
            {
                Pseudonym pp = GetPolymorphicPseudonym();
                ep = pseudonymProvider.GetEPAsync(pp, parameters[INDEX_SP]).Result;
                db.AddEncrypted(parameters[INDEX_UID], parameters[INDEX_SP], ep);
            }
            return new string[][] { new string[] { ep.Encode() } };
        }

        /**
         * Execute the <code>getPP</code> query
         * @return 2D array <code>result</code> with <code>result[0][0]</code> being the encoded polymorphic pseudonym.
         */
        public string[][] GetPP()
        {
            return new string[][] { new string[] { GetPolymorphicPseudonym().Encode() } };
        }

        /**
         * Get a polymorphic pseudonym from the database, or create one if it does not yet exist.
         * @param return The polymorphic pseudonym for the User ID in parameters[0]
         */
        private Pseudonym GetPolymorphicPseudonym()
        {
            Pseudonym pp = db.GetPolymorphic(parameters[INDEX_UID]);
            if (pp == null)
            {
                IdP idp = new IdP(SystemParams.Curve.DecodePoint(Convert.FromBase64String(y_k)));
                pp = idp.GeneratePolymorphicPseudonym(parameters[INDEX_UID]);
                db.AddPolymorphic(parameters[INDEX_UID], pp);
            }

            return pp;
        }
    }
}