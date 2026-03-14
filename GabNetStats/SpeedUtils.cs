namespace GabNetStats
{
    internal static class SpeedUtils
    {
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
                        speedUnit = TrayIconManager.SpeedUnitsByte.GiB;
                        res = System.Math.Round(rawSpeed / (double)1073741824, 2);
                    }
                    else if (rawSpeed >= 1048576) //1048576 = 2 ^ 20
                    {
                        speedUnit = TrayIconManager.SpeedUnitsByte.MiB;
                        res = System.Math.Round(rawSpeed / (double)1048576, 2);
                    }
                    else if (rawSpeed >= 1024) //1024 = 2 ^ 10
                    {
                        speedUnit = TrayIconManager.SpeedUnitsByte.KiB;
                        res = System.Math.Round(rawSpeed / (double)1024, 2);
                    }
                    else
                    {
                        speedUnit = TrayIconManager.SpeedUnitsByte.Bytes;
                        res = rawSpeed;
                    }
                    break;

                case 2:
                    if (rawSpeed >= 1000000000)
                    {
                        speedUnit = TrayIconManager.SpeedUnitsBit.Gbit;
                        res = System.Math.Round(rawSpeed / (double)1000000000, 2);
                    }
                    else if (rawSpeed >= 1000000)
                    {
                        speedUnit = TrayIconManager.SpeedUnitsBit.Mbit;
                        res = System.Math.Round(rawSpeed / (double)1000000, 2);
                    }
                    else if (rawSpeed >= 1000)
                    {
                        speedUnit = TrayIconManager.SpeedUnitsBit.Kbit;
                        res = System.Math.Round(rawSpeed / (double)1000, 2);
                    }
                    else
                    {
                        speedUnit = TrayIconManager.SpeedUnitsBit.bit;
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
