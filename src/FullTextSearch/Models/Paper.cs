using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FullTextSearch.Models
{
    public class Paper
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string EventType { get; set; }
        
        public string PdfName { get; set; }

        [Required]
        public string Abstract { get; set; }

        [Required]
        public string PaperText { get; set; }

        public double Rank { get; set; }
    }
}
