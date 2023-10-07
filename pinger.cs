using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;



namespace pingTest
{
    public class PingerResult
    {
        public string ip;
        public int time;
        public DateTime timestamp;
    }


    public class Pinger
    {

        /// <summary>
        /// Manda count pings y devuelve el mayor de los tiempos en la propiedad time. En caso de fallar devuelve -1
        /// </summary>
        public async static Task<PingerResult> PingHost(string ip, int count, int timeout = 1000)
        {
            Ping pinger = new Ping();
            var options = new PingOptions();
            options.DontFragment = true;
            long maxPing = -1;
            
            try
            {
                PingReply PR;

                for (int i = 0; i < count; i++)
                {
                    PR = await pinger.SendPingAsync(ip, timeout);

                    if (PR.Status != IPStatus.Success)
                        return new PingerResult { ip = ip, time = -1 , timestamp=DateTime.Now};
                    else
                        maxPing = PR.RoundtripTime > maxPing ? PR.RoundtripTime : maxPing;
                    Thread.Sleep(500);
                }
            }
            catch (Exception e)
            {
                return new PingerResult { ip = ip, time = -1, timestamp = DateTime.Now };
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return new PingerResult { ip = ip, time = (int)maxPing, timestamp = DateTime.Now };
        }
    }

}
