import os
import glob
import time
import datetime
import rrdtool
 
os.system('modprobe w1-gpio')
os.system('modprobe w1-therm')
 
base_dir = '/sys/bus/w1/devices/'

#sample response
#7c 01 4b 46 7f ff 0c 10 7f : crc=7f YES
#7c 01 4b 46 7f ff 0c 10 7f t=23750
def read_temperature_sensor(device_dir):
        tfile = open(device_dir)
        text = tfile.read()
        tfile.close()
        #need to get the 2nd line
        line = text.split("\n")[1]
        #take the 9th item with t=23750
        temperaturedata = line.split(" ")[9]
        #take the value
        temperature = float(temperaturedata[2:])
        #make it in Celsuius
        temperature = temperature / 1000
        return temperature

def read_sensors_data():
        devices = glob.glob(base_dir + '28*')
        for device_dir in devices:
                device = device_dir + '/w1_slave'
                device_name = os.path.basename(device_dir)
                data = read_temperature_sensor(device)
                now = datetime.datetime.now()
                write_data(device_name,'N:'+str(data))


def write_data(device, data):
        if not os.path.exists(device+'.rrd'):
                print('create rrd for',device)
                rrdtool.create( device+'.rrd',
                '--start','now',
                '--step','120',
                'DS:temp:GAUGE:300:10:100',
                'RRA:AVERAGE:0.5:1:43200',
                'RRA:MIN:0.5:50:36000',
                'RRA:MAX:0.5:50:36000',
                'RRA:AVERAGE:0.5:50:36000')

        print (datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S"),"device=",device,"temperature=",data)
        rrdtool.update(device+'.rrd',data)

while True:
	read_sensors_data()
	time.sleep(5)