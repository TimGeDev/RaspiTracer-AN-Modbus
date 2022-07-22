# RaspiTracer-AN-Modbus
Basic Implementation of a Communication Bridge between my EPEVER Tracer AN 3210 and MQTT (Homeassistant). May also work with other models that appear to use the same protocol (LS-B, VS-B, Tracer-B, Tracer-A, iTracer, eTrace).

This application is currently reading live state values and sending them json encoded via mqtt, where the values can be used as entities

# Usage

1. Download or build the application
2. Set the credentials in the mqtt.json file
3. Execute on target machine
4. (optional) schedule by the use of a cronjob:
   
       * * * * * cd /root/RaspiTracer/ && DOTNET_ROOT="/opt/dotnet" ./RaspTracer-AN-Modbus >> /dev/null 2>&1

5. Configure in Homeassistant:
   
        mqtt:
          sensor:
            - name: "Battery Voltage"
              state_topic: "home/solar/battery"
              value_template: "{{ value_json.BATTERY_VOLTAGE }}"
   



# Hardware 
Coming soon.
# References

| **Usage**                            | **Source**                                                             |
|--------------------------------------|------------------------------------------------------------------------|
| EPEVER MODBUS Protocol documentation | https://www.img4.cz/i4wifi/attach/StoItem/7069/MODBUS-Protocol-v25.pdf                          |
| Useful Tips and general direction    | http://www.informatik.htw-dresden.de/~beck/TracerLogging.html          |
| CRC Libary                           | https://github.com/meetanthony/crccsharp |
| CRC Online Tool (CRC-16/MODBUS)                          | https://crccalc.com |

Note: when using the crccalc.com tool, make sure to reverse the bytes given as crc: 0xABCD => CDAB
