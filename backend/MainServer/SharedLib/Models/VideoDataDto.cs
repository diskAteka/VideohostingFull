using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Models
{
    public class VideoDataDto
    {
        public bool Exist { get; set; } = true; 
        public string VideoPath { get; set; }
        public string Format { get; set; } = "video/mp4";
        public bool EnableRangeProcessing { get; set; } = true;
    }
}
