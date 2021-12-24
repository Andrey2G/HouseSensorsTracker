import os
#get pin
pin=os.environ['SIM_PIN']
#start 3G Modem connection
os.system("/usr/bin/modem3g/sakis3g connect USBINTERFACE=\"1\" USBMODEM=\"19d2:0031\" APN=\"internet\" SIM_PIN=\""+pin+"\"")