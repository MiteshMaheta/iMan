using System;
using System.Collections.Generic;
using System.Text;
using Prism.Mvvm;
using SQLite;

namespace iMan.Data
{
    [Table("item")]
    public class Item : Entity
    {

        private string categoryId;
        [Column("categoryId")]
        public string CategoryId
        {
            get { return categoryId; }
            set { SetProperty(ref categoryId,value); }
        }

        private double rate;
        [Column("rate")]
        public double Rate
        {
            get { return rate; }
            set { SetProperty(ref rate, value); }
        }

        private string name;
        [Column("name")]
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string unit;
        [Column("unit")]
        public string Unit
        {
            get { return unit; }
            set { SetProperty(ref unit, value); }
        }
    }

    [Table("category")]
    public class Category : BindableBase
    {
        private String TempId;
        [Column("id"), PrimaryKey, AutoIncrement]
        public String Id
        {
            get { return TempId; }
            set { SetProperty(ref TempId, value); }
        }

        private string name;
        [Column("name")]
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

    }
}
