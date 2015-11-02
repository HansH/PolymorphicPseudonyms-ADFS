using nl.surfnet.PolymorphicPseudonyms;
using System.Linq;

namespace nl.surfnet.PolymorphicPseudonyms.IdentityServer
{
    /**
     * Database class to get and store polymorphic and encrypted pseudonyms in a database.
     */
    class PseudonymDB
    {
        private PolyPseudDataContext db;

        /**
         * Create a PseudonymDB
         * @param connectionString The connection string to connect to the database.
         */
        public PseudonymDB(string connectionString)
        {
            
            db = new PolyPseudDataContext(connectionString);
        }

        /**
         * Get a polymorphic pseudonym for a user from the database.
         * @param user the User ID to get a polymorphic pseudonym for.
         * @return The polymorphic pseudonym for the specified user, or null if no polymorphic pseudonym was found in the database.
         */
        public Pseudonym GetPolymorphic(string user)
        {
            var pseudonyms = from p in db.PolymorphicPseudonyms
                             where p.user == user
                             select Pseudonym.Decode(p.pseudonym);

            return pseudonyms.FirstOrDefault();
        }

        /**
         * Store a polymorphic pseudonym in the database.
         * @param user The User ID to store the polymorphic pseudonym for.
         * @param pp The polymorphic pseudonym to store.
         */
        public void AddPolymorphic(string user, Pseudonym pp)
        {
            PolymorphicPseudonym pseudonym = new PolymorphicPseudonym();
            pseudonym.user = user;
            pseudonym.pseudonym = pp.Encode();
            pseudonym.id = System.Guid.NewGuid();
            db.PolymorphicPseudonyms.InsertOnSubmit(pseudonym);
            db.SubmitChanges();
        }

        /**
         * Get an encrypted pseudonym for a user from the database.
         * @param user the User ID to get a encrypted pseudonym for.
         * @param sp the SP ID to get a encrypted pseudonym for.
         * @return The encrypted pseudonym for the specified user and SP, or null if no encrypted pseudonym was found in the database.
         */
        public Pseudonym GetEncrypted(string user, string sp)
        {
            var pseudonyms = from ep in db.EncryptedPseudonyms
                             where ep.user == user && ep.sp == sp
                             select Pseudonym.Decode(ep.pseudonym);

            return pseudonyms.FirstOrDefault();
        }

        /**
         * Store a encrypted pseudonym in the database.
         * @param user The User ID to store the encrypted pseudonym for.
         * @param sp the SP ID to store the encrypted pseudonym for.
         * @param ep The encrypted pseudonym to store.
         */
        public void AddEncrypted(string user, string sp, Pseudonym ep)
        {
            EncryptedPseudonym pseudonym = new EncryptedPseudonym();
            pseudonym.user = user;
            pseudonym.sp = sp;
            pseudonym.pseudonym = ep.Encode();
            pseudonym.id = System.Guid.NewGuid();
            db.EncryptedPseudonyms.InsertOnSubmit(pseudonym);
            db.SubmitChanges();
        }
    }
}
