using System;
using System.Collections.Generic;
using System.Data;

namespace myWallet
{
    class Program
    {
        public static void showDataTable(DataTable dt)
        {
            foreach (DataColumn column in dt.Columns)
            {
                Console.Write("\t" + column.ColumnName);
            }
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                    Console.Write("\t" + row[column]);
            }
        }
        static void Main(string[] args)
        {
            DataManager dm = new();
            dm.updateCurrenciesInMarketAssets();
            Console.ReadLine();
        }
    }
}
