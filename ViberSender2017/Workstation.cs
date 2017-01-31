namespace ViberSender2017
{
    using System;
    using System.Management;
    using System.Text;

    public static class Workstation
    {
        public static string GenerateWorkstationId()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher();
            StringBuilder builder = new StringBuilder();
            searcher.Query = new ObjectQuery("select * from Win32_Processor");
            foreach (ManagementObject obj2 in searcher.Get())
            {
                builder.Append(ManagmentObjectPropertyData(obj2.Properties["ProcessorId"]));
                builder.Append(',');
            }
            searcher.Query = new ObjectQuery("select * from Win32_BaseBoard");
            foreach (ManagementObject obj3 in searcher.Get())
            {
                builder.Append(ManagmentObjectPropertyData(obj3.Properties["Product"]));
                builder.Append(',');
            }
            return builder.ToString();
        }

        private static string ManagmentObjectPropertyData(PropertyData data)
        {
            string str = string.Empty;
            if ((data.Value == null) || string.IsNullOrEmpty(data.Value.ToString()))
            {
                return str;
            }
            switch (data.Value.GetType().ToString())
            {
                case "System.String[]":
                {
                    string[] strArray = (string[]) data.Value;
                    foreach (string str3 in strArray)
                    {
                        str = str + str3 + " ";
                    }
                    return str;
                }
                case "System.UInt16[]":
                {
                    ushort[] numArray = (ushort[]) data.Value;
                    foreach (ushort num3 in numArray)
                    {
                        str = str + num3 + " ";
                    }
                    return str;
                }
            }
            return data.Value.ToString();
        }
    }
}

