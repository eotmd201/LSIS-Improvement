using OpenCvSharp;
using System;

namespace LSIS
{
    class PacketPoint
    {
        public int frameCount;
        public TimeSpan videotimecount;
        public Point point;
        public double speed = 0;
        public double range = 0;
        public double time;
        public double totalspeed = 0;
        public double totaltime = 0;
        public double totaldist = 0;
        public String nowtime;
        public String Num;

        public PacketPoint(TimeSpan videotime, int x, int y)
        {
            videotimecount = videotime;
            point = new Point(x, y);
        }

        public void SetPoint(int x, int y)
        {
            point = new Point(x, y);
        }

    }
}
