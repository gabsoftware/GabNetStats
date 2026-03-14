using GabNetStats.Properties;

namespace GabNetStats
{
    internal static class SpeedUtils
    {
        internal enum eBandwithUnit : int
        {
            bit  = 8,
            Byte = 1
        }
        internal enum eBandwidthMultiplier : long
        {
            un = 1,               // 2^0
            K  = 1024,            // 2^10
            M  = 1048576,         // 2^20
            G  = 1073741824,      // 2^30
            T  = 1099511627776    // 2^40
        }

        internal static class SpeedUnitsByte
        {
            public static string Bytes = Res.str_Bytes;
            public static string KiB   = Res.str_KiB;
            public static string MiB   = Res.str_MiB;
            public static string GiB   = Res.str_GiB;
            public static string TiB   = Res.str_TiB;
        }
        internal static class SpeedUnitsBit
        {
            public static string bit  = Res.str_bit;
            public static string Kbit = Res.str_Kbit;
            public static string Mbit = Res.str_Mbit;
            public static string Gbit = Res.str_Gbit;
            public static string Tbit = Res.str_Tbit;
        }


        internal static double computeSpeed(long rawSpeed, ref string speedUnit)
        {
            return computeSpeed(rawSpeed, ref speedUnit, 1);
        }

        internal static double computeSpeed(long rawSpeed, ref string speedUnit, int typeunit)
        {
            double res = 0;
            switch (typeunit)
            {
                case 1:
                    if (rawSpeed >= 1073741824) //1073741824 = 2 ^ 30
                    {
                        speedUnit = SpeedUnitsByte.GiB;
                        res = System.Math.Round(rawSpeed / (double)1073741824, 2);
                    }
                    else if (rawSpeed >= 1048576) //1048576 = 2 ^ 20
                    {
                        speedUnit = SpeedUnitsByte.MiB;
                        res = System.Math.Round(rawSpeed / (double)1048576, 2);
                    }
                    else if (rawSpeed >= 1024) //1024 = 2 ^ 10
                    {
                        speedUnit = SpeedUnitsByte.KiB;
                        res = System.Math.Round(rawSpeed / (double)1024, 2);
                    }
                    else
                    {
                        speedUnit = SpeedUnitsByte.Bytes;
                        res = rawSpeed;
                    }
                    break;

                case 2:
                    if (rawSpeed >= 1000000000)
                    {
                        speedUnit = SpeedUnitsBit.Gbit;
                        res = System.Math.Round(rawSpeed / (double)1000000000, 2);
                    }
                    else if (rawSpeed >= 1000000)
                    {
                        speedUnit = SpeedUnitsBit.Mbit;
                        res = System.Math.Round(rawSpeed / (double)1000000, 2);
                    }
                    else if (rawSpeed >= 1000)
                    {
                        speedUnit = SpeedUnitsBit.Kbit;
                        res = System.Math.Round(rawSpeed / (double)1000, 2);
                    }
                    else
                    {
                        speedUnit = SpeedUnitsBit.bit;
                        res = rawSpeed;
                    }
                    break;
                default:
                    goto case 1;
            }
            return res;
        }
    }
}
