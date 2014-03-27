using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioRecognizer.Models
{
    public class EchoNestTrackResult
    {
        public string ServiceStatus { get; set; }
        public string TrackStatus { get; set; }
        public string TrackId { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
    }
}
