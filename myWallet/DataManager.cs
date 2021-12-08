using System;
using System.Collections.Generic;
using System.Data;

namespace myWallet
{
    class DataManager
    {
        private Database db;
        public DataManager()
        {
            db = new Database();
        }
        public void updateCurrenciesInMarketAssets()
        {
            DataTable dt = db.getMarketAssets();
            int id, updated;
            string result,symbol;
            foreach (DataRow row in dt.Rows)
            {
                id = (int)row[0];
                symbol = row["symbolAsset"].ToString();
                Console.WriteLine();
                Console.Write(symbol+" "+id+" ");

                var res = ApiQuery.ParseResult<GlobalQuoteResult>(ApiQuery.quoteEndpoint(symbol));
                if (res.Count==0 || (DateTime.Today - res[0].latestDay).TotalDays > 7)
                {
                    db.deleteMarketAsset(id);
                    Console.Write("Deleted");
                    continue;
                }
                
                List<SearchingResult> results = ApiQuery.ParseResult<SearchingResult>(ApiQuery.search(symbol));
                if (results.Count > 0)
                {
                    int i = 0;
                    foreach(var item in results)
                    {
                        if (item.symbol.Equals(symbol))
                            break;
                        i++;
                    }
                    updated = db.updateMarketAssetCurrency(id,results[i].currency);
                    if(updated>0)
                        Console.Write("Updated");
                }
            }
        }
    }
}
