using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notes.Models
{
    public class Note
    {
        public int ID { get; set; }
        public string Text { get; set; }
    }
}
