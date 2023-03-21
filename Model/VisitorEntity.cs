using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

namespace SimpleFunction.Model
{
    //[DebuggerDisplay("{RowKey}: {LastVisit}")]
    public class VisitorEntity : ITableEntity
    {
        public const string DateTimeDatabaseFormat = "yyyyMMddHHmmss";

        public string PartitionKey
        { 
            get => "partition";
            set { }
        }

        public string RowKey
        { 
            get => VisitorName;
            set => VisitorName = value; 
        }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        public DateTime LastVisitDateTime { get; set; }

        [IgnoreDataMember]
        public string VisitorName { get; set; }

        public string LastVisit
        {
            get => LastVisitDateTime.ToString(DateTimeDatabaseFormat);
            set => LastVisitDateTime = DateTime.ParseExact(
                value, 
                DateTimeDatabaseFormat, 
                CultureInfo.InvariantCulture);
        }



        //public override string ToString()
        //{
        //    return $"{RowKey}: {LastVisitDateTime:ddd dd MM yyyy, HH:mm:ss)}";
        //}
    }
}
