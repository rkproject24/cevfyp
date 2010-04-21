using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ClassLibrary;
using System.Threading;

namespace Client
{
    public class VlcHandler
    {
        IntPtr instance, player;
        libvlc_exception_t ex = new libvlc_exception_t();
        ClientConfig cConfig = new ClientConfig();
        PlaybackFrm playform;

        string[] args;
        int boardcastport;
        public bool playing = false;
        int httpCaching = 2000;

        public delegate void RepaintPlayPanel(IntPtr instance2, IntPtr player2);
        //public delegate void RepaintPlayPanel(string[] args2, int boardcastport2, VlcHandler vlchld);

        public VlcHandler(string configPath)
        {
            cConfig.load(configPath);//"C:\\ClientConfig");
            args = new string[]
            {
            "-I", "dummy", "--no-ignore-config", "--http-caching="+httpCaching,"--http-reconnect","--ipv4-timeout=2000", 
            @"--plugin-path="+cConfig.PluginPath+"\\plugins",
            "--vout-filter=deinterlace", "--deinterlace-mode=blend"
            };

            //"--ignore-config",
        }

        public int getHttpCaching()
        {
            return httpCaching;
        }

        public bool getPlayingState()
        {
            return playing;
        }


        static void Raise(ref libvlc_exception_t ex)
        {
            if (LibVlc.libvlc_exception_raised(ref ex) != 0)
                MessageBox.Show(LibVlc.libvlc_exception_get_message(ref ex));
        }

        public void play(PlaybackFrm playform, int port)
        {
            this.playform = playform;
            playing = true;

            boardcastport = port;

            LibVlc.libvlc_exception_init(ref ex);

            //"--http-caching=2000"

            instance = LibVlc.libvlc_new(args.Length, args, ref ex);
            Raise(ref ex);

            IntPtr media = LibVlc.libvlc_media_new(instance, @"http://127.0.0.1:" + boardcastport.ToString(), ref ex);
            Raise(ref ex);

            // LibVlc.libvlc_media_add_option(media, @" :drop-late-frames", ref ex);
            //LibVlc.libvlc_media_add_option(media, @":dst="+cConfig.Localdisplay, ref ex);

            player = LibVlc.libvlc_media_player_new_from_media(media, ref ex);
            Raise(ref ex);

            LibVlc.libvlc_media_release(media);

            playform.playPanel.BeginInvoke(new RepaintPlayPanel(playform.rePlay), new object[] { instance, player });
            //////vlc-0.9.9 version
            ////LibVlc.libvlc_media_player_set_drawable(player, p.Handle, ref ex);

            ////vlc-1.0.0 version
            //LibVlc.libvlc_media_player_set_hwnd(player, playform.playPanel.Handle, ref ex);
            //Raise(ref ex);

            //LibVlc.libvlc_media_player_play(player, ref ex);
            //Raise(ref ex);

            setMute(1);
        }

        /*    public void pause()
            {
                LibVlc.libvlc_exception_init(ref ex);
                LibVlc.libvlc_media_player_pause(player, ref ex);
                Raise(ref ex);
            }
            */

        public void stop()
        {
            LibVlc.libvlc_exception_init(ref ex);
            LibVlc.libvlc_media_player_stop(player, ref ex);
            Raise(ref ex);
            LibVlc.libvlc_media_player_release(player);
            LibVlc.libvlc_release(instance);
            playing = false;
        }


        public void setMute(int status)
        {
            LibVlc.libvlc_exception_init(ref ex);
            LibVlc.libvlc_audio_set_mute(instance, status, ref ex);
            Raise(ref ex);
        }

        public int getMute()
        {
            LibVlc.libvlc_exception_init(ref ex);
            int checkNo = LibVlc.libvlc_audio_get_mute(instance, ref ex);
            Raise(ref ex);
            return checkNo;
        }

        public int getVolume()
        {
            LibVlc.libvlc_exception_init(ref ex);
            int vol = LibVlc.libvlc_audio_get_volume(instance);
            Raise(ref ex);
            return vol;
        }

        public void setVolume(int vol)
        {
            LibVlc.libvlc_exception_init(ref ex);
            LibVlc.libvlc_audio_set_volume(instance, vol, ref ex);
            Raise(ref ex);

        }
        public void restart(int port)
        {
            //if (playing)//avoid multiple thread restarting
            //{
            //    playing = false;
            //    this.stop();
            //    boardcastport = port;
                

            //    LibVlc.libvlc_exception_init(ref ex);

            //    //"--http-caching=2000"

            //    instance = LibVlc.libvlc_new(args.Length, args, ref ex);
            //    Raise(ref ex);
            //    IntPtr media = LibVlc.libvlc_media_new(instance, @"http://127.0.0.1:" + boardcastport.ToString(), ref ex);
            //    Raise(ref ex);
            //    player = LibVlc.libvlc_media_player_new_from_media(media, ref ex);
            //    Raise(ref ex);
            //    LibVlc.libvlc_media_release(media);

            //    playform.playPanel.BeginInvoke(new RepaintPlayPanel(playform.rePlay), new object[] { instance, player });
            //    Console.WriteLine("vlcRestart");

            //    setMute(1);
            //    playing = true;
            //}

            boardcastport = port;
            LibVlc.libvlc_exception_init(ref ex);
            LibVlc.libvlc_media_player_stop(player, ref ex);
            Raise(ref ex);
            LibVlc.libvlc_media_player_release(player);
            Raise(ref ex);

            Thread.Sleep(100);

            IntPtr media = LibVlc.libvlc_media_new(instance, @"http://127.0.0.1:" + boardcastport.ToString(), ref ex);
            Raise(ref ex);
            LibVlc.libvlc_media_player_play(player, ref ex);
            Raise(ref ex);
            LibVlc.libvlc_media_release(media);
            Raise(ref ex);
        }
    }
}
