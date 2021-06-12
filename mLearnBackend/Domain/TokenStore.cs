using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Google.Apis.Json;

namespace mLearnBackend.Domain
{
    public class TokenStore : IDataStore
    {
        public Task ClearAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync<T>(string key)
        {
            TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
            using (mLearnDBEntities dbConetx = new mLearnDBEntities())
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentException("Key MUST have a value");
                }


                var reuslt = dbConetx.StoreAPITokens.Where(x => x.tokenKey == key).FirstOrDefault();
                
                if (reuslt == null || string.IsNullOrWhiteSpace(reuslt.tokenValue.ToString()))
                {
                    completionSource.SetResult(default(T));
                }
                else
                {
                    dbConetx.StoreAPITokens.Remove(reuslt);
                    dbConetx.SaveChanges();
                    completionSource.SetResult(default(T));
                }

                return completionSource.Task;



            }
        }

        public Task<T> GetAsync<T>(string key)
        {
            TaskCompletionSource<T> completionSource = new TaskCompletionSource<T>();
            using (mLearnDBEntities dbConetx = new mLearnDBEntities())
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentException("Key MUST have a value");
                }

               
                var reuslt = dbConetx.StoreAPITokens.Where(x => x.tokenKey == key).FirstOrDefault();
                if(reuslt == null || string.IsNullOrWhiteSpace(reuslt.tokenValue.ToString()))
                {
                    completionSource.SetResult(default(T));
                }
                else
                {
                    completionSource.SetResult(NewtonsoftJsonSerializer
                                               .Instance.Deserialize<T>(reuslt.tokenValue.ToString()));
                }
               
                return completionSource.Task;



            }
        }
    

        public Task StoreAsync<T>(string key, T value)
        {
            using (mLearnDBEntities dbConetx = new mLearnDBEntities())
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentException("Key MUST have a value");
                }

                string contents = NewtonsoftJsonSerializer.Instance.Serialize((object)value);
                var reuslt = dbConetx.StoreAPITokens.Where(x => x.tokenKey == key).FirstOrDefault();
                if (reuslt != null)
                {
                    reuslt.tokenValue = contents;
                }
                else
                {
                    StoreAPIToken store = new StoreAPIToken();
                    store.tokenKey = key;
                    store.tokenValue = contents;
                    dbConetx.StoreAPITokens.Add(store);

                }
                dbConetx.SaveChanges();
                return Task.Delay(0);



            }
        }
    }
}

/* using (mLearnDBEntities dbConetx = new mLearnDBEntities())
            {
                if (string.IsNullOrEmpty(key))
                {
                    throw new ArgumentException("Key MUST have a value");
                }

                string contents = NewtonsoftJsonSerializer.Instance.Serialize((object)value);
                var reuslt = dbConetx.StoreAPITokens.Where(x => x.tokenKey == key).FirstOrDefault();
                if(reuslt != null)
                {
                    reuslt.tokenValue = contents;
                }
                else
                {
                    StoreAPIToken store = new StoreAPIToken();
                    store.tokenKey = key;
                    store.tokenValue = contents;
                    dbConetx.StoreAPITokens.Add(store);

                }
                dbConetx.SaveChanges();
               return Task.Delay(0);

               

            }*/