using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioRecognizer.Services;
using NAudio.Lame;
using NAudio.Wave;

namespace AudioRecognizer
{
    class Program
    {
        private static string filePath = "";
        private static int recordSeconds = 8;
        private static int analysisWaitSeconds = 5;
        private static bool skipAnalysis = false;

        static void Main(string[] args)
        {
            try
            {
                GetSwitchData(args);

                Console.WriteLine("Recording " + recordSeconds + " seconds of audio from current output stream...");
                //var audioStream = new Recorder().GetMp3Sample(recordSeconds, filePath);
                var audioStream = new Recorder().GetWavSample(recordSeconds, filePath);

                if (!skipAnalysis)
                {
                    var _client = new EchoNest();
                    Console.WriteLine("Uploading file to EchoNest for identification...");
                    var result = _client.IdentifyTrackFromStream(audioStream, "wav");
                    while (result.TrackStatus == "pending")
                    {
                        System.Threading.Thread.Sleep(analysisWaitSeconds * 1000);
                        Console.WriteLine("Waiting for analysis...");
                        result = _client.GetTrackInformation(result.TrackId);
                    }

                    if (result.TrackStatus == "complete" && result.Title == null)
                        Console.WriteLine("Unable to identify track...");
                    else if (result.TrackStatus == "complete")
                        Console.WriteLine(result.Artist + ": " + result.Title);
                    else
                        Console.WriteLine("Unable to process request " + result.ServiceStatus);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Application failure: " + ex.Message);                 
            }
            
            Console.WriteLine("Press [Enter] to continue...");
            Console.ReadLine();
        }

        static void GetSwitchData(string[] args)
        {
            for (int idx = 0; idx < args.Length; idx++)
            {
                string arg = args[idx];
                if (arg == "-file" && args.Length > idx)
                {
                    filePath = args[idx + 1];
                    if (filePath.IndexOfAny(Path.GetInvalidPathChars()) > -1) 
                        throw new ApplicationException("Invalid file path specified.");
                }
                    
                if (arg == "-recordSeconds" && args.Length > idx)
                    Int32.TryParse(args[idx + 1], out recordSeconds);

                if (arg == "-analysisWaitSeconds" && args.Length > idx)
                    Int32.TryParse(args[idx + 1], out analysisWaitSeconds);

                if (arg == "-skipAnalysis")
                    skipAnalysis = true;
            }
            
        }
  
    }
}
