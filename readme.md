## Pinger
Permite enviar ping simultaneos a múltiples hosts y visualizar todos los resultados en úna sóla pantalla.
Al archivo IPList.txt continene el listado de ip's a testear.

### Funcionamiento
MAX_TASKS define el número máximo de threads a usar.

El resultado se muestra en verde en caso satisfactorio, en rojo si el ping falla y amarillo en caso de que el tiempo sea inestable.


Para detectar la inestabilidad el programa computa el promedio y desvío estándar del ping para cada ip.
