using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace myWallet
{
    public class SearchingResult
    {
        public string symbol { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string region { get; set; }
        public string marketOpen { get; set; }
        public string marketClose { get; set; }
        public string timezone { get; set; }
        public string currency { get; set; }
        public double matchScore { get; set; }
        public override string ToString()
        {
            return $"\tSymbol: {symbol}\n\tName: {name}\n\tType: {type}\n\tRegion: {region}\n\tCurrency: {currency}";
        }
    }
    public class GlobalQuoteResult
    {
        public string symbol { get; set; }
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double price { get; set; }
        public int volume { get; set; }
        public DateTime latestDay { get; set; }
        public double previousClose { get; set; }
        public double change { get; set; }
        public string changePercent { get; set; }

        public override string ToString()
        {
            return $"\tSymbol: {symbol}\n\tPrice: {price}\n\tLatest trading day: {latestDay}";
        }
    }
    public class DailyResult
    {
        public DateTime timestamp { get; set; }
        public double open { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double close { get; set; }
        public int volume { get; set; }
    }
    public class ApiQuery
    {
        private static string start = "https://www.alphavantage.co/query?function=";
        private static string end = "&datatype=csv&apikey="+Secrets.apiKey;
        private static string makeQuery(string queryURL)
        {
            string result = "";
            using (WebClient client = new WebClient())
            {
                try
                {
                    var uri = new Uri(queryURL);
                    result = client.DownloadString(uri);
                    while (result.Contains("higher API call"))
                    {
                        Console.WriteLine("Please wait a moment due to the limitation to 150 API call per minute (10seconds)");
                        Thread.Sleep(10000);
                        result = client.DownloadString(uri);
                    }
                }
                catch (WebException e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }
            return result;
        }
        public static string search(string keyword)
        {
            string queryURL = $"{start}SYMBOL_SEARCH&keywords={keyword}{end}";
            var result = makeQuery(queryURL);
            return result;
        }
        public static string daily(string symbol)
        {
            return daily(symbol, "full");
        }
        public static string daily(string symbol, string outputsize)
        {
            if (!outputsize.Equals("full") && !outputsize.Equals("compact"))
            {
                outputsize = "full";
                Console.Error.WriteLine("Output size in function daily should be full or compact");
            }
            string queryURL = $"{start}TIME_SERIES_DAILY&symbol={symbol}&outputsize={outputsize}{end}";
            var result = makeQuery(queryURL);
            return result;
        }
        public static string quoteEndpoint(string symbol)
        {
            string queryURL = $"{start}GLOBAL_QUOTE&symbol={symbol}{end}";
            var result = makeQuery(queryURL);
            return result;
        }
        public static List<T> ParseResult<T>(string result)
        {
            if (result.Length <= 2) return new List<T>();
            using var textReader = new StringReader(result);
            using var csvr = new CsvReader(textReader, System.Globalization.CultureInfo.InvariantCulture);
            return csvr.GetRecords<T>().ToList();
        }
    }
}
