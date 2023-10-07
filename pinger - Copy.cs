using System.Net.NetworkInformation;

namespace pingTest
{

    public class Pingerddd
    {

        /// <summary>
        /// Manda count pings y devuelve el mayor de los tiempos. Si alguno falla devuelve -1
        /// </summary>
        public static int PingHost(string ip, int count)
        {
            Ping pinger = new Ping();
            int timeout = 500;
            var options = new PingOptions();
            options.DontFragment = true;
            long maxPing = -1;

            try
            {
                PingReply PR;

                for (int i = 0; i < count; i++)
                {
                    //Debug.WriteLine($"ping {ip}: count {i + 1}");
                    PR = pinger.Send(ip, timeout);
                    if (PR.Status != IPStatus.Success)
                        return -1;
                    else
                        maxPing = PR.RoundtripTime > maxPing ? PR.RoundtripTime : maxPing;
                }
            }
            catch (Exception e)
            {
                return -1;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return (int)maxPing;
        }
    }

}
