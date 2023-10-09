﻿
using pingTest;
using System.Runtime.InteropServices;


class PingResultDetail : PingerResult
{
    public float mean = 0;
    public float varianza = 0;
}

class PingTest
{

    static void Main(string[] args)
    {  
        int MAX_TASKS = 100;

        List<string> pingList = File.ReadLines("iplist.txt").ToList();
        MAX_TASKS = MAX_TASKS>pingList.Count?pingList.Count:MAX_TASKS;

        int i = 0;

        var pings = new List<Task<PingerResult>>();

        string ip;

        var tiempos = new Dictionary<string, PingResultDetail>();
        //crea todos los ping en el dictionario
        foreach(string newip in pingList)
        {
            tiempos.Add(newip, new PingResultDetail());
        }
        
        while (true)
        {
            while (pings.Count < MAX_TASKS)
            {
                ip = pingList[i];
                i = i >= pingList.Count-1 ? 0 : i+1;
                pings.Add(Task.Run(()=>Pinger.PingHost(ip,1500)));                
            }

            int finishedTaskID=Task.WaitAny(pings.ToArray());
            var finishedTask = pings[finishedTaskID];
            if (tiempos[finishedTask.Result.ip].ip == null) //en la primera pasada establece la media en el valor obtenido del ping
                tiempos[finishedTask.Result.ip].mean=finishedTask.Result.time>0? finishedTask.Result.time:0;
            tiempos[finishedTask.Result.ip].ip = finishedTask.Result.ip;
            tiempos[finishedTask.Result.ip].time = finishedTask.Result.time;
            tiempos[finishedTask.Result.ip].timestamp = finishedTask.Result.timestamp;
            float desvio = Math.Abs(finishedTask.Result.time - tiempos[finishedTask.Result.ip].mean) / tiempos[finishedTask.Result.ip].mean;
                if (finishedTask.Result.time > 0)
                {//calcula media y desvío
                    tiempos[finishedTask.Result.ip].mean += (finishedTask.Result.time - tiempos[finishedTask.Result.ip].mean) * .15f;
                    tiempos[finishedTask.Result.ip].varianza += (desvio * desvio - tiempos[finishedTask.Result.ip].varianza) * .15f;
                }

            Console.ForegroundColor = ConsoleColor.Black;
            if (finishedTask.Result.time >= 0)
            {
                Console.BackgroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
            }
            
            if (tiempos[finishedTask.Result.ip].mean > 5 && tiempos[finishedTask.Result.ip].varianza / tiempos[finishedTask.Result.ip].mean > 0.5)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
            }

            Console.CursorTop =  pingList.FindIndex(ip => ip == finishedTask.Result.ip);
            Console.WriteLine($"{finishedTask.Result.ip,-25} ping:{finishedTask.Result.time + "ms",-8} promedio:{Math.Round(tiempos[finishedTask.Result.ip].mean)+"ms",-8}  hora:{finishedTask.Result.timestamp.ToString("T")}".PadRight(Console.BufferWidth));
            Console.CursorVisible = false;
            Console.CursorTop = pingList.Count;

            Console.ForegroundColor= ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            int caidos = tiempos.Values.Where(s => s.time == -1).ToArray().Length;
            Console.WriteLine("".PadRight(Console.BufferWidth));
            Console.WriteLine($"Servidores caidos:  {caidos}/{pingList.Count,-36} hora:{DateTime.Now.ToString("T")}");
            Console.WriteLine("\nHaga click sobre la pantalla para pausar la ejecución. Para reanudar presione 'esc'");
            
            pings.Remove(finishedTask);

        }
    }


}