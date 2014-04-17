#!/bin/bash

cd ~/vice
sudo unbuffer ./controller | ./timestamp.sh > vfdLog.log &
sudo unbuffer python touchscreen.py | ./timestamp.sh > touchscreenLog.log
