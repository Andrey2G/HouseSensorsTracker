## OS Raspberry OS 64 Lite

https://downloads.raspberrypi.org/raspios_lite_arm64/images/

Select the latest version and download zip file with OS image.

![Start Raspberry PI Imager](images/raspios-image.png)

Download and install [Raspberry PI Imager](https://www.raspberrypi.com/software/) software and run it.

![Start Raspberry PI Imager](images/rpi-imager-1.png)

To choose OS image click on Choose OS and select "Use Custom"

![Select OS Imager](images/rpi-imager-2.png)

And then choose downloaded zip file

![Select Custom OS](images/rpi-imager-3.png)

Click on Choose Storage to select SD card

![Select Custom OS](images/rpi-imager-4.png)

Click Ctrl+Shift+X to display Advanced Options

Disable Ovescan, set hostname (if youlike), enable SSH and set the password for the "pi" user

![Select Custom OS](images/rpi-imager-4-1.png)

Configure WiFi connection (of course in case if you will use USB WiFi stick) by set SSID and password,
set locale settings

![Select Custom OS](images/rpi-imager-4-2.png)

Set "Skip first-run wizard", and set "Eject media when finished"

![Select Custom OS](images/rpi-imager-4-3.png)

Click on Write to write the image to the selected SD card

![Select Custom OS](images/rpi-imager-5.png)

![Select Custom OS](images/rpi-imager-6.png)

![Select Custom OS](images/rpi-imager-7.png)

![Select Custom OS](images/rpi-imager-8.png)

![Select Custom OS](images/rpi-imager-9.png)

Boot the system, and enable the One-Wire interface (Check more details about it [here](https://pinout.xyz/pinout/1_wire))

sudo nano /boot/config.txt

and add this to the bottom of the file:

dtoverlay=w1-gpio

Exit Nano with saving changes and reboot

sudo reboot

Now enable modules w1-gpio and w1-therm

sudo modprobe w1-gpio

sudo modprobe w1-therm

Now you will be able to see your temperarure sensor(s)

ls /sys/bus/w1/devices

## RRD Tool

[About RRD Tool](https://www.mrtg.org/rrdtool/)

Install dependencies (see more details [here](https://pythonhosted.org/rrdtool/install.html#debian-ubuntu))

sudo apt-get install librrd-dev libpython3-dev

Install PIP

sudo apt-get install pip

Install RDD Tool

sudo apt-get install rrdtool python-rrdtool