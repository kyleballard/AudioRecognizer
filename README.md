AudioRecognizer
===============
Analyzes your default audio output at time of execution and sends a sample to EchoNest for analysis. If EchoNest is able to identity the audio sampling, you will be presented with the Artist and Track information for the sample you recorded. This application is similar to Shazam, but works on your Windows desktop machine (Vista or higher). Note: You will need an API Key from EchoNest (http://developer.echonest.com/) in order for this to work properly.

Switches: 
-file
ex: AudioRecognizer -file "c:\temp\save_here.mp3"

-recordSeconds (asks recognizer to wait X seconds before sending for analysis)
ex: AudioRecognizer -recordSeconds 10

-analysisWaitSeconds (asks recognizer to wait X seconds before retrying request from EchoNest)
ex: AudioRecognizer -analysisWaitSeconds 10

-skipAnalysis (do not send file for Analysis)
ex: AudioRecognizer -skipAnalysis -file "c:\temp\save_here.mp3"
