using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorkingWithImages.Models
{
    public class HTask
    {
        public System.Guid HTaskId { get; set; }
        public string Title { get; set; }
        public string Discription { get; set; }
        public byte[] Image { get; set; }
        public int ImageMimeType { get; set; }
        public byte[] thumbnail { get; set; }
    }
}