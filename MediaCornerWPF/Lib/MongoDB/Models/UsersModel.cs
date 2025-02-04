using Microsoft.VisualBasic.ApplicationServices;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCornerWPF.Lib.MongoDB.Models
{
    /// <summary>
    /// Model dla użytkownika
    /// </summary>
    public class UsersModel
    {
        public BsonObjectId _id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public List<string> Friends { get; set; } = new List<string>();
        public string profilePicture { get; set; }
    }
}