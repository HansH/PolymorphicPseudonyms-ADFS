using Microsoft.IdentityServer.ClaimsPolicy.Engine.AttributeStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nl.surfnet.PolymorphicPseudonyms.IdentityServer
{
    /** 
     * Attribute Store that can be used in AD FS to create polymorphic and encrypted pseudonyms for users.
     */
    public class PolyPseudStore : IAttributeStore
    {
        private Dictionary<string, string> config;
        private Func<string[][]> func;

        /**
         * Begin asynchronous execution of a query.
         * @param query The query to execute. Can be one of the following:
         * <ul>
         * <li><b>randomize</b> Randomizes a pseudonym. parameters[0] must be the pseudonym to randomize.
         * <li><b>getEP</b> Returns an encrypted pseudonym. parameters[0] must be the User ID, parameters[0] the SP ID.
         * <li><b>getEP</b> Returns a polymorphic pseudonym. parameters[0] must be the User ID.
         * </ul>
         * @param parameters The parameters for the query.
         * @param callback The delegate to call when execution is finished.
         * @param state Not used.
         * @return IAsyncResult for the asynchronous execution.
         */
        public IAsyncResult BeginExecuteQuery(string query, string[] parameters, AsyncCallback callback, object state)
        {
            PolyPseudWorker worker = new PolyPseudWorker(parameters, config["y_k"], config["connectionString"], config["pseudonymProviderUrl"]);

            switch(query)
            {
                case "randomize":
                    func = new Func<string[][]>(worker.Randomize);
                    return func.BeginInvoke(callback, state);
                case "getEP":
                    func = new Func<string[][]>(worker.GetEP);
                    return func.BeginInvoke(callback, state);
                case "getPP":
                    func = new Func<string[][]>(worker.GetPP);
                    return func.BeginInvoke(callback, state);
                default:
                    throw new ArgumentException(String.Format("The query '{0}' is not recognized.", query));
            }
            
        }

        /** 
         * Gets the result from an asynchronously executed query.
         * @param result The IAsyncResult from the asynchronous execution.
         * @return The result of the query.
         */
        public string[][] EndExecuteQuery(IAsyncResult result)
        {
            return func.EndInvoke(result);
        }
         
        /**
         * Initialize the store with the given configuration
         * @param config The configuration parameters for the store. Must contain the following:
         * <ul>
         * <li><b>y_k</b> The public key used in the polymorphic pseudonym system.
         * <li><b>connectionString</b> The connection string for the database to store polymorphic and encrypted pseudonyms in.
         * <li><b>pseudonymProviderUrl</b> The url for the pseudonym provider, as a format string with {0} being the polymorphic pseudonym and {1} being the SP ID.
         * </ul>
         */
        public void Initialize(Dictionary<string, string> config)
        {
            this.config = config;
        }
    }
}
