using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Converter;
using System.IO;

namespace ytdownloader
{
    public partial class F_Downloader : Form
    {
        public F_Downloader()
        {
            InitializeComponent();
        }
       
        private async  void button1_Click(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            string apath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);


            Console.WriteLine(path);

            string link = link_box.Text ;
            string reg = @"^https://\w{0,3}.?youtube+\.\w{2,3}/watch\?v=[\w-]{11}";
            bool x = System.Text.RegularExpressions.Regex.IsMatch(link, reg);
            if (!x)
            {
                MessageBox.Show("Enter a valid url");
                return;
            }
            if(audioonly.Checked)
            {
                button1.Enabled = false;
                button1.Text = "Downloading...";
                var yt = new YoutubeClient();
                var vid = await yt.Videos.GetAsync(link);
                var tit = vid.Title;
                string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

                foreach (char c in invalid)
                {
                    tit = tit.Replace(c.ToString(), "");
                }
                var auth = vid.Author.Title;
                var dur = vid.Duration;
                Console.WriteLine(tit);
                Console.WriteLine(auth);
                Console.WriteLine(dur);
                var sM = await yt.Videos.Streams.GetManifestAsync(link);
                var sI = sM.GetAudioOnlyStreams().GetWithHighestBitrate();

                await yt.Videos.Streams.DownloadAsync(sI, apath + @"\" + $"{tit}audio.{sI.Container}");
                MessageBox.Show($"Audio Downloaded! See in {apath}", "Message");
                button1.Text = "Download";

                button1.Enabled = true;
                link_box.Text = "";
                audioonly.Checked = false;
                return;



            }
            else
            {
                Console.WriteLine(comboBox1.Text);
                if(comboBox1.Text == "")
                {
                    MessageBox.Show("Resolution is required");
                    return;

                }
                button1.Enabled = false;
                button1.Text = "Downloading...";

                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync(link);
                var title = video.Title;
                string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

                foreach (char c in invalid)
                {
                    title = title.Replace(c.ToString(), "");
                }
                var author = video.Author.Title;
                var duration = video.Duration;
                Console.WriteLine(title);
                Console.WriteLine(author);
                Console.WriteLine(duration);
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(link);
                var audioStreamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();
                try
                {
                    var videoStreamInfo = streamManifest.GetVideoStreams().First(s => s.VideoQuality.Label == comboBox1.Text);
                    var streamInfos = new IStreamInfo[] { audioStreamInfo, videoStreamInfo };
                    await youtube.Videos.DownloadAsync(streamInfos, new ConversionRequestBuilder(path + @"\" + $"{title}.mp4").Build());
                    MessageBox.Show($"Video Downloaded! See in {path}", "Message");
                    button1.Enabled = true;
                    link_box.Text = "";


                }
                catch (Exception ex)
                {
                    var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
                    await youtube.Videos.Streams.DownloadAsync(streamInfo, path + @"\" + $"{title}.{streamInfo.Container}");
                    MessageBox.Show("FFmpeg binary is missing so Downloaded in highest quality available. Please add ffmpeg to path", "Message");
                    MessageBox.Show($"Video Downloaded! See in {path}", "Message");
                    button1.Enabled = true;
                    link_box.Text = "";
                }
                button1.Enabled = true;
                button1.Text = "Download";
                return;

            }
         




        }

     
    }
}
