namespace ViberSender2017
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class MakeSends
    {
        public bool SendSms(string number, string text, bool first, string path = null)
        {
            if (first && !WinApi.StartWork())
            {
                return false;
            }
            WinApi.ClickNumber();
            WinApi.EnterNumber(number);
            WinApi.ClickMessage();
            Thread.Sleep(200);
            if (path != null)
            {
                return WinApi.SendMsg(text, path, true);
            }
            return WinApi.SendMsg(text, null, false);
        }
    }
}

