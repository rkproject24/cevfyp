using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ClassLibrary;

namespace Client
{
    class VlcHandler
    {
        IntPtr instance, player;
        libvlc_exception_t ex = new libvlc_exception_t();
        ClientConfig cConfig = new ClientConfig();

        string[] args;
        int boardcastport;

        public VlcHandler()
        {
            cConfig.load("C:\\ClientConfig");
            args = new string[]
            {
            "-I", "dummy", "--ignore-config",
            @"--plugin-path="+cConfig.PluginPath+"\\plugins",
            "--vout-filter=deinterlace", "--deinterlace-mode=blend"
            };
        }

        static void Raise(ref libvlc_exception_t ex)
        {
            if (LibVlc.libvlc_exception_raised(ref ex) != 0)
                MessageBox.Show(LibVlc.libvlc_exception_get_message(ref ex));
        }

        public void play(Panel p,int port)
        {
            boardcastport = port;

            LibVlc.libvlc_exception_init(ref ex);

            //"--http-caching=2000"

            instance = LibVlc.libvlc_new(args.Length, args, ref ex);
            //Raise(ref ex);

            IntPtr media = LibVlc.libvlc_media_new(instance, @"http://127.0.0.1:"+ boardcastport.ToString(), ref ex);
            //Raise(ref ex);

           // LibVlc.libvlc_media_add_option(media, @" :drop-late-frames", ref ex);
            
            player = LibVlc.libvlc_media_player_new_from_media(media, ref ex);
            //Raise(ref ex);

            LibVlc.libvlc_media_release(media);

            LibVlc.libvlc_media_player_set_drawable(player, p.Handle, ref ex);
            //Raise(ref ex);

            LibVlc.libvlc_media_player_play(player, ref ex);
            //Raise(ref ex);
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
    }
}
