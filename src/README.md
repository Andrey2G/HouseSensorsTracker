## OS Raspberry OS 64 Lite







## usb-modeswitch
Install usb-modeswitch

sudo apt-get update

\# remove Modem Manager just in case exists

sudo apt-get remove ModemManager

sudo apt-get install ppp usb-modeswitch

## Saskis3G

Install Saskis3G

Sources: https://github.com/Trixarian/sakis3g-source

wget "http://raspberry-at-home.com/files/sakis3g.tar.gz"

sudo mkdir /usr/bin/modem3g

sudo chmod 777 /usr/bin/modem3g

sudo cp sakis3g.tar.gz /usr/bin/modem3g

cd /usr/bin/modem3g

sudo tar -zxvf sakis3g.tar.gz

sudo chmod +x sakis3g

Re-check 3G Modem connection in interactive mode (just to make sure that it's working fine)

sudo /usr/bin/modem3g/sakis3g --interactive "menu" "console"

## 3G modem connection
Check the list of USB devices

lsusb

Bus 002 Device 001: ID 1d6b:0003 Linux Foundation 3.0 root hub

Bus 001 Device 006: ID <b>19d2:0031</b> ZTE WCDMA Technologies MSM MF110/MF627/MF636

Bus 001 Device 002: ID 2109:3431 VIA Labs, Inc. Hub

Bus 001 Device 001: ID 1d6b:0002 Linux Foundation 2.0 root hub

copy ID of your USB modem (in my case it's <b>19d2:0031</b>)

Makea connection in command line

sudo /usr/bin/modem3g/sakis3g --console "connect" "USBMODEM=19d2:0031" "APN=internet" "USBINTERFACE=1" "SIM_PIN=XXXX"

Please, re-check APN value with your Mobile Operator, and username in APN_USER with password APN_PASS if exists

Please, provide pin code of your sim card in parameter SIM_PIN

