for dir in /sys/bus/w1/devices/28*;
do

echo "device = $(basename $dir)"

# one value per 120 sec
# if no new data supplied for more than 300sec, temperature value is unknown, min value +10, max value +100
# archive areas
#  retention 60 days
# 43200 * 120 /60 / 60 / 60 = 60 days
rrdtool create temp-"$(basename $dir)".rrd --step 120 \
DS:temp:GAUGE:300:10:100 \
RRA:AVERAGE:0.5:1:43200 \
RRA:MIN:0.5:50:36000 \
RRA:MAX:0.5:50:36000 \
RRA:AVERAGE:0.5:50:36000;

done