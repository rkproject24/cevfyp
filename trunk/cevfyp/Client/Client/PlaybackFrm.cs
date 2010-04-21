using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ClassLibrary;

namespace Client
{
    public partial class PlaybackFrm : Form
    {
        //public IntPtr instance, player;

        libvlc_exception_t ex = new libvlc_exception_t();

        static void Raise(ref libvlc_exception_t ex)
        {
            if (LibVlc.libvlc_exception_raised(ref ex) != 0)
                Console.WriteLine(LibVlc.libvlc_exception_get_message(ref ex));
        }

        public PlaybackFrm()
        {
            InitializeComponent();
        }

        public void rePlay(IntPtr instance, IntPtr player )
        {
            //vlc-1.0.0 version
            LibVlc.libvlc_media_player_set_hwnd(player, playPanel.Handle, ref ex);
            Raise(ref ex);

            LibVlc.libvlc_media_player_play(player, ref ex);
            Raise(ref ex);

        }
    }
}
