using System;
using System.IO;
using NAudio.Lame;
using NAudio.Wave;

namespace AudioRecognizer.Services
{
    public class Recorder
    {
        #region "MP3 Recording"

        private LameMP3FileWriter writer;
        private bool _isRecording = false;

        public MemoryStream GetMp3Sample(int seconds, string filepath = "")
        {
            try
            {
                var stream = new MemoryStream();
                // Start recording from loopback
                IWaveIn waveIn = new WasapiLoopbackCapture();
                waveIn.DataAvailable += waveIn_DataAvailable;
                waveIn.RecordingStopped += waveIn_RecordingStopped;
                // Setup MP3 writer to output at 96kbit/sec
                writer = new LameMP3FileWriter(stream, waveIn.WaveFormat, LAMEPreset.ABR_96);
                _isRecording = true;
                waveIn.StartRecording();

                // Wait for X seconds
                System.Threading.Thread.Sleep(seconds * 1000);
                waveIn.StopRecording();

                // flush output to finish MP3 file correctly
                writer.Flush();
                // Dispose of objects
                waveIn.Dispose();
                writer.Dispose();

                if (filepath != "")
                    using (var file = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                        stream.WriteTo(file);

                return stream;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private void waveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            // signal that recording has finished
            _isRecording = false;
        }

        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // write recorded data to MP3 writer
            if (writer != null)
                writer.Write(e.Buffer, 0, e.BytesRecorded);
        }

        #endregion

        #region "WAV Recording"
        private IWaveIn _waveIn;
        private WaveFileWriter _writer;
        private string _fileName = "";

        public MemoryStream GetWavSample(int seconds, string filepath)
        {
            var stream = new MemoryStream();
            _fileName = filepath;
            _waveIn = new WasapiLoopbackCapture();
            _writer = new WaveFileWriter(stream, _waveIn.WaveFormat);
            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.RecordingStopped += OnRecordingStopped;
            _waveIn.StartRecording();
            _isRecording = true;
            System.Threading.Thread.Sleep(seconds * 1000);
            _waveIn.StopRecording();

            if (filepath != "")
                using (var file = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                    stream.WriteTo(file);

            return stream;
        }

        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (_writer != null)
            {
                _writer.Close();
                _writer = null;
            }

            if (_waveIn != null)
            {
                _waveIn.Dispose();
                _waveIn = null;
            }
            _isRecording = false;
            if (e.Exception != null)
                throw e.Exception;
        }

        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            _writer.Write(e.Buffer, 0, e.BytesRecorded);
            //int secondsRecorded = (int)(_writer.Length / _writer.WaveFormat.AverageBytesPerSecond);
        } 
        #endregion  
    }
}
