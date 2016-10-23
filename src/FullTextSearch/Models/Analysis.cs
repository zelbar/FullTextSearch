using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FullTextSearch.Models
{
    public class Analysis
    {
        [Required]
        public DateTime From { get; set; }
        
        [Required]
        public DateTime To { get; set; }

        [Required]
        public string Term { get; set; }

        [Required]
        public AnalysisType Type { get; set; }

        public string[][] Results { get; set; }
    }
}
