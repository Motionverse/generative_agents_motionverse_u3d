using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Utils 
{
   

    /// <summary>
    /// 文件类
    /// </summary>
    //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    //public class FileOpenDialog
    //{
    //    public int structSize = 0;
    //    public IntPtr dlgOwner = IntPtr.Zero;
    //    public IntPtr instance = IntPtr.Zero;
    //    public String filter = null;
    //    public String customFilter = null;
    //    public int maxCustFilter = 0;
    //    public int filterIndex = 0;
    //    public String file = null;
    //    public int maxFile = 0;
    //    public String fileTitle = null;
    //    public int maxFileTitle = 0;
    //    public String initialDir = null;
    //    public String title = null;
    //    public int flags = 0;
    //    public short fileOffset = 0;
    //    public short fileExtension = 0;
    //    public String defExt = null;
    //    public IntPtr custData = IntPtr.Zero;
    //    public IntPtr hook = IntPtr.Zero;
    //    public String templateName = null;
    //    public IntPtr reservedPtr = IntPtr.Zero;
    //    public int reservedInt = 0;
    //    public int flagsEx = 0;
    //}

    /// <summary>
    /// 文件夹类
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class OpenDialogDir
    {
        public IntPtr hwndOwner = IntPtr.Zero;
        public IntPtr pidlRoot = IntPtr.Zero;
        public String pszDisplayName = "123";
        public String lpszTitle = null;
        public UInt32 ulFlags = 0;
        public IntPtr lpfn = IntPtr.Zero;
        public IntPtr lParam = IntPtr.Zero;
        public int iImage = 0;
    }

    public class DialogShow
    {
        //[DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        //public static extern bool GetOpenFileName([In, Out] FileOpenDialog dialog);

        //[DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        //public static extern bool GetSaveFileName([In, Out] FileOpenDialog dialog);

        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);
    }
    public static string ChooseDirectory(string title, string openPath = "")
    {
        OpenDialogDir openDir = new OpenDialogDir();
        openDir.pszDisplayName = new string(new char[2000]);
        openDir.lpszTitle = title;
        openDir.ulFlags = 1;// BIF_NEWDIALOGSTYLE | BIF_EDITBOX;
        IntPtr pidl = DialogShow.SHBrowseForFolder(openDir);

        char[] path = new char[2000];
        for (int i = 0; i < 2000; i++)
            path[i] = '\0';
        if (DialogShow.SHGetPathFromIDList(pidl, path))
        {
            string str = new string(path);
            string DirPath = str.Substring(0, str.IndexOf('\0'));
            return DirPath;
        }

        return "";
    }

}
