using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;

using System.Threading;

/// <summary>
/// Summary description for Watermark
/// </summary>
/// 
namespace SteganoWave{
public class Watermark
{
	public Watermark()
	{
		
    }
    public void btnHide_Click(string pathFile )
    {
        //hide the message inside the carrier wave


        

            Stream sourceStream = null;
            FileStream destinationStream = null;
            WaveStream audioStream = null;
            System.IO.Directory.GetCurrentDirectory();
            //create a stream that contains the message, preceeded by its length
            Stream messageStream = GetMessageStream();
            //open the key file
            string path = @"C:/Users/admin/Desktop/KTGT/GoogleDriveRestAPI-v3/GoogleDriveRestAPI-v3";
            //Stream keyStream = new FileStream()
            Stream keyStream = new FileStream( path + "/Key/keyfile.txt", FileMode.Open);
            //Stream s = new FileStream("../music/Còn Đâu Vòng Tay (Remix).mp3");

            try
            {

                //how man samples do we need?
                long countSamplesRequired = WaveUtility.CheckKeyForMessage(keyStream, messageStream.Length);

                if (countSamplesRequired > Int32.MaxValue)
                {
                    throw new Exception("Message too long, or bad key! This message/key combination requires " + countSamplesRequired + " samples, only " + Int32.MaxValue + " samples are allowed.");
                }

                //if (rdoSrcFile.Checked)
                //{ //use a .wav file as the carrier
                
                //sourceStream = new FileStream(path + "/Music/Charge.wav", FileMode.Open);
                sourceStream = new FileStream(pathFile, FileMode.Open);
                
                //}
                //else
                //{ //record a carrier wave
                //    frmRecorder recorder = new frmRecorder(countSamplesRequired);
                //    recorder.ShowDialog(this);
                //    sourceStream = recorder.RecordedStream;
                //}

               
                //create an empty file for the carrier wave
                
                //destinationStream = new FileStream(path + "/Encode/Charge.wav", FileMode.Create);
                destinationStream = new FileStream(pathFile, FileMode.Create);

                //copy the carrier file's header
                audioStream = new WaveStream(sourceStream, destinationStream);
                if (audioStream.Length <= 0)
                {
                    throw new Exception("Invalid WAV file");
                }

                //are there enough samples in the carrier wave?
                if (countSamplesRequired > audioStream.CountSamples)
                {
                    String errorReport = "The carrier file is too small for this message and key!\r\n"
                        + "Samples available: " + audioStream.CountSamples + "\r\n"
                        + "Samples needed: " + countSamplesRequired;
                    throw new Exception(errorReport);
                }

                //hide the message
                WaveUtility utility = new WaveUtility(audioStream, destinationStream);
                utility.Hide(messageStream, keyStream);
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                if (keyStream != null) { keyStream.Close(); }
                if (messageStream != null) { messageStream.Close(); }
                if (audioStream != null) { audioStream.Close(); }
                if (sourceStream != null) { sourceStream.Close(); }
                if (destinationStream != null) { destinationStream.Close(); }
                
            }
        }
    private Stream GetMessageStream()
    {
        string msg = "hoangtuan"; 
        BinaryWriter messageWriter = new BinaryWriter(new MemoryStream());
        messageWriter.Write(msg.Length);
        messageWriter.Write(Encoding.ASCII.GetBytes(msg));
        messageWriter.Seek(0, SeekOrigin.Begin);
        return messageWriter.BaseStream;
    }

    }
}
 
	
