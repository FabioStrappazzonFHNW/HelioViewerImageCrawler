using Newtonsoft.Json;
using System;
using System.IO;
using unirest_net.http;

namespace HelioViewerImageCrawler
{
    class Program
    {
        private const string HV_API = "https://api.helioviewer.org/v2/{0}/?date={1}&sourceId=10";
        private const string DATE_FORMAT = "yyyy-MM-ddTHH:mm:ssZ";

        static void Main(string[] args)
        {
            if(args.Length != 3)
            {
                Console.WriteLine("Usage: HelioViewerImageCrawler startDate imageCount timeInterval\n"+
                    "\nstartDate:\tDate of first Image\nimageCount:\thow many images\n"+
                    "timeInterval:\t[x1|x4|x16|x64|x256] x1 downloads every image (interval of 36 seconds), x4 every fourth and so on.\n\n"+
                    "example: HelioViewerImageCrawler 2018-1-1 3 x4");

            }
            else
            {
                var start = DateTime.Parse(args[0]);
                var imageCount = int.Parse(args[1]);
                TimeSpan? delta = null;
                switch (args[2])
                {
                    case   "x1": delta = new TimeSpan(0,  0, 35); break;
                    case   "x4": delta = new TimeSpan(0,  2, 24); break;
                    case  "x16": delta = new TimeSpan(0,  9, 26); break;
                    case  "x64": delta = new TimeSpan(0, 38, 24); break;
                    case "x256": delta = new TimeSpan(2, 33, 36); break;
                }
                if(delta.HasValue)
                {
                    DownloadImages(start, imageCount, delta.Value);
                }
                else
                {
                    Console.WriteLine("Unknown time interval. Use one of x1, x4, x16, x64, x256");
                }
            }
        }

        public static void DownloadImages(DateTime start, int imageCount, TimeSpan delta)
        {
            String lastDate = "";
            int counter = 1;
            while (counter <= imageCount)
            {
                Console.WriteLine("trying date " + start.ToString(DATE_FORMAT));
                var closestImageUrl = string.Format(HV_API, "getClosestImage", start.ToString(DATE_FORMAT));
                var response = Unirest.get(closestImageUrl).asString();
                if (response.Code != 200)
                {
                    Console.WriteLine("error getting closest date: " + response.Code);
                    continue;
                }
                start += delta;
                var date = JsonConvert.DeserializeObject<ClosestImageResponse>(response.Body).date;
                Console.WriteLine("closest date: " + date);
                if (date == lastDate)
                {
                    Console.WriteLine("skipping, same as last");
                    continue;
                }
                lastDate = date;
                var getImageUrl = string.Format(HV_API, "getJP2Image", DateTime.Parse(date).ToString(DATE_FORMAT));
                HttpResponse<Stream> imgResponse = Unirest.get(getImageUrl).asBinary();
                if (imgResponse.Code != 200)
                {
                    Console.WriteLine("error getting image: " + imgResponse.Code);
                    continue;
                }
                using (var fileStream = File.Create(@"./" + counter + ".jp2"))
                {
                    imgResponse.Body.CopyTo(fileStream);
                }
                Console.WriteLine("image downloaded\n");
                counter++;
            }
        }
        public struct ClosestImageResponse
        {
            public int id;
            public string date;
            public string name;
            public double scale;
            public int width;
            public int height;
            public double refPixelX;
            public double refPixelY;
            public object sunCenterOffsetParams;
            public int layeringOrder;

            public ClosestImageResponse(int id, string date, string name, double scale, int width, int height, double refPixelX, double refPixelY, object sunCenterOffsetParams, int layeringOrder)
            {
                this.id = id;
                this.date = date;
                this.name = name;
                this.scale = scale;
                this.width = width;
                this.height = height;
                this.refPixelX = refPixelX;
                this.refPixelY = refPixelY;
                this.sunCenterOffsetParams = sunCenterOffsetParams;
                this.layeringOrder = layeringOrder;
            }
        }
    }
}
