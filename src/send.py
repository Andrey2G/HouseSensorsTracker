import os
import glob
import requests
import rrdtool
import json


def send_data(sensor, payload):
    result = requests.post('http://localhost:8088/api/sensors/'+sensor,data=json.dumps(payload))
    print("sensor=",sensor,"data=",payload,"send result=",result)


data_files = glob.glob(os.path.dirname(os.path.abspath(__file__)) + '/28*')
for rrd in data_files:
    result = rrdtool.fetch(rrd, "AVERAGE", "-s", "-1h")
    start, end, step = result[0]
    ds = result[1]
    rows = result[2]
    print("rrd=",rrd,"start=",start," end=",end,"step=",step,"ds=",ds,"rows=",rows)
    payloadData=[]
    ts=start
    for t in rows:
        payloadData.append({'ts':ts, 't':t[0]})
        ts=ts+step

    sensor = os.path.splitext(os.path.basename(rrd))[0]
    send_data(sensor,payloadData)