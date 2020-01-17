namespace Statistics.Models
{
    public class StatsResult
    {
        public string title { get; set; }
        public string uid { get; set; }

        int _totalDown;
        public int totalDown
        {
            get
            {
                return _totalDown > 0 ? _totalDown : 0;
            }
            set { _totalDown = value; }
        }

        int _totalUp;
        public int totalUp
        {
            get
            {
                return _totalUp > 0 ? _totalUp : 0;
            }
            set { _totalUp = value; }
        }

        int _curDownSpeed;
        public int curDownSpeed
        {
            get
            {
                return _curDownSpeed > 0 ? _curDownSpeed : 0;
            }
            set { _curDownSpeed = value; }
        }

        int _curUpSpeed;
        public int curUpSpeed
        {
            get
            {
                return _curUpSpeed > 0 ? _curUpSpeed : 0;
            }
            set { _curUpSpeed = value; }
        }

        public long stamp { get; set; } = -1;

        public StatsResult()
        {
            uid = string.Empty;
            title = string.Empty;
            totalDown = 0;
            totalUp = 0;
            curDownSpeed = 0;
            curUpSpeed = 0;
        }
    }
}
