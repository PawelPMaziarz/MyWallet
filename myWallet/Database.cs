using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;

namespace myWallet
{
    public class Database
    {
        private NpgsqlConnection con;
        public Database() : this("localhost", Secrets.dbUser, Secrets.dbPassword, Secrets.dbName) { }
        public Database(string host, string username, string password, string database)
        {
            using (con)
                con = new NpgsqlConnection($"Host={host};Username={username};Password={password};Database={database}");
            con.Open();
        }
        public DataTable getTablesList()
        {
            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM pg_catalog.pg_tables WHERE schemaname != 'pg_catalog' AND schemaname != 'information_schema'; ", con);
            using NpgsqlDataReader rdr = cmd.ExecuteReader();

            var dt = new DataTable();
            dt.Load(rdr);
            return dt;
        }
        public DataTable getMarketAssets()
        {
            DataTable dt = new DataTable();
            using (NpgsqlCommand cmd = new NpgsqlCommand("select * from public.\"MarketAssets\" where \"symbolCurrency\" like 'PLN' order by \"idMarketAsset\" asc", con))
            {
                dt.Load(cmd.ExecuteReader());
            }
            return dt;
        }
        public int deleteMarketAsset(int id)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("delete from public.\"MarketAssets\" where \"idMarketAsset\"=@id", con))
            {
                cmd.Parameters.AddWithValue("@id", id);
                return cmd.ExecuteNonQuery();
            }
        }
        public int updateMarketAssetCurrency(int id, string value)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand("update public.\"MarketAssets\" set \"symbolCurrency\"=@value where \"idMarketAsset\"=@id", con))
            {
                cmd.Parameters.AddWithValue("@value", value);
                cmd.Parameters.AddWithValue("@id",id);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
