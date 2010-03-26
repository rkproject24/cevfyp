using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ClassLibrary
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct libvlc_exception_t
    {
        public int b_raised;
        public int i_code;
        [MarshalAs(UnmanagedType.LPStr)]
        public string psz_message;
    }

    public static class LibVlc
    {
        #region core
        [DllImport("libvlc")]
        public static extern IntPtr libvlc_new(int argc, [MarshalAs(UnmanagedType.LPArray,
          ArraySubType = UnmanagedType.LPStr)] string[] argv, ref libvlc_exception_t ex);

        [DllImport("libvlc")]
        public static extern void libvlc_release(IntPtr instance);
        #endregion

        #region media
        [DllImport("libvlc")]
        public static extern IntPtr libvlc_media_new(IntPtr p_instance,
          [MarshalAs(UnmanagedType.LPStr)] string psz_mrl, ref libvlc_exception_t p_e);

        [DllImport("libvlc")]
        public static extern void libvlc_media_add_option(IntPtr media, [MarshalAs(UnmanagedType.LPStr)] string ppsz_options, ref libvlc_exception_t ex);

        [DllImport("libvlc")]
        public static extern void libvlc_media_release(IntPtr p_meta_desc);
        #endregion

        #region audio
        [DllImport("libvlc")]
        public static extern void libvlc_audio_set_mute(IntPtr p_instance, int status, ref libvlc_exception_t ex);

        [DllImport("libvlc")]
        public static extern int libvlc_audio_get_mute(IntPtr p_instance, ref libvlc_exception_t ex);

        [DllImport("libvlc")]
        public static extern int libvlc_audio_get_volume (IntPtr p_instance);

        [DllImport("libvlc")]
        public static extern void libvlc_audio_set_volume(IntPtr p_instance, int status, ref libvlc_exception_t ex);

        
        #endregion

        #region video
        [DllImport("libvlc")]
        public static extern int libvlc_video_get_width(IntPtr player, ref libvlc_exception_t ex);

        [DllImport("libvlc")]
        public static extern int libvlc_video_get_height(IntPtr player, ref libvlc_exception_t ex);
        #endregion

        #region media player
        [DllImport("libvlc")]
        public static extern IntPtr libvlc_media_player_new_from_media(IntPtr media,
          ref libvlc_exception_t ex);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_release(IntPtr player);

        ////vlc-0.9.9 version
        //[DllImport("libvlc")]
        //public static extern void libvlc_media_player_set_drawable(IntPtr player, IntPtr drawable,
        //  ref libvlc_exception_t p_e);

        //vlc-1.0.0 version
        [DllImport("libvlc")]
        public static extern void libvlc_media_player_set_hwnd(IntPtr player, IntPtr drawable,ref libvlc_exception_t p_e);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_play(IntPtr player, ref libvlc_exception_t ex);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_pause(IntPtr player, ref libvlc_exception_t ex);

        [DllImport("libvlc")]
        public static extern void libvlc_media_player_stop(IntPtr player, ref libvlc_exception_t ex);

        [DllImport("libvlc")]
        public static extern float libvlc_media_player_get_fps(IntPtr player, ref libvlc_exception_t ex);


        #endregion

        #region exception
        [DllImport("libvlc")]
        public static extern void libvlc_exception_init(ref libvlc_exception_t p_exception);

        [DllImport("libvlc")]
        public static extern int libvlc_exception_raised(ref libvlc_exception_t p_exception);

        [DllImport("libvlc")]
        public static extern string libvlc_exception_get_message(ref libvlc_exception_t p_exception);
        #endregion
    }
}
