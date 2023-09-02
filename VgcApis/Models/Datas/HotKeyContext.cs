using System;

namespace VgcApis.Models.Datas
{
    public class HotKeyContext
    {
        public uint modifier { get; set; }
        public uint key { get; set; }
        public int evCode { get; set; }
        public Action handler { get; set; }

        public uint KeyMessage { get; set; }

        public HotKeyContext() { }

        public static HotKeyContext Create(
            Action handler,
            string keyName,
            bool hasAlt,
            bool hasCtrl,
            bool hasShift
        )
        {
            if (
                !Misc.Utils.TryParseKeyMesssage(
                    keyName,
                    hasAlt,
                    hasCtrl,
                    hasShift,
                    out uint modifier,
                    out uint key
                )
            )
            {
                return null;
            }

            var context = new HotKeyContext()
            {
                handler = handler,
                modifier = modifier,
                key = key,
                KeyMessage = (key << 16) | modifier,
            };

            return context;
        }
    }
}
