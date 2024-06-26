using System;
using System.Collections.Generic;

namespace BudgetManager.FinanceDataMining.Comodity.JsonModelDto
{
    internal class Dataset
    {
        public int id { get; set; }
        public string dataset_code { get; set; }
        public string database_code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime refreshed_at { get; set; }
        public string newest_available_date { get; set; }
        public string oldest_available_date { get; set; }
        public List<string> column_names { get; set; }
        public string frequency { get; set; }
        public string type { get; set; }
        public bool premium { get; set; }
        public object limit { get; set; }
        public object transform { get; set; }
        public object column_index { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public List<List<object>> data { get; set; }
        public object collapse { get; set; }
        public object order { get; set; }
        public int database_id { get; set; }
    }
}
