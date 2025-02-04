using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCornerWPF.Lib.API.Models
{
    /// <summary>
    /// Model dla filmu
    /// </summary>
    public class MovieModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string overview { get; set; }
        public string poster_path { get; set; }
        public string release_date { get; set; }
        public double vote_average { get; set; }
    }
}
