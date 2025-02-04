using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCornerWPF.Lib.MongoDB.Models
{
    /// <summary>
    /// Model dla watchlisty
    /// </summary>
    public class WatchlistedModel
    {
        public BsonObjectId _id { get; set; }
        public string UsersId { get; set; }
        public int MovieId { get; set; }
        public DateTime Timestamp { get; set; } 
    }
}
