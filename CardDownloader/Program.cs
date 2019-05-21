using System;
using System.Net;
using System.IO;
using System.Drawing;
using KevandConsole;

namespace CardDownloader
{
    class Program
    {

        public static void Main()
        {
            kc.ww("Nazwa boostera: ");
            string pack = kc.rl();
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/" + pack);
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/" + pack + "/Common");
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/" + pack + "/Uncommon");
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/" + pack + "/Rare");
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/" + pack + "/UltraRare");
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/" + pack + "/Energy");
            kc.wl("----------------");
            kc.ww("Od: ");
            int from = Convert.ToInt32(kc.rl());
            kc.ww("Do: ");
            int to = Convert.ToInt32(kc.rl());

            string collection = pack.Substring(0, 2).ToLower();
            string number = pack.Substring(2);

            bool twoDigit = false;

            if (number.Length == 2)
                twoDigit = true;

            using (var client = new WebClient())
            {

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                double fullSize = 0;

                for (int i = from; i <= to; i++)
                {

                    string url = "https://assets.pokemon.com/assets/cms2/img/cards/web/" + pack + "/" + pack + "_EN_" + i + ".png";

                    try
                    {
                        kc.cls();
                        kc.wl("Pobieranie karty nr. " + i);
                        fullSize += GetFileSize(new Uri(url));
                        client.DownloadFile(url, pack + @"\" + i.ToString() + ".png");
                    }
                    catch (WebException e)
                    {
                        kc.wl("Cant download card number " + i + " from pack called " + pack + " error: " + e);
                    }
                }

                kc.wl("Rozmiar: " + fullSize + " MB");

            }

            kc.wl("Sortowanie...");

            DirectoryInfo info = new DirectoryInfo(pack + "/");

            foreach(FileInfo fi in info.GetFiles())
            {
                kc.wl(fi.Name);
                if (imgdetect.Detect(new Bitmap(pack + @"\" + fi.Name), "Ultrarare"))
                    File.Move(pack + @"\" + fi.Name, pack + @"\Ultrarare\" + fi.Name);
                else if (imgdetect.Detect(new Bitmap(pack + @"\" + fi.Name), "Common"))
                    File.Move(pack + @"\" + fi.Name, pack + @"\Common\" + fi.Name);
                else if (imgdetect.Detect(new Bitmap(pack + @"\" + fi.Name), "Uncommon"))
                    File.Move(pack + @"\" + fi.Name, pack + @"\Uncommon\" + fi.Name);
                else if(imgdetect.Detect(new Bitmap(pack + @"\" + fi.Name), "Rare"))
                    File.Move(pack + @"\" + fi.Name, pack + @"\Rare\" + fi.Name);

            }

            kc.wl("Koniec sortowania");

            using (WebClient client = new WebClient())
            {
                if(twoDigit)
                    client.DownloadFile("https://assets.pokemon.com/assets/cms2/img/trading-card-game/series/" + collection + "_series/" + collection + number + "/" + collection + number + "_logo_169_en.png", pack + @"\cover.png");
                else
                    client.DownloadFile("https://assets.pokemon.com/assets/cms2/img/trading-card-game/series/" + collection + "_series/" + collection + "0" + number + "/" + collection + "0" + number + "_logo_169_en.png", pack + @"\cover.png");
            }

            kc.rk();

        }

        private static double GetFileSize(Uri uriPath)
        {
            var webRequest = WebRequest.Create(uriPath);
            webRequest.Method = "HEAD";

            using (var webResponse = webRequest.GetResponse())
            {
                var fileSize = webResponse.Headers.Get("Content-Length");
                var fileSizeInMegaByte = Math.Round(Convert.ToDouble(fileSize) / 1024.0 / 1024.0, 2);
                return fileSizeInMegaByte;
            }
        }

    }
}