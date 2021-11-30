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

Click on Write to write the image to the selected SD card

![Select Custom OS](images/rpi-imager-5.png)

![Select Custom OS](images/rpi-imager-6.png)

![Select Custom OS](images/rpi-imager-7.png)

![Select Custom OS](images/rpi-imager-8.png)

![Select Custom OS](images/rpi-imager-9.png)

Now remove your SD card and insert again

Add empty file with name ssh to enable SSH

Add file wpa_supplicant.conf with the following details to enable WiFi connection

<code>

country=US

ctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev

network={

    ssid="YOUR_NETWORK_NAME"

    psk="YOUR_PASSWORD"

    key_mgmt=WPA-PSK

}

</code>
