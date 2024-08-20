namespace WorkRestTool
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Save
    {
        public static Save Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Save();
                }
                return instance;
            }

            set { instance = value; }
        }
        private static Save? instance;

        public int OutHour { get; set; } = 19;
        public int OutMinute { get; set; } = 30;
        public int WorkHour { get; set; } = 6;
        public int WorkMinute { get; set; } = 0;
        public int WorkSecond { get; set; } = 0;
        public int MinWorkMinute { get; set; } = 30;
        public int MinRestMinute { get; set; } = 15;
        public int EatUseMinute { get; set; } = 45;
        public int EatHour { get; set; } = 17;
        public int EatMinute { get; set; } = 30;
    }
}
