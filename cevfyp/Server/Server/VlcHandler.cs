using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using ClassLibrary;

namespace Server
{
    class VlcHandler
    {
        IntPtr instance, player,media;
        libvlc_exception_t ex = new libvlc_exception_t();
        ServerConfig sConfig = new ServerConfig();

        public VlcHandler()
        {
            sConfig.load("C:\\ServerConfig.xml");
        }

        static void Raise(ref libvlc_exception_t ex)
        {
            if (LibVlc.libvlc_exception_raised(ref ex) != 0)
                MessageBox.Show(LibVlc.libvlc_exception_get_message(ref ex));
        }

        public void streaming(Panel p, string filesrc, string libsrc)
        {
            LibVlc.libvlc_exception_init(ref ex);

            string[] args = new string[]
            {
            "-I", "dummy", "--ignore-config",
            @"--plugin-path="+sConfig.PluginPath +"\\plugins",
            "--vout-filter=deinterlace", "--deinterlace-mode=blend"
            };

            instance = LibVlc.libvlc_new(args.Length, args, ref ex);
            Raise(ref ex);

             media = LibVlc.libvlc_media_new(instance, @""+filesrc, ref ex);
             Raise(ref ex);

             LibVlc.libvlc_media_add_option(media, @":sout=#duplicate{dst=display,dst=std{access=http,mux=" + sConfig.StreamType + ",dst=127.0.0.1:" + sConfig.VlcStreamPort + "}} :sout-all", ref ex);
           
            player = LibVlc.libvlc_media_player_new_from_media(media, ref ex);
            Raise(ref ex);

           

            LibVlc.libvlc_media_release(media);

           

            LibVlc.libvlc_media_player_set_drawable(player, p.Handle, ref ex);
            Raise(ref ex);

            
            LibVlc.libvlc_media_player_play(player, ref ex);
            Raise(ref ex);

        }

        public void pause()
        {
            LibVlc.libvlc_exception_init(ref ex);
            LibVlc.libvlc_media_player_pause(player, ref ex);
            Raise(ref ex);

          
        }

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
           int checkNo= LibVlc.libvlc_audio_get_mute(instance, ref ex);
            Raise(ref ex);
            return checkNo;
        }

        public int getVideoHeight()
        {
            LibVlc.libvlc_exception_init(ref ex);
            int height = LibVlc.libvlc_video_get_height(player, ref ex);
            Raise(ref ex);
            return height;
        }

        public int getVideoWidth()
        {
            LibVlc.libvlc_exception_init(ref ex);
            int width=LibVlc.libvlc_video_get_width(player, ref ex);
            Raise(ref ex);
            return width;
        }

        public float getFPS()
        {
            float fps = LibVlc.libvlc_media_player_get_fps(player, ref ex);
            Raise(ref ex);
            return fps;
        }

      

    }
}
