using System;
using System.IO;
using AudioRecognizer.Models;
using Newtonsoft.Json;
using RestSharp;

namespace AudioRecognizer.Services
{
    public class EchoNest
    {
        private const string BaseUri = "http://developer.echonest.com/api/v4/";

        // GET API KEY FOR ECHONEST FROM HERE http://developer.echonest.com/account/register
        private string apiKey = "";
        public string ApiKey
        {
            get
            {
                if (apiKey == "")
                {
                    if (!File.Exists("ApiKey.txt"))
                        throw new ApplicationException("ApiKey.txt file is missing.  Please see ReadMe.txt");

                    apiKey = File.ReadAllText("ApiKey.txt").Trim();
                    if (apiKey == "") throw new ApplicationException("No ApiKey for EchoNest provided.  Please visit: http://developer.echonest.com/account/register and fill in key after registration.");
                }
                return apiKey;
            }
        }

        public EchoNestTrackResult GetTrackInformation(string id)
        {            
            var client = new RestClient(BaseUri);
            var request = new RestRequest("track/profile", Method.GET);
            request.AddParameter("api_key", ApiKey);
            request.AddParameter("format", "json");
            request.AddParameter("id", id);
            request.AddParameter("bucket", "audio_summary");

            IRestResponse response = client.Execute(request);
            dynamic trackResult = JsonConvert.DeserializeObject(response.Content);
            return new EchoNestTrackResult()
            {
                ServiceStatus = trackResult.response.status.message,
                Album = trackResult.response.track.release,
                Artist = trackResult.response.track.artist,
                Title = trackResult.response.track.title,
                TrackId = trackResult.response.track.id,
                TrackStatus = trackResult.response.track.status
            };
        }

        public EchoNestTrackResult IdentifyTrackFromFile(string filePath)
        {
            if (ApiKey == "") throw new ApplicationException("No ApiKey for EchoNest provided.  Please visit: http://developer.echonest.com/account/register and fill in key after registration.");

            var client = new RestClient(BaseUri);
            var request = new RestRequest("track/upload", Method.POST);
            request.AddParameter("api_key", ApiKey);
            request.AddParameter("filetype", Path.GetExtension(filePath).Replace(".",""));
            request.AddFile("track", filePath);

            IRestResponse response = client.Execute(request);
            dynamic uploadResult = JsonConvert.DeserializeObject(response.Content);

            return new EchoNestTrackResult()
            {
                ServiceStatus = uploadResult.response.status.message,
                Album = uploadResult.response.track.release,
                Artist = uploadResult.response.track.artist,
                Title = uploadResult.response.track.title,
                TrackId = uploadResult.response.track.id,
                TrackStatus = uploadResult.response.track.status
            };
        }

        public EchoNestTrackResult IdentifyTrackFromStream(MemoryStream stream, string filetype = "mp3")
        {

            if (ApiKey == "") throw new ApplicationException("No ApiKey for EchoNest provided.  Please visit: http://developer.echonest.com/account/register and fill in key after registration.");

            var client = new RestClient(BaseUri);
            var request = new RestRequest("track/upload", Method.POST);
            request.AddParameter("api_key", ApiKey);
            request.AddParameter("filetype", filetype);
            request.AddFile("track", stream.ToArray(), "track." + filetype);

            IRestResponse response = client.Execute(request);
            dynamic uploadResult = JsonConvert.DeserializeObject(response.Content);

            return new EchoNestTrackResult()
            {
                ServiceStatus = uploadResult.response.status.message,
                Album = uploadResult.response.track.release,
                Artist = uploadResult.response.track.artist,
                Title = uploadResult.response.track.title,
                TrackId = uploadResult.response.track.id,
                TrackStatus = uploadResult.response.track.status
            };
        }
    }
}
